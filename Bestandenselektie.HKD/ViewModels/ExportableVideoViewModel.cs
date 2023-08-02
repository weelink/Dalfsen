using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableVideoViewModel : ExportableFileViewModel
    {
        public ExportableVideoViewModel(ExplorerViewModel parentViewModel, FileInfo file, ExporterViewModel exporter)
            : base(parentViewModel, file, exporter)
        {
        }
    }
}
