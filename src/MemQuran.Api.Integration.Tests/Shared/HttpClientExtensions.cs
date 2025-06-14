using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests.Shared;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonPrintOptions = new() { WriteIndented = true };

    private static readonly JsonSerializerOptions JsonDeserializeOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task<T> GetJsonResultAsync<T>(this HttpClient client, string url,
        HttpStatusCode expectedStatus, ITestOutputHelper output)
    {
        var response = await client.GetAsync(url);
        return await DeserializeAndCheckResponse<T>(response, expectedStatus, output);
    }

    public static async Task<T> PostForJsonResultAsync<T>(this HttpClient client, string url,
               object content, HttpStatusCode expectedStatus, ITestOutputHelper output)
    {
        var response = await client.PostAsJsonAsync(url, content);
        return await DeserializeAndCheckResponse<T>(response, expectedStatus, output);
    }

    private static async Task<T> DeserializeAndCheckResponse<T>(HttpResponseMessage response,
        HttpStatusCode expectedStatus, ITestOutputHelper output)
    {
        var stringContent = await response.Content.ReadAsStringAsync();
        try
        {
            var result = string.IsNullOrEmpty(stringContent) 
                ? Activator.CreateInstance<T>()
                : JsonSerializer.Deserialize<T>(stringContent, JsonDeserializeOptions);
            Assert.Equal(expectedStatus, response.StatusCode);
            Assert.NotNull(result);
            return result;
        }
        catch (Exception)
        {
            WriteOutput(stringContent, output);
            throw;
        }
    }

    private static void WriteOutput(string stringContent, ITestOutputHelper output)
    {
        string? outputText;
        try
        {
            var jsonContent = JsonDocument.Parse(stringContent);
            outputText = JsonSerializer.Serialize(jsonContent, JsonPrintOptions);
        }
        catch
        {
            outputText = stringContent;
        }
        output.WriteLine(outputText);
    }
}
