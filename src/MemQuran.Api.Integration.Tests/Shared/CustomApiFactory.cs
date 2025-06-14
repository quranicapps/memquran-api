using MemQuran.Api.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace MemQuran.Api.Integration.Tests.Shared;

// Created per test file because of IClassFixture<CustomApiFactory>
// SharedFixture injected as singleton per test collection name

public class CustomApiFactory(SharedFixture sharedFixture) : WebApplicationFactory<Program>
{
    public ClientsSettings ClientsSettings { get; set; } = null!;
    public SharedFixture SharedFixture => sharedFixture;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("integration-test");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "override.json"))
            .Build();

        ClientsSettings = configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>()!;

        AddWireMockStubs();
        
        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder =>
                configurationBuilder
                    .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "override.json")));
    }

    private void AddWireMockStubs()
    {
        SharedFixture.Server
            .Given(Request.Create().WithPath($"/gh/quranstatic/static@{ClientsSettings.JsDelivrService.Version}/json/prophets/en_prophets.json").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404));
    }
}