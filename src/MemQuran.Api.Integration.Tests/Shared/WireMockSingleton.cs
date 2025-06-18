using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace MemQuran.Api.Integration.Tests.Shared;

public static class WireMockSingleton
{
    private static readonly Lazy<WireMockServer> LazyInstance = new(() =>
    {
        var server = WireMockServer.Start(8383);

        AddWireMockStubs(server);

        return server;
    });

    private static void AddWireMockStubs(WireMockServer server)
    {
        const string baseUrlPath = "gh/quranstatic/static";
        const string versionFromOverrideJson = "AnyVersionForTestingPurposes";

        server.Given(Request.Create().WithPath($"/{baseUrlPath}@{versionFromOverrideJson}/json/prophets/en_prophets.json").UsingGet()).RespondWith(Response.Create().WithStatusCode(404));
    }

    public static WireMockServer Instance => LazyInstance.Value;
}