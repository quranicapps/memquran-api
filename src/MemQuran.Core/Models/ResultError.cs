namespace MemQuran.Core.Models;

public class ResultError(int statusCode, string message, string? reference = null)
{
    public int Code { get; set; } = Convert.ToInt32(statusCode);
    public string Message { get; set; } = message;
    public string? Reference { get; set; } = reference;
    public IEnumerable<ResultError> InnerErrors { get; set; } = null!;
}