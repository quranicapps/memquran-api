using MemQuran.Api.Clients.BetterStack;
using MemQuran.Core.Contracts;
using MemQuran.Core.Messaging;
using MemQuran.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static void AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // Configuration
        var seqConfigurationSection = builder.Configuration.GetSection(SeqSettings.SectionName);
        var seqSettings = seqConfigurationSection.Get<SeqSettings>();
        if (seqSettings == null) throw new InvalidOperationException($"{nameof(SeqSettings)} is not configured. Please check your appsettings.json or environment variables.");

        var jaegerSettings = builder.Configuration.GetSection(JaegerSettings.SectionName).Get<JaegerSettings>();
        if (jaegerSettings == null) throw new InvalidOperationException($"{nameof(JaegerSettings)} is not configured. Please check your appsettings.json or environment variables.");

        var betterStackSettings = builder.Configuration.GetSection(BetterStackSettings.SectionName).Get<BetterStackSettings>();
        if (betterStackSettings == null) throw new InvalidOperationException($"{nameof(BetterStackSettings)} is not configured. Please check your appsettings.json or environment variables.");

        // Logging
        builder.Logging
            .AddOpenTelemetry(options => options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName)))
            .AddSimpleConsole()
            .AddSeq(seqConfigurationSection);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(tracing => tracing
                    // .AddHttpClientInstrumentation() //OpenTelemetry.Instrumentation.Http
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddAspNetCoreInstrumentation(x =>
                    {
                        // Exclude health check requests from tracing
                        x.Filter = context => !context.Request.Path.StartsWithSegments("/api/health");
                    })
                    .AddOtlpExporter(opt =>
                    {
                        // Aspire
                        opt.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? throw new InvalidOperationException("OTEL_EXPORTER_OTLP_ENDPOINT is not configured. Please check your appsettings.json or environment variables or Aspire startup."));

                        // Jaeger
                        // opt.Endpoint = new Uri(jaegerSettings.Endpoint);

                        // Seq
                        // opt.Endpoint = new Uri(seqSettings.OpenTelemetryTraceIngestUrl);
                        // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                        // if (!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";

                        //BetterStack
                        // opt.Endpoint = new Uri($"{betterStackSettings.IngestBaseUrl}/v1/traces");
                        // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                        // opt.Headers = $"Authorization=Bearer {betterStackSettings.BearerToken}";
                    })
                    .AddSource(nameof(EvictCacheItemMessageV1)) // Should be the name of any activities used in code
                    .AddSource(nameof(EvictCacheAllMessageV1)) // Should be the name of any activities used in code
                // .AddConsoleExporter()
            )
            .WithMetrics(metrics => metrics
                    // .AddHttpClientInstrumentation() //OpenTelemetry.Instrumentation.Http
                    // metrics.AddAspNetCoreInstrumentation()
                    //     .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        // Aspire
                        opt.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? throw new InvalidOperationException("OTEL_EXPORTER_OTLP_ENDPOINT is not configured. Please check your appsettings.json or environment variables or Aspire startup."));

                        // Jaeger
                        // opt.Endpoint = new Uri(jaegerSettings.Endpoint);

                        // Seq
                        // opt.Endpoint = new Uri(seqSettings.OpenTelemetryTraceIngestUrl);
                        // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                        // if(!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";

                        //BetterStack
                        // opt.Endpoint = new Uri($"{betterStackSettings.IngestBaseUrl}/v1/metrics");
                        // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                        // opt.Headers = $"Authorization=Bearer {betterStackSettings.BearerToken}";
                    })
                // .AddConsoleExporter()
            )
            .WithLogging(logging =>
            {
                logging.AddOtlpExporter(opt =>
                {
                    // Aspire
                    opt.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? throw new InvalidOperationException("OTEL_EXPORTER_OTLP_ENDPOINT is not configured. Please check your appsettings.json or environment variables or Aspire startup."));

                    // Jaeger
                    // opt.Endpoint = new Uri(jaegerSettings.Endpoint);

                    // Seq
                    // opt.Endpoint = new Uri(seqSettings.OpenTelemetryLogIngestUrl);
                    // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                    // if (!string.IsNullOrWhiteSpace(seqSettings.ApiKey)) opt.Headers = $"X-Seq-ApiKey={seqSettings.ApiKey}";

                    //BetterStack
                    // opt.Endpoint = new Uri($"{betterStackSettings.IngestBaseUrl}/v1/logs");
                    // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                    // opt.Headers = $"Authorization=Bearer {betterStackSettings.BearerToken}";
                });
            });

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        builder.Services.AddHttpClient<ITelemetryClient, BetterStackTelemetryClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(betterStackSettings.IngestBaseUrl);
                httpClient.Timeout = betterStackSettings.DefaultTimeout;
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {betterStackSettings.BearerToken}");
            })
            .AddHttpMessageHandler(() => new BetterStackDelegatingHandler());

        // Service Discovery
        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // 1. Add Service Discovery (so you can use "http://apiservice" names)
            http.AddServiceDiscovery();

            // 2. Add your custom resilience logic here
            http.AddStandardResilienceHandler();
            // http.AddResilienceHandler("my-global-strategy", pipeline =>
            // {
            //     pipeline.AddRetry(new HttpRetryStrategyOptions
            //     {
            //         MaxRetryAttempts = 3,
            //         Delay = TimeSpan.FromSeconds(2),
            //         BackoffType = DelayBackoffType.Exponential
            //     });
            //
            //     pipeline.AddTimeout(TimeSpan.FromSeconds(10));
            // });
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });
    }
}