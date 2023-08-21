using Blackbird.Applications.Sdk.Common;

namespace Apps.HTTP;

public class HttpApplication : IApplication
{
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