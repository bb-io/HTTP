using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.HTTP;

public class HttpApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.Utilities];
        set { }
    }
    
    public string Name
    {
        get => "HTTP Application";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}