using Dalfsen.Collections;
using Dalfsen.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dalfsen.ViewModels
{
    public abstract class ExplorerViewModel : ViewModel
    {
        private readonly DirectoryInfo directory;

        private bool isExpanded;

        protected ExplorerViewModel(DirectoryInfo directory)
        {
            this.directory = directory;
            Name = directory.Name;
            Directories = new SmartCollection<DirectoryViewModel>(new[] { default(DirectoryViewModel)! });
            LoadDirectoriesCommand = new DelegateCommand(() => LoadDirectories());
        }

        public string Name { get; }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }

        public SmartCollection<DirectoryViewModel> Directories { get; }
        public ICommand LoadDirectoriesCommand { get; }

        private void LoadDirectories()
        {
            Directories.Clear();

            IEnumerable<DirectoryInfo> directories = directory.EnumerateDirectories("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false
            });

            IEnumerable<DirectoryViewModel> directoryViewModels = directories.Select(directory => new DirectoryViewModel(directory)).ToList();

            Directories.AddRange(directoryViewModels);
        }
    }
}
