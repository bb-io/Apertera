using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.Apertera.Models.Responses;

public class TextTranslationResponse : ITranslateTextOutput
{
    [Display("Translated text")]
    public string TranslatedText { get; set; } = string.Empty;
}
