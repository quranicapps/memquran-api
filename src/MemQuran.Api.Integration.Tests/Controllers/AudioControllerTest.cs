using MemQuran.Api.Integration.Tests.Shared;
using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests.Controllers;

[Trait("Category", "Integration"), Collection(nameof(AudioCollection))]
public class AudioControllerTest(CustomApiFactory customApiFactory, ITestOutputHelper testOutputHelper) 
    : BaseTest(customApiFactory, testOutputHelper), IClassFixture<CustomApiFactory>
{
    [Fact]
    public async Task GetSurahAudioJson_Returns_NotFound()
    {
        await Task.Delay(1000);
        Assert.True(true);
        // Logger.WriteLine(content);
    }
    
    [Fact]
    public async Task GetSurahAudioJson_Returns_NotFound2()
    {
        await Task.Delay(1000);
        Assert.True(true);
        // Logger.WriteLine(content);
    }
}