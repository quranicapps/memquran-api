using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public static class HttpResponseExtensions
    {
        public static Task RespondAsync(this HttpResponse httpResponse, HttpStatusCode httpStatusCode, ResultError resultError) => httpResponse.RespondAsync(httpStatusCode, new[] { resultError });

        public static Task RespondAsync(this HttpResponse httpResponse, HttpStatusCode httpStatusCode, IReadOnlyCollection<ResultError> resultErrors)
        {
            var result = new ResponseResult
            {
                Errors = resultErrors
            };

            return httpResponse.RespondAsync(httpStatusCode, result);
        }


        public static async Task RespondAsync(this HttpResponse httpResponse, HttpStatusCode httpStatusCode, ResponseResult responseResult)
        {
            var contentType = httpResponse.HttpContext.Request.ContentType;

            while (true)
            {
                string stringResult;
                switch (contentType)
                {
                    case MediaTypeNames.Application.Json:
                        {
                            httpResponse.ContentType = MediaTypeNames.Application.Json;
                            var jsonOptions = httpResponse.HttpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>();
                            stringResult = JsonSerializer.Serialize(responseResult, jsonOptions.Value.JsonSerializerOptions);

                            break;
                        }
                    case MediaTypeNames.Application.Xml:
                        {
                            httpResponse.ContentType = MediaTypeNames.Application.Xml;
                            //todo - The settings here need to influence the serialisation
                            // mvcXmlOptions = httpResponse.HttpContext.RequestServices.GetService<MvcXmlOptions>();
                            var serialiser = new XmlSerializer(typeof(ResponseResult));

                            await using var stringWriter = new StringWriter();
                            using var xmlWriter = XmlWriter.Create(stringWriter);
                            serialiser.Serialize(xmlWriter, responseResult);
                            stringResult = stringWriter.ToString(); // Your XML

                            break;
                        }
                    default:
                        contentType = MediaTypeNames.Application.Json;
                        continue;
                }

                httpResponse.StatusCode = (int)httpStatusCode;
                await httpResponse.WriteAsync(stringResult);
                return;
            }
        }
    }

