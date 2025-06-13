using Newtonsoft.Json;

namespace MemQuran.Infrastructure.Extensions;

public static class JsonExtensions
{
    public static bool TryParseJsonString<T>(this string? json, out T @object)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            @object = default;
            return false;
        }
            
        try
        {
            @object = JsonConvert.DeserializeObject<T>(json);
            return true;
        }
        catch
        {
            @object = default;
            return false;
        }
    }
}