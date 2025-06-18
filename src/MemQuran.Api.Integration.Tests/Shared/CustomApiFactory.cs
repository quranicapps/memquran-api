using MemQuran.Api.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

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

        // Initial data for configuration, this is to override the BaseUrl setting
        // from the integration test app.config, because we want to use the WireMock server
        // port which we have made dynamic from the docker TestContainer.
        KeyValuePair<string, string?>[] initialData = 
        [
            new KeyValuePair<string, string?>("Clients:JsDelivrService:BaseUrl", sharedFixture.WireMockBaseUrl),
        ];

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"))
            .AddInMemoryCollection(initialData)
            .Build();

        ClientsSettings = configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>()!;
        
        // Adding the appsettings again to override configuration after the program.cs has requested the API appsettings.
        // This is necessary to ensure that the settings are available in the test environment. (Work around)
        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
    }
}