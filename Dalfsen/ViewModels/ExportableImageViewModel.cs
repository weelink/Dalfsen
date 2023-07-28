using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace Dalfsen.ViewModels
{
    public class ExportableImageViewModel : ViewModel
    {
        private readonly FileInfo file;
        private bool selected;

        public ExportableImageViewModel(DirectoryInfo directory, FileInfo file)
        {
            this.file = file;

            Name = Path.GetRelativePath(directory.FullName, file.FullName);
            FullPath = file.FullName!;

            try
            {
                if (file.Extension.ToLowerInvariant() == ".svg")
                {
                    return;
                }

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
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }

        public string FullPath { get; }
        public string Name { get; }
        public int Height { get; }
        public int Width { get; }
        public string Dimensions
        {
            get { return $"{Width} x ${Height}"; }
        }
    }
}
