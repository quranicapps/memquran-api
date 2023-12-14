using System.Net;
using System.Text.Json;

public abstract class BaseHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BaseHttpClient> _logger;

    protected BaseHttpClient(HttpClient httpClient, ILogger<BaseHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    protected abstract string ServiceName { get; }

    protected async Task<string> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        try
        {
            _logger.LogInformation("{Name}.{Method}: CALLING: {Endpoint}", nameof(BaseHttpClient), nameof(SendAsync), request.RequestUri);
            response = await _httpClient.SendAsync(request, cancellationToken);
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

        _logger.LogError("Received status code {StatusCode} ({StatusCodeInt}) when calling endpoint {RequestUri}", response.StatusCode, (int)response.StatusCode, request.RequestUri);


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
            _logger.LogInformation("{Name}.{Method}: CALLING: {Endpoint}", nameof(BaseHttpClient), nameof(SendAsync), request.RequestUri);
            response = await _httpClient.SendAsync(request, cancellationToken);
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

        _logger.LogError("Received status code {StatusCode} ({StatusCodeInt}) when calling endpoint {RequestUri}", response.StatusCode, (int)response.StatusCode, request.RequestUri);
        
        throw new HttpServiceException(ServiceName, request, response.StatusCode, responseContent);
    }
}