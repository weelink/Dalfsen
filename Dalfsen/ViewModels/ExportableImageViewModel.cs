using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace Dalfsen.ViewModels
{
    public class ExportableImageViewModel : ViewModel
    {
        private readonly FileInfo file;
        private readonly ExporterViewModel exporter;
        private bool isSelected;

        public ExportableImageViewModel(DirectoryInfo directory, FileInfo file, ExporterViewModel exporter)
        {
            this.file = file;
            this.exporter = exporter;
            Name = Path.GetRelativePath(directory.FullName, file.FullName);
            FullPath = file.FullName!;
            isSelected = exporter.IsSelected(this);

            PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(IsSelected))
                {
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
            catch (System.Exception)
            {
                Debug.WriteLine(FullPath);
                throw;
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }

        public string FullPath { get; }
        public string Name { get; }
        public int Height { get; }
        public int Width { get; }
        public string Dimensions
        {
            get { return $"{Width} x {Height}"; }
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
