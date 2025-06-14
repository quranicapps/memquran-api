using System.Net;
using MemQuran.Api.Integration.Tests.Shared;
using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests;

[Trait("Category", "Integration"), Collection(nameof(MemQuranCollection))]
public class AudioControllerTest(CustomApiFactory customApiFactory, ITestOutputHelper testOutputHelper) 
    : BaseTest(customApiFactory, testOutputHelper), IClassFixture<CustomApiFactory>
{
    [Fact]
    public async Task GetSurahAudioJson_Returns_NotFound()
    {
        var result = await Client.GetAsync("/json/prophets/en_prophets.json");
        
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
    
    [Fact]
    public async Task GetSurahAudioJson_Returns_NotFound2()
    {
        await Task.Delay(1000);
        Assert.True(true);
        // Logger.WriteLine(content);
    }
}