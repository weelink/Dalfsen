using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableExcelViewModel : ExportableFileViewModel
    {
        public ExportableExcelViewModel(FileInfo file, ExporterViewModel exporter)
            : base(file, exporter)
        {
        }
    }
}
