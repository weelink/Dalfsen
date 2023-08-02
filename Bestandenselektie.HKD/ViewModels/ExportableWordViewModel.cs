using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableWordViewModel : ExportableFileViewModel
    {
        public ExportableWordViewModel(ExplorerViewModel parentViewModel, FileInfo file, ExporterViewModel exporter)
            : base(parentViewModel, file, exporter)
        {
        }
    }
}
