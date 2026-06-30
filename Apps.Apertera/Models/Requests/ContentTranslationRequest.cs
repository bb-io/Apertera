using Apps.Apertera.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.Apertera.Models.Requests;

public class ContentTranslationRequest : ITranslateFileInput
{
    [Display("File")]
    public FileReference File { get; set; } = default!;

    [Display("Source language", Description = "The source language for translation"), StaticDataSource(typeof(LanguageDataSourceHandler))]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language", Description = "The target language for translation"), StaticDataSource(typeof(LanguageDataSourceHandler))]
    public string TargetLanguage { get; set; } = string.Empty;

    [Display("Output file handling", Description = "Determine the format of the output file. The default is to convert to XLIFF for downstream steps."), StaticDataSource(typeof(ProcessFileFormatHandler))]
    public string? OutputFileHandling { get; set; }

    [Display("File translation strategy", Description = "Select whether to use Apertera's native file processing or Blackbird interoperability mode"), StaticDataSource(typeof(FileTranslationStrategyHandler))]
    public string? FileTranslationStrategy { get; set; }

    [Display("Project ID", Description = "Optional project ID for translation")]
    public string? ProjectId { get; set; }
}
