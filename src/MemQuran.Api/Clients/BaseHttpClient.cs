using System.Net;
using System.Text.Json;
using MemQuran.Api.Exceptions;

namespace MemQuran.Api.Clients;

public abstract class BaseHttpClient(HttpClient httpClient, ILogger<BaseHttpClient> logger)
{
    protected abstract string ServiceName { get; }

    protected async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        return await GetAsync(request, cancellationToken);
    }
    
    protected async Task<HttpResponseMessage> GetAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            logger.LogInformation("{Name}.{Method}: CALLING: {Endpoint}", nameof(BaseHttpClient), nameof(SendAsync), request.RequestUri);
            response = await httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.InternalServerError);
        }
        catch (TaskCanceledException ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.InternalServerError);
        }

        return response;
    }
    
    protected async Task<string> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            logger.LogInformation("{Name}.{Method}: CALLING: {Endpoint}", nameof(BaseHttpClient), nameof(SendAsync), request.RequestUri);
            response = await httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.InternalServerError);
        }
        catch (TaskCanceledException ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.InternalServerError);
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return responseContent;
        }
        
        if(response.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning("Received 404 Not Found when calling endpoint {RequestUri}", request.RequestUri);
            return null; // Return empty string for 404 Not Found
        }

        logger.LogError("Received status code {StatusCode} ({StatusCodeInt}) when calling endpoint {RequestUri}", response.StatusCode, (int)response.StatusCode, request.RequestUri);


        throw new HttpServiceException(ServiceName, request, response.StatusCode, responseContent);
    }

    protected async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var responseContent = await SendAsync(request, cancellationToken);
        
        return JsonSerializer.Deserialize<T>(responseContent);
    }

    protected async Task<byte[]> GetBytesAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            logger.LogInformation("{Name}.{Method}: CALLING: {Endpoint}", nameof(BaseHttpClient), nameof(SendAsync), request.RequestUri);
            response = await httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.InternalServerError);
        }
        catch (TaskCanceledException ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            throw new HttpServiceException(ServiceName, request, ex, HttpStatusCode.InternalServerError);
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            using var content = response.Content;
            return await content.ReadAsByteArrayAsync(cancellationToken);
        }

        logger.LogError("Received status code {StatusCode} ({StatusCodeInt}) when calling endpoint {RequestUri}", response.StatusCode, (int)response.StatusCode, request.RequestUri);
        
        throw new HttpServiceException(ServiceName, request, response.StatusCode, responseContent);
    }
}