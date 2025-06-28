using MemQuran.Api.Configuration.ApiServices;
using Microsoft.OpenApi.Models;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ApiOpenApiExtensions
{
    public static IServiceCollection AddOpenApiServices(this IServiceCollection services, Action<ApiConfiguration> configuration)
    {
        var config = new ApiConfiguration();
        configuration(config);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddOpenApi("memquranapi", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "MemQuran API";
                document.Info.Version = "1.0.2";
                document.Info.Description = "An API for getting Quran data from MemQuran project. " +
                                            "This API provides access to Quran text, translations, audio files, and more. " +
                                            "It is designed to be used by developers who want to integrate Quran data into their applications.";
                document.Info.Contact = new OpenApiContact
                {
                    Name = "MemQuran Team",
                    Email = "my email"
                };

                return Task.CompletedTask;
            });

            options.AddSchemaTransformer((schema, context, cancellationToken) =>
            {
                if (schema.Type == "decimal")
                {
                    schema.Format = "decimal";
                }

                return Task.CompletedTask;
            });
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}