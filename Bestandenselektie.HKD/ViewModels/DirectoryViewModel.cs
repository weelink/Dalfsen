using Bestandenselektie.HKD.Services;
using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class DirectoryViewModel : ExplorerViewModel
    {
        public DirectoryViewModel(DirectoryInfo directory, Settings settings)
            : base(directory, settings)
        {
        }
    }
}
