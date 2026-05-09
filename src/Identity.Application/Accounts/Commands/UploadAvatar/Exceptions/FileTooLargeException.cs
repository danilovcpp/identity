namespace Identity.Application.Accounts.Commands.UploadAvatar.Exceptions;

public sealed class FileTooLargeException : Exception
{
    public FileTooLargeException(long maxSizeInMb)
        : base($"File size exceeds the maximum allowed size of {maxSizeInMb} MB.")
    {
    }
}
