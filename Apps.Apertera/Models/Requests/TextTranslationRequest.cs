using Apps.Apertera.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.Apertera.Models.Requests;

public class TextTranslationRequest : ITranslateTextInput
{
    [Display("Text", Description = "Text to translate")]
    public string Text { get; set; } = string.Empty;

    [Display("Source language", Description = "The source language for translation"), StaticDataSource(typeof(LanguageDataSourceHandler))]
    public string SourceLanguage { get; set; } = string.Empty;

    [Display("Target language", Description = "The target language for translation"), StaticDataSource(typeof(LanguageDataSourceHandler))]
    public string TargetLanguage { get; set; } = string.Empty;
}
