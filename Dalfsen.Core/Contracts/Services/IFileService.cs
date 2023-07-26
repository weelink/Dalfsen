namespace Dalfsen.Core.Contracts.Services;

public interface IFileService
{
    T Read<T>(string folderPath, string fileName);

    void Save<T>(string folderPath, string fileName, T content);

    void Delete(string folderPath, string fileName);

    Task<IEnumerable<DriveInfo>> GetDrivesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(DirectoryInfo parent, CancellationToken cancellationToken);
    Task<IEnumerable<FileInfo>> GetImagesAsync(DirectoryInfo path, bool includeSubdirectories, CancellationToken cancellationToken);
}
