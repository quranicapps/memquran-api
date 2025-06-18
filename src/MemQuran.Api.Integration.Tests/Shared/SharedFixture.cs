using MemQuran.Api.Integration.Tests.Shared.WireMock;

namespace MemQuran.Api.Integration.Tests.Shared;

// Created once for all tests of the same collection name
// because of Collection(nameof(MemQuranCollection)) attribute on the test classes

public class SharedFixture : IAsyncLifetime
{
    public string WireMockBaseUrl { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        WireMockBaseUrl = await WireMockSingleton.Instance;

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}