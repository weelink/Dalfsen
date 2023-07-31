using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportablePdfViewModel : ExportableFileViewModel
    {
        public ExportablePdfViewModel(FileInfo file, ExporterViewModel exporter)
            : base(file, exporter)
        {
        }
    }
}
