namespace QuranApi.Contracts;

public interface IHashingService
{
    string ToHashString(string source);
}