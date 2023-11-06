using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportablePdfViewModel : ExportableFileViewModel
    {
        public ExportablePdfViewModel(ExplorerViewModel parentViewModel, FileInfo file, ExporterViewModel exporter, ReferenceData referenceData)
            : base(parentViewModel, file, exporter, referenceData)
        {
        }
    }
}
