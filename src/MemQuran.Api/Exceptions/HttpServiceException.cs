using System.Net;

namespace MemQuran.Api.Exceptions;

[Serializable]
public class HttpServiceException : Exception
{
    public string ServiceName { get; }
    public HttpStatusCode HttpStatusCode { get; }
    public string ResponseContent { get; }

    public HttpServiceException(string serviceName, HttpRequestMessage httpRequestMessage, Exception exception, HttpStatusCode httpStatusCode)
        : base($"{serviceName} received {exception.GetType().Namespace} calling when calling endpoint {httpRequestMessage.RequestUri} : {exception.Message}")
    {
        ServiceName = serviceName;
        HttpStatusCode = httpStatusCode;
    }

    public HttpServiceException(string serviceName, HttpRequestMessage httpRequestMessage, HttpStatusCode httpStatusCode, string responseContent)
        : base($"{serviceName} received http status: {httpStatusCode} ({(int) httpStatusCode}) calling: {httpRequestMessage.RequestUri}")
    {
        ServiceName = serviceName;
        HttpStatusCode = httpStatusCode;
        ResponseContent = responseContent;
    }
}