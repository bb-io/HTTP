using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.HTTP.DataHandlers;

public class ContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new[]
        {
            new DataSourceItem("application/json", "Application/json"),
            new DataSourceItem("application/x-www-form-urlencoded", "Form (URL-encoded)"),
            new DataSourceItem("multipart/form-data", "Multipart form data")
        };
    }
}
