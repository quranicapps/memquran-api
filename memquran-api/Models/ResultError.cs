public class ResultError
{
    public ResultError(int statusCode, string message, string? reference = null)
    {
        Code = Convert.ToInt32(statusCode);
        Message = message;
        Reference = reference;
    }

    public int Code { get; set; }
    public string Message { get; set; }
    public string? Reference { get; set; }
    public IEnumerable<ResultError> InnerErrors { get; set; }
}
