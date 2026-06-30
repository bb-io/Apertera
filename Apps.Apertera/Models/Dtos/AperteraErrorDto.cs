using Newtonsoft.Json;

namespace Apps.Apertera.Models.Dtos;

public class AperteraErrorDto
{
    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonProperty("detail")]
    public string? Detail { get; set; }

    public string? FirstNonEmpty() => Message ?? Error ?? Detail;
}
