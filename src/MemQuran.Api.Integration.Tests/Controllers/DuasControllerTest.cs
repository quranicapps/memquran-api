using MemQuran.Api.Integration.Tests.Shared;
using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests.Controllers;

[Trait("Category", "Integration"), Collection(nameof(DuaCollection))]
public class DuasControllerTest(CustomApiFactory customApiFactory, ITestOutputHelper testOutputHelper) 
    : BaseTest(customApiFactory, testOutputHelper), IClassFixture<CustomApiFactory>
{
    [Fact]
    public async Task Test1()
    {
        await Task.Delay(1000);
        Assert.True(true);
    } 
    
    [Fact]
    public async Task Test2()
    {
        await Task.Delay(1000);
        Assert.True(true);
    } 
    
    [Fact]
    public async Task Test3()
    {
        await Task.Delay(1000);
        Assert.True(true);
    } 
}