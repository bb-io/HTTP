using System.Text.Json;
using Apps.HTTP;
using Apps.HTTP.Models.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.HTTP.Base;

namespace Tests.HTTP;

[TestClass]
public class ActionsTests : TestBase
{
    [TestMethod]
    public async Task Post_WithValidParameters_ShouldReturnSuccessResponse()
    {
        var actions = new Actions(InvocationContext, FileManager);
        var postRequest = new PostRequest
        {
            Endpoint = "pipelines/",
            IsBodyInJsonFormat = true,
            Body = @"{ ""ping"": ""pong"" }",
            Headers = @"{ ""Authorization"": ""Bearer ..."" }"
        };

        var result = await actions.Post(postRequest);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.StatusCode);

        Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
    }
}