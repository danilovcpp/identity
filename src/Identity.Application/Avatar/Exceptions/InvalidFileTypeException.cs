namespace Identity.Application.Avatar.Exceptions;

public sealed class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException()
        : base("Invalid file type. Only image files (JPEG, PNG, GIF, WebP) are allowed.")
    {
    }
}
