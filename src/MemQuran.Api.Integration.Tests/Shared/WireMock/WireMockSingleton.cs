using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace MemQuran.Api.Integration.Tests.Shared.WireMock;

public class WireMockSingleton : IAsyncDisposable
{
    private static IContainer _container = null!;
    
    private static readonly Lazy<Task<string>> LazyInstance = new(async () =>
    {
        var wiremockConfigFolder = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Shared", "WireMock", "wiremock.org.stubs");

        _container = new ContainerBuilder()
            .WithImage("wiremock/wiremock:3.13.1")
            .WithName($"wiremock.org-{Guid.NewGuid()}")
            .WithPortBinding(8080, true)
            .WithBindMount(wiremockConfigFolder, "/home/wiremock")
            .Build();

        await _container.StartAsync();
        await Task.Delay(600);
        
        return $"http://127.0.0.1:{_container.GetMappedPublicPort(8080)}";
    });

    public static Task<string> Instance => LazyInstance.Value;

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}