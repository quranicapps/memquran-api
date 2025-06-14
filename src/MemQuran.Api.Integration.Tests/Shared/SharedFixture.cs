using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace MemQuran.Api.Integration.Tests.Shared;

// Created once for all tests of the same collection name
// because of Collection(nameof(MemQuranCollection)) attribute on the test classes

public class SharedFixture : IAsyncLifetime
{
    public WireMockServer Server { get; set; } = null!;

    public async Task InitializeAsync()
    {
        Server = WireMockServer.Start(8383);

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Server.Stop();    
        Server.Dispose();
        
        await Task.CompletedTask;
    }
}