using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableImageViewModel : ExportableFileViewModel
    {
        public ExportableImageViewModel(FileInfo file, ExporterViewModel exporter)
            : base(file, exporter)
        {
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

        public int Height { get; }
        public int Width { get; }

        public override string? Dimensions
        {
            get { return $"{Width} x {Height}"; }
        }
    }
}
