using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Apertera.Models.Dtos;

public class TextTranslationResultDto
{
    // API returns a string for single input and an array for multiple inputs
    [JsonProperty("Translation")]
    public JToken? Translation { get; set; }

    [JsonProperty("engine")]
    public string? Engine { get; set; }

    public IReadOnlyList<string> GetOutputs() =>
        Translation switch
        {
            JArray arr => arr.Values<string?>().Select(s => s ?? string.Empty).ToList(),
            not null   => [Translation.ToString()],
            _          => []
        };
}
