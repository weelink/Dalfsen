using System.IO;
using System.Collections.Generic;

namespace Bestandenselektie.HKD.Services
{
    public class Settings
    {
        public Settings()
        {
            ProcessedDirectories = new HashSet<string>();
            ConflictResolutions = new List<bool> { true, false, false };
        }

        public void MarkAsProcessed(DirectoryInfo directory)
        {
            ProcessedDirectories.Add(directory.FullName.ToLowerInvariant());
        }

        public void MarkAsProcessed(IEnumerable<DirectoryInfo> directories)
        {
            foreach (var directory in directories)
            {
                MarkAsProcessed(directory);
            }
        }

        public bool HasBeenProcessed(DirectoryInfo directory)
        {
            return ProcessedDirectories.Contains(directory.FullName.ToLowerInvariant());
        }

        public List<bool> ConflictResolutions { get; set; }
        public string? ExportDirectory { get; set; }
        public HashSet<string> ProcessedDirectories { get; set; }
        public bool ExportToExcel { get; set; }
        public bool MarkDirectoriesAsProcessed { get; set; }
        public string? ExcelFilename { get; set; }
    }
}
