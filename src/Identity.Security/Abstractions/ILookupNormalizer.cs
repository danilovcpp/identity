namespace Identity.Security.Abstractions;

public interface ILookupNormalizer
{
    string Normalize(string key);
}
