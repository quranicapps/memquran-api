using MemQuran.Api.Settings;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace MemQuran.Api.Integration.Tests.Shared;

// Created once for all tests of the same collection name
// because of Collection(nameof(MemQuranCollection)) attribute on the test classes

public class SharedFixture : IAsyncLifetime
{
    public WireMockServer Server { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Server = WireMockSingleton.Instance;

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}