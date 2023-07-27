using Dalfsen.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dalfsen.ViewModels
{
    public class DirectoryViewModel : ViewModel
    {
        private readonly DirectoryInfo directory;
        private bool isExpanded;
        private bool isSelected;
        
        public DirectoryViewModel(DirectoryInfo directory)
        {
            this.directory = directory;
         
            Name = directory.Name;
            Directories = new ObservableCollection<DirectoryViewModel>(new[] { default(DirectoryViewModel)! });
            LoadDirectoriesCommand = new AsyncDelegateCommand(() => LoadDirectoriesAsync());
        }

        public string Name { get; }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }

        public ObservableCollection<DirectoryViewModel> Directories { get; }
        public ICommand LoadDirectoriesCommand { get; }

        private Task LoadDirectoriesAsync()
        {
            Directories.Clear();

            IEnumerable<DirectoryInfo> directories = directory.EnumerateDirectories("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false
            });

            foreach (DirectoryInfo directory in directories)
            {
                Directories.Add(new DirectoryViewModel(directory));
            }

            return Task.CompletedTask;
        }
    }
}
