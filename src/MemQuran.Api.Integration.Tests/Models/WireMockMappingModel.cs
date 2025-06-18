using Newtonsoft.Json;

namespace MemQuran.Api.Integration.Tests.Models;

public class WireMockMappingModel
{
    public Mappings[] Mappings { get; init; } = null!;
    public Meta? Meta { get; init; }
}

public class Mappings
{
    public string? Id { get; init; }
    public Request Request { get; init; } = null!;
    public Response Response { get; init; } = null!;
    public string Uuid { get; init; } = null!;
}

public class Request
{
    public string Method { get; init; } = null!;
    public string Url { get; init; } = null!;
    public string UrlPathTemplate { get; init; } = null!;
}

public class Response
{
    public int Status { get; init; }
    public Headers? Headers { get; init; }
}

public class Headers
{
    [JsonProperty("Content_Type")]
    public string? ContentType { get; init; }
}

public class Meta
{
    public int Total { get; init; }
}