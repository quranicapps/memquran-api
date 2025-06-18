using WireMock.Server;

namespace MemQuran.Api.Integration.Tests.Shared.WireMock;

public class WireMockSingleton : IDisposable
{
    private static readonly Lazy<WireMockServer> LazyInstance = new(() => WireMockServer.Start(8080));

    public static WireMockServer Instance => LazyInstance.Value;
    
    public void Dispose()
    {
        if(Instance.IsStarted) Instance.Stop();
        Instance.Dispose();
        GC.SuppressFinalize(this);
    }
}