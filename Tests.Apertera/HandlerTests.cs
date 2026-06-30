using Apps.Apertera.Handlers.Static;
using Tests.Apertera.Base;

namespace Tests.Apertera;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public void LanguageHandler_ReturnsLanguages()
    {
        var handler = new LanguageDataSourceHandler();
        var result = handler.GetData().ToList();

        Console.WriteLine($"Total languages: {result.Count}");
        foreach (var item in result)
            Console.WriteLine($"  {item.Value}: {item.DisplayName}");

        Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public void ProcessFileFormatHandler_ReturnsFormats()
    {
        var handler = new ProcessFileFormatHandler();
        var result = handler.GetData().ToList();

        Console.WriteLine($"Total formats: {result.Count}");
        foreach (var item in result)
            Console.WriteLine($"  {item.Value}: {item.DisplayName}");

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void FileTranslationStrategyHandler_ReturnsStrategies()
    {
        var handler = new FileTranslationStrategyHandler();
        var result = handler.GetData().ToList();

        Console.WriteLine($"Total strategies: {result.Count}");
        foreach (var item in result)
            Console.WriteLine($"  {item.Value}: {item.DisplayName}");

        Assert.AreEqual(2, result.Count);
    }
}
