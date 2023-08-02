using Bestandenselektie.HKD.Services;
using System.Collections.Concurrent;
using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class DirectoryViewModel : ExplorerViewModel
    {
        private static readonly ConcurrentDictionary<string, DirectoryViewModel> LoadedDirectories = new ConcurrentDictionary<string, DirectoryViewModel>();

        private DirectoryViewModel(DirectoryInfo directory, Settings settings)
            : base(directory, settings)
        {
        }

        public static DirectoryViewModel Get(DirectoryInfo directory, Settings settings)
        {
            return LoadedDirectories.GetOrAdd(directory.FullName, _ => new DirectoryViewModel(directory, settings));
        }
    }
}
