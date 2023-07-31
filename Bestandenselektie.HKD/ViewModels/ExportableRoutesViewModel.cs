using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableRoutesViewModel : ExportableFileViewModel
    {
        public ExportableRoutesViewModel(FileInfo file, ExporterViewModel exporter)
            : base(file, exporter)
        {
        }
    }
}
