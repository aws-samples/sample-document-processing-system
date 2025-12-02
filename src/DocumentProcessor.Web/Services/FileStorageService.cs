namespace DocumentProcessor.Web.Services;

public class FileStorageService
{
    private readonly string _basePath = "uploads";

    public FileStorageService()
    {
        if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
    }

    public Task<Stream> GetDocumentAsync(string path)
    {
        var fullPath = Path.Combine(_basePath, path);
        return Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public async Task<string> SaveDocumentAsync(Stream stream, string fileName)
    {
        var uniqueName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(fileName)}";
        var relativePath = Path.Combine(DateTime.UtcNow.ToString("yyyy/MM/dd"), uniqueName);
        var fullPath = Path.Combine(_basePath, relativePath);
        var dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        using var fs = new FileStream(fullPath, FileMode.Create);
        await stream.CopyToAsync(fs);
        return relativePath;
    }
}
