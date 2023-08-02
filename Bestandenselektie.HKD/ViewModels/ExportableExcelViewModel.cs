using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableExcelViewModel : ExportableFileViewModel
    {
        public ExportableExcelViewModel(ExplorerViewModel parentViewModel, FileInfo file, ExporterViewModel exporter)
            : base(parentViewModel, file, exporter)
        {
        }
    }
}
