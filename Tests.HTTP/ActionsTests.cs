using System.Text.Json;
using Apps.HTTP;
using Apps.HTTP.Models.Requests;
using Blackbird.Applications.Sdk.Common.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HTTP.Base;

namespace Tests.HTTP;

[TestClass]
public class ActionsTests : TestBase
{
    [TestMethod]
    public async Task Post_WithJson_ReturnsSuccessfulResponse()
    {
        var actions = new Actions(InvocationContext, FileManager);
        var postRequest = new PostRequest
        {
            Endpoint = "/post",
            IsBodyInJsonFormat = true,
            Body = @"{ ""ping"": ""pong"" }",
            Headers = @"{ ""Authorization"": ""Bearer ..."" }"
        };

        var result = await actions.Post(postRequest);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StatusCode);

        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }

    [TestMethod]
    public async Task Post_WithJsonAndFile_ReturnsSuccessfulResponse()
    {
        // Arrange
        var actions = new Actions(InvocationContext, FileManager);
        var postRequest = new PostRequest
        {
            Endpoint = "/post",
            IsBodyInJsonFormat = true,
            Body = @"{ ""test"": ""aha"" }",
            Headers = @"{ ""Authorization"": ""Bearer ..."" }",
            File = new FileReference { Name = "hello.txt" }
        };

        // Act
        var result = await actions.Post(postRequest);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StatusCode);

        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }

    [TestMethod]
    public async Task Post_WithFile_ReturnsSuccessfulResponse()
    {
        // Arrange
        var actions = new Actions(InvocationContext, FileManager);
        var postRequest = new PostRequest
        {
            Endpoint = "/post",
            File = new FileReference { Name = "123.xlsx" },
            FieldName = "table",
            Body = "",
            IsBodyInJsonFormat = false
        };

        // Act
        var result = await actions.Post(postRequest);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StatusCode);

        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }

    [TestMethod]
    public async Task GetFile_WithValidParameters_ReturnsFileWithOriginalFilename()
    {
        // Arrange
        var actions = new Actions(InvocationContext, FileManager);
        var request = new GetRequest
        {
            Endpoint = "/response-headers",
            QueryParameters = "{ \"Content-Disposition\": \"attachment; filename=\\\"document.txt\\\"\" }"
        };

        // Act
        var result = await actions.GetFile(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StatusCode);

        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }

    [TestMethod]
    public async Task GetFile_WithInvalidContentDispositionHeader_ReturnsFileWithGuidFilename()
    {
        // Arrange
        var actions = new Actions(InvocationContext, FileManager);
        var request = new GetRequest
        {
            Endpoint = "/response-headers",
            QueryParameters = "{ \"Content-Disposition\": \"attachment;\" }"
        };

        // Act
        var result = await actions.GetFile(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StatusCode);

        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }

    [TestMethod]
    public async Task GetFile_WithSemicolonSeparatedContentTypeHeader_ReturnsFileWithOneContentTypeHeader()
    {
        // Arrange
        var actions = new Actions(InvocationContext, FileManager);
        var request = new GetRequest
        {
            Endpoint = "/tests/xhtml/testfiles/resources/pdf/dummy.pdf"
        };

        // Act
        var result = await actions.GetFile(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StatusCode);

        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }
}