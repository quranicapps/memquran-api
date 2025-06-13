namespace MemQuran.Core.Contracts;

public interface IHashingService
{
    string ToHashString(string source);
}