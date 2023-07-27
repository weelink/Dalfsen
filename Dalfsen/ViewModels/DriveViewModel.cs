using Dalfsen.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dalfsen.ViewModels
{
    public class DriveViewModel : ViewModel
    {
        private bool isExpanded;
        private bool isSelected;
        private readonly DriveInfo drive;

        public DriveViewModel(DriveInfo drive)
        {
            this.drive = drive;
            Name = drive.Name;
            IsExpanded = Name.Equals("c:\\", StringComparison.OrdinalIgnoreCase);

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

            IEnumerable<DirectoryInfo> directories = drive.RootDirectory.EnumerateDirectories("*", new EnumerationOptions
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
