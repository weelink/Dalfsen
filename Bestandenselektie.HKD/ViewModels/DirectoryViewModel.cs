using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class DirectoryViewModel : ExplorerViewModel
    {
        public DirectoryViewModel(DirectoryInfo directory)
            : base(directory)
        {
        }
    }
}
