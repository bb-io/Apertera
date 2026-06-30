using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Apertera;

public class Application : IApplication, ICategoryProvider
{
    public string Name => "Apertera";

    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.MachineTranslationAndMtqe];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}
