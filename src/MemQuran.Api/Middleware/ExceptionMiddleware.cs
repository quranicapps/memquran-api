using System.Net;
using MemQuran.Api.Exceptions;
using MemQuran.Api.Extensions;
using MemQuran.Api.Models;

namespace MemQuran.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerFactory _loggerFactory;

    public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerFactory = loggerFactory;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (HttpServiceException ex)
        {
            var reference = Guid.NewGuid().ToString();
            var resultError = new ResultError((int)HttpStatusCode.InternalServerError, "An error has occurred, please try again.", reference)
            {
                InnerErrors = ex.ResponseContent.TryParseJsonString<ResponseResult>(out var responseResult) 
                    ? new List<ResultError>(responseResult.Errors ?? Array.Empty<ResultError>()) 
                    : new List<ResultError> { new ResultError((int)ex.HttpStatusCode, $"Http service to service error: {ex.Message}", reference) }
            };

            await HandleException(httpContext, ex, resultError);
        }
        catch (Exception ex)
        {
            var resultError = new ResultError((int)HttpStatusCode.InternalServerError, "An error has occurred, please try again.", reference: Guid.NewGuid().ToString());

            await HandleException(httpContext, ex, resultError);
        }
    }

    private async Task HandleException(HttpContext httpContext, Exception ex, ResultError resultError)
    {
        var logger = _loggerFactory.CreateLogger(GetType());
        logger.LogError(ex, "Reference: {Reference}. {Message}", resultError.Reference, ex.Message);

        if (httpContext.Response.HasStarted == false)
        {
            if (httpContext.Request.QueryString.Value != null && httpContext.Request.QueryString.Value.Contains("show-developer-exception"))
            {
                throw ex;
            }

            await httpContext.Response .RespondAsync(HttpStatusCode.InternalServerError, resultError);
        }
    }
}