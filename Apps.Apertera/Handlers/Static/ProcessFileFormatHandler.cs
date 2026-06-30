using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Apertera.Handlers.Static;

public class ProcessFileFormatHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
    [
        new("application/xliff+xml", "Interoperable XLIFF (default)"),
        new("xliff1", "XLIFF 1.2"),
        new("original", "Original data format"),
    ];
}
