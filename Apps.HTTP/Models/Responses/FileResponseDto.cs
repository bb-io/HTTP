using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.HTTP.Models.Responses;

public class FileResponseDto
{
    public FileResponseDto(FileReference contentFile, string contentType, string fileName)
    {
        ContentFile = contentFile;
        ContentType = contentType;
        FileName = fileName;
    }

    [Display("Content file")]
    public FileReference ContentFile { get; set; }

    [Display("Content type")]
    public string ContentType { get; set; }

    [Display("File name")]
    public string FileName { get; set; }
}