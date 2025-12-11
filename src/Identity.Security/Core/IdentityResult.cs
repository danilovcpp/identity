namespace Identity.Security.Core;

public class IdentityResult
{
    public bool Succeeded { get; }
    public string[] Errors { get; }

    private IdentityResult(bool succeeded, string[] errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public static IdentityResult Success() => new(true, []);
    public static IdentityResult Failed(params string[] errors) => new(false, errors);
}
