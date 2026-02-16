using Apps.HTTP.DataHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.HTTP.Models.Requests;

public class PostRequest : Request
{
    private readonly string _body;
    
    [Display("Content type")]
    [StaticDataSource(typeof(ContentTypeDataHandler))]
    public string? ContentType { get; set; }
    
    [Display("Is request body in JSON format")]
    public bool IsBodyInJsonFormat { get; init; }
    
    [Display("Request body")]
    public string Body { get => _body; init => _body = ReplaceBrackets(value)!; }

    [Display("File")]
    public FileReference? File { get; set; }

    [Display("Form field name for the file. Default is 'file'")]
    public string? FieldName { get; set; }
}