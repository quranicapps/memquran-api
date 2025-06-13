using System.Security.Cryptography;
using System.Text;
using MemQuran.Api.Contracts;

namespace MemQuran.Api.Services;

public class HashingService : IHashingService
{
    public string ToHashString(string source)
    {
        var sb = new StringBuilder();
        foreach (var b in SHA256.HashData(Encoding.UTF8.GetBytes(source)))
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}