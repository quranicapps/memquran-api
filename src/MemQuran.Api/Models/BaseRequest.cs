namespace MemQuran.Api.Models;

public class BaseRequest
{
    public string? SourceIpAddress { get; set; }
    public string? TraceId { get; set; }
    public string? TraceState { get; set; }
}