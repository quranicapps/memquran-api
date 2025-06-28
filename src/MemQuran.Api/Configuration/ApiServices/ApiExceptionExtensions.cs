using System.Diagnostics;
using MemQuran.Api.Configuration.ApiServices;
using MemQuran.Api.Middleware;
using Microsoft.AspNetCore.Diagnostics;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ApiExceptionExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services, Action<ApiConfiguration> configuration)
    {
        var config = new ApiConfiguration();
        configuration(config);

        services.AddProblemDetails(opts => // built-in problem details support
            opts.CustomizeProblemDetails = ctx =>
            {
                if (!ctx.ProblemDetails.Extensions.ContainsKey("traceId"))
                {
                    var traceId = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
                    ctx.ProblemDetails.Extensions.Add(new KeyValuePair<string, object?>("traceId", traceId));
                }

                if (!ctx.ProblemDetails.Extensions.ContainsKey("instance"))
                {
                    ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
                }

                var exception = ctx.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

                if (ctx.ProblemDetails.Status != 500) return;

                ctx.ProblemDetails.Detail = "An error occurred in our API. Use the trace id when contacting us.";

                if (config.Environment.IsProduction()) return;

                if (!ctx.ProblemDetails.Extensions.ContainsKey("errorMessage"))
                {
                    ctx.ProblemDetails.Extensions.Add("errorMessage", exception?.Message);
                }

                if (!ctx.ProblemDetails.Extensions.ContainsKey("stackTrace"))
                {
                    ctx.ProblemDetails.Extensions.Add("stackTrace", exception?.StackTrace);
                }
            }
        );

        services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();
        services.AddProblemDetails();

        return services;
    }
}