using Apps.Apertera.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Apertera.Handlers.Static;

public class LanguageDataSourceHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
        LanguageConstants.Languages.Select(x => new DataSourceItem(x.Key, x.Value));
}
