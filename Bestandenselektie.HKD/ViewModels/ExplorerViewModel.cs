using Bestandenselektie.HKD.Collections;
using Bestandenselektie.HKD.Commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Bestandenselektie.HKD.ViewModels
{
    public abstract class ExplorerViewModel : ViewModel
    {
        private bool isExpanded;

        protected ExplorerViewModel(DirectoryInfo directory)
        {
            Directory = directory;
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
        public DirectoryInfo Directory { get; }

        private void LoadDirectories()
        {
            Directories.Clear();

            IEnumerable<DirectoryInfo> directories = Directory.EnumerateDirectories("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false
            });

            IEnumerable<DirectoryViewModel> directoryViewModels = directories.Select(directory => new DirectoryViewModel(directory)).ToList();

            Directories.AddRange(directoryViewModels);
        }
    }
}
