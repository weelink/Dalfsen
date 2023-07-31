using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ExportableWordViewModel : ExportableFileViewModel
    {
        public ExportableWordViewModel(FileInfo file, ExporterViewModel exporter)
            : base(file, exporter)
        {
        }
    }
}
