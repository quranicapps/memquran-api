namespace MemQuran.Core.Models;

public class ResponseResult
{
    public IReadOnlyCollection<ResultError>? Errors { get; set; }
    public object? Data { get; set; }
    public object? Meta { get; set; }
}