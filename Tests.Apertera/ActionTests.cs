using Apps.Apertera.Actions;
using Apps.Apertera.Models.Requests;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Apertera.Base;

namespace Tests.Apertera;

[TestClass]
public class ActionTests : TestBase
{
    private TranslationActions Actions => new(InvocationContext, FileManager);

    // ─── Text ────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task TranslateText_EnglishToFrench_ReturnsTranslatedText()
    {
        var result = await Actions.TranslateText(new TextTranslationRequest
        {
            Text = "Hello, world! This is a test.",
            SourceLanguage = "eng",
            TargetLanguage = "fra",
        });

        Console.WriteLine($"Translated: {result.TranslatedText}");
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TranslatedText));
        Assert.AreNotEqual("Hello, world! This is a test.", result.TranslatedText);
    }

    // ─── HTML – Blackbird interop ─────────────────────────────────────────────

    [TestMethod]
    public async Task TranslateContent_Html_Blackbird_DefaultXliff2()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "The Loire Valley_en-US.html", ContentType = "text/html" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
        Assert.IsTrue(result.File.Name.EndsWith(".xlf") || result.File.Name.EndsWith(".xliff"),
            $"Expected XLIFF output, got: {result.File.Name}");
    }

    [TestMethod]
    public async Task TranslateContent_Html_Blackbird_Xliff1()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "The Loire Valley_en-US.html", ContentType = "text/html" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
            OutputFileHandling = "xliff1",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
        Assert.IsTrue(result.File.Name.EndsWith(".xlf") || result.File.Name.EndsWith(".xliff"),
            $"Expected XLIFF output, got: {result.File.Name}");
    }

    [TestMethod]
    public async Task TranslateContent_Html_Blackbird_OriginalFormat()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "The Loire Valley_en-US.html", ContentType = "text/html" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
            OutputFileHandling = "original",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
        Assert.IsTrue(result.File.Name.EndsWith(".html"),
            $"Expected HTML output, got: {result.File.Name}");
    }

    // ─── DOCX – Blackbird interop ─────────────────────────────────────────────

    [TestMethod]
    public async Task TranslateContent_Docx_Blackbird_DefaultXliff2()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "translatable.docx", ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
        Assert.IsTrue(result.File.Name.EndsWith(".xlf") || result.File.Name.EndsWith(".xliff"),
            $"Expected XLIFF output, got: {result.File.Name}");
    }

    // ─── TXT – Blackbird interop (PlaintextCoder) ─────────────────────────────

    [TestMethod]
    public async Task TranslateContent_Txt_Blackbird_DefaultXliff2()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "translatable.txt", ContentType = "text/plain" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
    }

    // ─── PDF – auto-routed to native regardless of strategy ──────────────────

    [TestMethod]
    public async Task TranslateContent_Pdf_AutoRoutedToNative()
    {
        // PDF always uses the native Apertera strategy, even when blackbird is selected (or unset)
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "translatable.pdf", ContentType = "application/pdf" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
            // no strategy set — should silently route to native
        });

        Console.WriteLine($"Output: {result.File.Name}, Type: {result.File.ContentType}");
        Assert.IsNotNull(result.File);
        Assert.IsTrue(result.File.Name.EndsWith(".pdf"), $"Expected PDF output, got: {result.File.Name}");
    }

    // ─── Native (Apertera) strategy – all four files ───────────────────────────

    [TestMethod]
    public async Task TranslateContent_Html_Native()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "The Loire Valley_en-US.html", ContentType = "text/html" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
            FileTranslationStrategy = "apertera",
        });

        Console.WriteLine($"Output: {result.File.Name}, Type: {result.File.ContentType}");
        Assert.IsNotNull(result.File);
    }

    [TestMethod]
    public async Task TranslateContent_Docx_Native()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "translatable.docx", ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
            FileTranslationStrategy = "apertera",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
    }

    [TestMethod]
    public async Task TranslateContent_Txt_Native()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "translatable.txt", ContentType = "text/plain" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
            FileTranslationStrategy = "apertera",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
    }

    [TestMethod]
    public async Task TranslateContent_Pdf_Native()
    {
        var result = await Actions.TranslateContent(new ContentTranslationRequest
        {
            File = new FileReference { Name = "translatable.pdf", ContentType = "application/pdf" },
            SourceLanguage = "eng",
            TargetLanguage = "fra",
            FileTranslationStrategy = "apertera",
        });

        Console.WriteLine($"Output: {result.File.Name}");
        Assert.IsNotNull(result.File);
    }
}
