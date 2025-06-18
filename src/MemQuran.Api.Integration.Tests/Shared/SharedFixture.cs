using MemQuran.Api.Integration.Tests.Shared.WireMock;
using WireMock.Server;

namespace MemQuran.Api.Integration.Tests.Shared;

// Created once for all tests of the same collection name
// because of Collection(nameof(MemQuranCollection)) attribute on the test classes

public class SharedFixture : IAsyncLifetime
{
    public WireMockServer WireMockServer { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        WireMockServer = WireMockSingleton.Instance;

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}