using MemQuran.Api.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace MemQuran.Api.Integration.Tests.Shared;

// Created per test file because of IClassFixture<CustomApiFactory>
// SharedFixture injected as singleton per test collection name

public class CustomApiFactory : WebApplicationFactory<Program>
{
    public CustomApiFactory(SharedFixture sharedFixture)
    {
        SharedFixture = sharedFixture;
        Client = CreateClient();
    }

    public ClientsSettings ClientsSettings { get; private set; } = null!;
    public SharedFixture SharedFixture { get; }
    public HttpClient Client { get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("integration-test");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"))
            .AddInMemoryCollection((KeyValuePair<string, string?>[])
            [
                new KeyValuePair<string, string?>("Clients:JsDelivrService:Version", "AnyVersionForTestingPurposes"),
            ])
            .Build();

        ClientsSettings = configuration.GetSection(ClientsSettings.SectionName).Get<ClientsSettings>()!;
        
        // Adding the appsettings again to override configuration after the program.cs has requested the API appsettings.
        // This is necessary to ensure that the settings are available in the test environment. (Work around)
        // These appsettings are only available for the WebApplicationFactory, not for the tests.
        // Only using AddInMemoryCollection, can we get the test appsettings in the tests,
        // like we are doing for ClientsSettings, it will not get the settings from the test appsettings.json
        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder => configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));

        builder.ConfigureTestServices(services =>
        {
            // Add services to override the ones in the API 
        });
    }
}