namespace Apps.Apertera.Models.Entities;

public class DocumentTranslationDto
{
    public byte[] FileContent { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
    public string? ProjectId { get; set; }
    public Dictionary<string, string> FormatFlags { get; set; } = new();
}
