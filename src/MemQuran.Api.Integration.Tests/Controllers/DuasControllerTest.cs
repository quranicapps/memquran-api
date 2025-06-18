using System.Net;
using System.Net.Http.Json;
using MemQuran.Api.Integration.Tests.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests.Controllers;

[Trait("Category", "Integration"), Collection(nameof(WireMockCollection))]
public class DuasControllerTest(CustomApiFactory customApiFactory, ITestOutputHelper testOutputHelper) 
    : IClassFixture<CustomApiFactory>
{
    [Theory]
    [InlineData("/json/duas/en_duas.json")]
    public async Task Get_Returns_Ok(string url)
    {
        var server = customApiFactory.SharedFixture.WireMockServer;
        var appSettingVersion = customApiFactory.ClientsSettings.JsDelivrService.Version;
        server.Given(Request
                .Create()
                .WithPath($"/gh/quranstatic/static@{appSettingVersion}/json/duas/*")
                .UsingGet())
            .RespondWith(Response.Create().WithStatusCode(StatusCodes.Status200OK));
        
        var response = await customApiFactory.Client.GetAsync(url);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Theory]
    [InlineData("/json/duas/en_duas.json")]
    public async Task Get_Returns_NotFound(string url)
    {
        var server = customApiFactory.SharedFixture.WireMockServer;
        var appSettingVersion = customApiFactory.ClientsSettings.JsDelivrService.Version;
        server.Given(Request
                .Create()
                .WithPath($"/gh/quranstatic/static@{appSettingVersion}/json/duas/*")
                .UsingGet())
            .RespondWith(Response.Create().WithStatusCode(StatusCodes.Status404NotFound));
        
        var response = await customApiFactory.Client.GetAsync(url);

        Assert.NotNull(response);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(content);
        Assert.Equal("Not Found", content.Title);
        Assert.Equal(StatusCodes.Status404NotFound, content.Status);
    }
}