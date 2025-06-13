namespace MemQuran.Api.Contracts;

public interface IHashingService
{
    string ToHashString(string source);
}