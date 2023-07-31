using Bestandenselektie.HKD.Extensions;
using System;
using System.Globalization;
using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public abstract class ExportableFileViewModel : ViewModel
    {
        private static readonly CultureInfo DutchCulture = new CultureInfo("nl-NL");

        private readonly FileInfo file;
        private readonly ExporterViewModel exporter;
        private bool isSelected;
        private bool shouldExport;

        protected ExportableFileViewModel(FileInfo file, ExporterViewModel exporter)
        {
            this.file = file;
            this.exporter = exporter;
            Name = file.Name;
            Extension = file.Extension;
            FullPath = file.FullName!;
            Directory = file.Directory!.FullName!;
            isSelected = exporter.IsSelected(this);
            Created = string.Empty;
            Modified = string.Empty;
            CreatedAsDate = DateTime.Today;
            ModifiedAsDate = DateTime.Today;

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
                CreatedAsDate = file.CreationTime;
                ModifiedAsDate = file.LastWriteTime;
                Created = CreatedAsDate.ToString(DutchCulture.DateTimeFormat);
                Modified = ModifiedAsDate.ToString(DutchCulture.DateTimeFormat);
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

        public string Directory { get; }

        public string FullPath { get; }
        public string Name { get; }
        public string Extension { get; }
        public string Modified { get; }
        public string Created { get; }
        public DateTime ModifiedAsDate { get; }
        public DateTime CreatedAsDate { get; }
        public virtual string? Dimensions { get; }

        public string Size
        {
            get
            {
                return file.Length.ToSize();
            }
        }

        public long SizeInBytes
        {
            get { return file.Length; }
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
            if (obj is ExportableFileViewModel that)
            {
                return file.FullName.Equals(that.file.FullName, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
    }
}
