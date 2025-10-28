namespace Identity.Application.Abstractions;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);

    string GetFileUrl(string fileName);
}
