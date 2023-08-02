using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportablePdfViewModel : ExportableFileViewModel
    {
        public ExportablePdfViewModel(ExplorerViewModel parentViewModel, FileInfo file, ExporterViewModel exporter)
            : base(parentViewModel, file, exporter)
        {
        }
    }
}
