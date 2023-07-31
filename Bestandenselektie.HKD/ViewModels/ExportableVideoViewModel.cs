using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableVideoViewModel : ExportableFileViewModel
    {
        public ExportableVideoViewModel(FileInfo file, ExporterViewModel exporter)
            : base(file, exporter)
        {
        }
    }
}
