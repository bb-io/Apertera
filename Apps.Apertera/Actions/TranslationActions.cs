using Apps.Apertera.Models.Requests;
using Apps.Apertera.Models.Responses;
using Apps.Apertera.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Bilingual.Xliff1;
using Blackbird.Filters.Coders;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;
using System.Text;
using Apps.Apertera.Models.Entities;

namespace Apps.Apertera.Actions;

[ActionList("Translation")]
public class TranslationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : Invocable(invocationContext)
{
    private const int MaxSpecificCoderCount = 4;

    [BlueprintActionDefinition(BlueprintAction.TranslateText)]
    [Action("Translate text", Description = "Translate text using Alexa Translations AI.")]
    public async Task<TextTranslationResponse> TranslateText([ActionParameter] TextTranslationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            throw new PluginMisconfigurationException("The text cannot be empty. Please fill in the 'Text' field.");

        if (string.IsNullOrWhiteSpace(request.SourceLanguage))
            throw new PluginMisconfigurationException("The source language cannot be empty. Please fill in the 'Source language' field.");

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
            throw new PluginMisconfigurationException("The target language cannot be empty. Please fill in the 'Target language' field.");

        var result = await Client.TranslateTextAsync([request.Text], request.SourceLanguage, request.TargetLanguage);
        var translatedText = result.GetOutputs().FirstOrDefault() ?? string.Empty;

        return new TextTranslationResponse { TranslatedText = translatedText };
    }

    [BlueprintActionDefinition(BlueprintAction.TranslateFile)]
    [Action("Translate", Description = "Translate a file using Alexa Translations AI. Supports XLIFF interoperability and native file translation.")]
    public async Task<TranslateFileResponse> TranslateContent([ActionParameter] ContentTranslationRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.TargetLanguage))
            throw new PluginMisconfigurationException("The target language cannot be empty. Please fill in the 'Target language' field.");
        
        if (string.IsNullOrWhiteSpace(input.SourceLanguage))
            throw new PluginMisconfigurationException("The source language cannot be empty. Alexa Translations requires an explicit source language.");

        var isPdf = Path.GetExtension(input.File.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        if (input.FileTranslationStrategy == "apertera" || isPdf)
            return await TranslateDocumentNatively(input);

        var specificCoders = CoderFactory.PossibleFromFileOrMediaType(input.File.Name, null).ToList();
        if (specificCoders.Count > MaxSpecificCoderCount)
        {
            var ext = Path.GetExtension(input.File.Name).TrimStart('.').ToUpperInvariant();
            throw new PluginMisconfigurationException(
                $"The {ext} file format is not supported in Blackbird interoperability mode. " +
                $"Supported formats: HTML, DOCX, PPTX, XLIFF, XML, TXT. " +
                $"To translate this file, switch to the 'Apertera native' file translation strategy.");
        }

        var stream = await fileManagementClient.DownloadAsync(input.File);
        var loadResult = Transformation.Load(stream, input.File.Name, null);

        if (!loadResult.Success)
            throw new PluginApplicationException(
                $"Could not parse the file '{input.File.Name}' for interoperable translation: {loadResult.Error}. " +
                $"Try switching to the 'Apertera native' file translation strategy.");

        return await HandleInteroperableTransformation(loadResult.Value, input);
    }

    private async Task<TranslateFileResponse> HandleInteroperableTransformation(
        Transformation content, ContentTranslationRequest input)
    {
        content.SourceLanguage ??= input.SourceLanguage;
        content.TargetLanguage ??= input.TargetLanguage.ToLower();

        async Task<IEnumerable<string>> BatchTranslate(IEnumerable<(Unit Unit, Segment Segment)> batch)
        {
            var sources = batch.Select(x => x.Segment.GetSource() ?? string.Empty);
            var result = await Client.TranslateTextAsync(sources, input.SourceLanguage!, input.TargetLanguage);
            return result.GetOutputs();
        }

        var translations = await content.GetUnits()
            .Batch(100, seg => !seg.IsIgnorbale && seg.IsInitial && !string.IsNullOrEmpty(seg.GetSource()))
            .Process(BatchTranslate);

        foreach (var (unit, results) in translations)
        {
            var sourceChars = 0;
            foreach (var (segment, translatedText) in results)
            {
                segment.SetTarget(translatedText);
                segment.State = SegmentState.Translated;
                sourceChars += segment.GetSource()?.Length ?? 0;
            }
            unit.Provenance.Translation.Tool = "Alexa Translations";
            unit.AddUsage("Alexa Translations", sourceChars, UsageUnit.Characters);
        }

        if (input.OutputFileHandling == "original")
        {
            var targetResult = content.Target();
            if (!targetResult.Success)
                throw new PluginMisconfigurationException(
                    "The original file content could not be retrieved. " +
                    "Please change 'Output file handling' to 'Interoperable XLIFF (default)' or use the Apertera native strategy.");

            var targetContent = targetResult.Value;
            return new TranslateFileResponse
            {
                File = await fileManagementClient.UploadAsync(
                    targetContent.ToStream(MetadataHandling.Exclude),
                    targetContent.OriginalMediaType,
                    targetContent.OriginalName)
            };
        }

        if (input.OutputFileHandling == "xliff1")
        {
            var xliff1String = Xliff1Serializer.Serialize(content);
            return new TranslateFileResponse
            {
                File = await fileManagementClient.UploadAsync(
                    xliff1String.ToStream(Encoding.UTF8),
                    MediaTypes.Xliff1,
                    content.BilingualFileName)
            };
        }

        return new TranslateFileResponse
        {
            File = await fileManagementClient.UploadAsync(
                content.ToStream(),
                MediaTypes.Xliff2,
                content.BilingualFileName)
        };
    }

    private async Task<TranslateFileResponse> TranslateDocumentNatively(ContentTranslationRequest input)
    {
        var stream = await fileManagementClient.DownloadAsync(input.File);
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        var docReq = new DocumentTranslationDto
        {
            FileContent = memoryStream.ToArray(),
            FileName = input.File.Name,
            SourceLanguage = input.SourceLanguage,
            TargetLanguage = input.TargetLanguage,
            ProjectId = input.ProjectId,
            FormatFlags = FileFormatFlagsFactory.GetFlags(input.File.Name),
        };

        var resultStream = await Client.TranslateDocumentAsync(docReq);

        return new TranslateFileResponse
        {
            File = await fileManagementClient.UploadAsync(resultStream, input.File.ContentType, input.File.Name)
        };
    }
}
