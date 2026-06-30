using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Apertera.Handlers.Static;

public class FileTranslationStrategyHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("blackbird", "Blackbird interoperable (default)"),
        new("apertera", "Apertera native"),
    ];
}
