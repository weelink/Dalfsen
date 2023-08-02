using Bestandenselektie.HKD.Collections;
using Bestandenselektie.HKD.Commands;
using Bestandenselektie.HKD.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Bestandenselektie.HKD.ViewModels
{
    public abstract class ExplorerViewModel : ViewModel
    {
        private readonly Settings settings;
        private bool isExpanded;
        private bool isProcessed;

        protected ExplorerViewModel(DirectoryInfo directory, Settings settings)
        {
            Directory = directory;
            this.settings = settings;
            Name = directory.Name;
            IsProcessed = settings.HasBeenProcessed(directory);
            Directories = new SmartCollection<DirectoryViewModel>(new[] { default(DirectoryViewModel)! });
            LoadDirectoriesCommand = new DelegateCommand(() => LoadDirectories());
        }

        public string Name { get; }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { SetProperty(ref isExpanded, value); }
        }

        public bool IsProcessed
        {
            get { return isProcessed; }
            set { SetProperty(ref isProcessed, value); }
        }

        public SmartCollection<DirectoryViewModel> Directories { get; }
        public ICommand LoadDirectoriesCommand { get; }
        public DirectoryInfo Directory { get; }

        internal ExplorerViewModel? GetDirectoryViewModelFor(FileInfo file)
        {
            if (Directory.FullName == file.DirectoryName)
            {
                return this;
            }

            var directories = LoadDirectories();
            DirectoryViewModel? directory = directories.SingleOrDefault(directory => directory.Directory.FullName == file.DirectoryName);

            if (directory != null)
            {
                return directory;
            }

            directory = directories.FirstOrDefault(directory => file.DirectoryName!.StartsWith(directory.Directory.FullName, StringComparison.InvariantCultureIgnoreCase));
            if (directory != null)
            {
                return directory.GetDirectoryViewModelFor(file);
            }

            return null;
        }

        private IEnumerable<DirectoryViewModel> LoadDirectories()
        {
            if (Directories.Count != 1 || Directories[0] != null)
            {
                return Directories;
            }

            IEnumerable<DirectoryInfo> directories = Directory.EnumerateDirectories("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = false
            });

            IEnumerable<DirectoryViewModel> directoryViewModels = directories.Select(directory => DirectoryViewModel.Get(directory, settings)).ToList();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Directories.Clear();
                Directories.AddRange(directoryViewModels);
            });

            return directoryViewModels;
        }
    }
}
