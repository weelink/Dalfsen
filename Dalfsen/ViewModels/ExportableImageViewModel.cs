using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Dalfsen.ViewModels
{
    public class ExportableImageViewModel : ViewModel
    {
        private readonly FileInfo file;
        private readonly ExporterViewModel exporter;
        private bool isSelected;
        private bool shouldExport;

        public ExportableImageViewModel(DirectoryInfo directory, FileInfo file, ExporterViewModel exporter)
        {
            this.file = file;
            this.exporter = exporter;
            Name = Path.GetRelativePath(directory.FullName, file.FullName);
            FullPath = file.FullName!;
            Directory = file.Directory!.FullName!;
            isSelected = exporter.IsSelected(this);

            PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(IsSelected))
                {
                    ShouldExport = IsSelected;

                    if (IsSelected)
                    {
                        exporter.Select(this);
                    }
                    else
                    {
                        exporter.Deselect(this);
                    }
                }
            };

            try
            {
                using (FileStream imageStream = File.OpenRead(FullPath))
                {
                    BitmapDecoder decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
                    Height = decoder.Frames[0].PixelHeight;
                    Width = decoder.Frames[0].PixelWidth;
                }

            }
            catch (Exception)
            {
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }

        public bool ShouldExport
        {
            get { return shouldExport; }
            set { SetProperty(ref shouldExport, value); }
        }

        public string Directory { get;  }

        public string FullPath { get; }
        public string Name { get; }
        public int Height { get; }
        public int Width { get; }
        public string Dimensions
        {
            get { return $"{Width} x {Height}"; }
        }

        public void CopyTo(string targetDirectory)
        {
            var target = DetermineTarget(targetDirectory);
            if (target == null)
            {
                return;
            }

            try
            {
                File.Copy(FullPath, target, true);
            }
            catch (Exception)
            {
            }
        }

        private string? DetermineTarget(string targetDirectory)
        {
            string target = Path.Combine(targetDirectory, file.Name);

            if (File.Exists(target))
            {
                if (exporter.ConflictSolutions[0])
                {
                    return EnsureUniqueFileName(targetDirectory);
                }
                if (exporter.ConflictSolutions[1])
                {
                    return target;
                }
                if (exporter.ConflictSolutions[2])
                {
                    return null;
                }
            }

            return target;
        }

        private string EnsureUniqueFileName(string targetDirectory)
        {
            var filename = Path.GetFileNameWithoutExtension(file.Name);
            var extension = Path.GetExtension(file.Name);

            int i = 1;
            while (File.Exists(Path.Combine(targetDirectory, $"{filename}_{i}{extension}")))
            {
                i++;
            }

            return Path.Combine(targetDirectory, $"{filename}_{i}{extension}");
        }

        public override int GetHashCode()
        {
            return file.FullName.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is ExportableImageViewModel that)
            {
                return file.FullName.Equals(that.file.FullName, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
    }
}
