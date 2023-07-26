﻿using System.Text;
using Dalfsen.Core.Contracts.Services;

using Newtonsoft.Json;

namespace Dalfsen.Core.Services;

public class FileService : IFileService
{
    public T Read<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        return default;
    }

    public void Save<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }

    public Task<IEnumerable<DriveInfo>> GetDrivesAsync(CancellationToken cancellationToken)
    {
        IEnumerable<DriveInfo> drives = DriveInfo.GetDrives().Where(drive => drive.IsReady).ToList();

        return Task.FromResult(drives);
    }

    public Task<IEnumerable<DirectoryInfo>> GetDirectoriesAsync(DirectoryInfo parent, CancellationToken cancellationToken)
    {
        var enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = false };
        IEnumerable<DirectoryInfo> directories = parent.EnumerateDirectories("*", enumerationOptions).ToList();

        return Task.FromResult(directories);
    }

    public Task<IEnumerable<FileInfo>> GetImagesAsync(DirectoryInfo path, bool includeSubdirectories, CancellationToken cancellationToken)
    {
        IEnumerable<FileInfo> files = path.EnumerateFiles("*", new EnumerationOptions
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = includeSubdirectories
        });

        return Task.FromResult(files);
    }
}
