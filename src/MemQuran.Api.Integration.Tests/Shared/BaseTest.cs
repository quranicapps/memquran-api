using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests.Shared;

// The test and base class created once for each unit test 

public class BaseTest(CustomApiFactory customApiFactory, ITestOutputHelper testOutputHelper)
{
    protected ITestOutputHelper Logger { get; } = testOutputHelper;
    protected HttpClient Client { get; private set; } = customApiFactory.CreateClient();
}