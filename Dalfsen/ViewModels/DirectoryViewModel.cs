using System.IO;

namespace Dalfsen.ViewModels
{
    public class DirectoryViewModel : ExplorerViewModel
    {
        public DirectoryViewModel(DirectoryInfo directory)
            : base(directory)
        {
        }
    }
}
