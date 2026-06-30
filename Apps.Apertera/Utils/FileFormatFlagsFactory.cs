namespace Apps.Apertera.Utils;

public static class FileFormatFlagsFactory
{
    public static Dictionary<string, string> GetFlags(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf"          => new() { ["use_ocr"] = "false", ["export_as_pdf"] = "true" },
            ".docx"         => new() { ["export_as_docx"] = "true" },
            ".pptx"         => new() { ["include_notes"] = "true", ["export_as_pptx"] = "true" },
            ".potx"         => new() { ["include_notes"] = "true", ["export_as_potx"] = "true" },
            ".xlsx"         => new() { ["export_as_xlsx"] = "true", ["include_hidden"] = "false" },
            ".vsdx"         => new() { ["export_as_vsdx"] = "true" },
            ".xliff" or ".xlf" => new() { ["export_as_text"] = "true" },
            ".txt"          => new() { ["export_as_text"] = "true" },
            _               => new()
        };
    }
}
