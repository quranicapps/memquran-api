using System.Net;
using MemQuran.Api.Integration.Tests.Shared;
using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests.Controllers;

[Trait("Category", "Integration"), Collection(nameof(ProphetsCollection))]
public class ProphetsControllerTest(CustomApiFactory customApiFactory, ITestOutputHelper testOutputHelper) 
    : BaseTest(customApiFactory, testOutputHelper), IClassFixture<CustomApiFactory>
{
    [Fact]
    public async Task GetProphets_Returns_NotFound()
    {
        var result = await Client.GetAsync("/json/prophets/en_prophets.json");
        
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
    
    [Fact]
    public async Task GetProphets_Returns_NotFound2()
    {
        var result = await Client.GetAsync("/json/prophets/en_prophets.json");
        
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
    
    [Fact]
    public async Task GetProphets_Returns_NotFound3()
    {
        var result = await Client.GetAsync("/json/prophets/en_prophets.json");
        
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}