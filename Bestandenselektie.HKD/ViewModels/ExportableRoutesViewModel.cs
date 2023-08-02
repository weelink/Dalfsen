using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableRoutesViewModel : ExportableFileViewModel
    {
        public ExportableRoutesViewModel(ExplorerViewModel parentViewModel, FileInfo file, ExporterViewModel exporter)
            : base(parentViewModel, file, exporter)
        {
        }
    }
}
