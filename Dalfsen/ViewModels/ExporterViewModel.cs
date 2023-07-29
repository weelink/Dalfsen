using Dalfsen.Commands;
using MahApps.Metro.Controls;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Dalfsen.ViewModels
{
    public class ExporterViewModel : ViewModel
    {
        private readonly ISet<ExportableImageViewModel> files;
        private string? targetDirectory;
        private bool isExportWindowOpen;
        private bool isValid;
        private readonly ProgressDialog dialog;
        private MetroWindow? parent;

        public ExporterViewModel()
        {
            files = new HashSet<ExportableImageViewModel>();
            ShowExportCommand = new DelegateCommand(() => IsExportWindowOpen = true);
            ExportCommand = new DelegateCommand<MetroWindow>(parent => ExportImages(parent!), _ => IsValid);
            BrowseDirectoryCommand = new DelegateCommand<MetroWindow>(parent => BrowseDirectory(parent!));
            CancelCommand = new DelegateCommand(() => IsExportWindowOpen = false);
            Images = new ObservableCollection<ExportableImageViewModel>();
            Images.CollectionChanged += Images_CollectionChanged;
            ConflictSolutions = new[] { true, false, false };

            PropertyChanged += ExporterViewModel_PropertyChanged;
            dialog = new ProgressDialog()
            {
                WindowTitle = "Exporteren",
                Text = "Bezig met exporteren...",
                ShowTimeRemaining = true,
            };

            dialog.DoWork += Dialog_DoWork;
            dialog.RunWorkerCompleted += Dialog_RunWorkerCompleted;
        }

        private void Dialog_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(parent, "Exporteren voltooid", "Expoteren", MessageBoxButton.OK, MessageBoxImage.Information);

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var image in Images.ToList())
                {
                    image.IsSelected = false;
                    image.ShouldExport = false;
                }
                IsExportWindowOpen = false;
                Images.Clear();
            });
        }

        private void ExportImages(MetroWindow parent)
        {
            this.parent = parent;
            dialog.ShowDialog(parent);
        }

        private void Dialog_DoWork(object? sender, DoWorkEventArgs e)
        {
            var files = Images.Where(file => file.ShouldExport).ToList();

            for (int x = 0; x < files.Count; x++)
            {
                ExportableImageViewModel file = files[x];
                dialog.ReportProgress(x, null, file.FullPath);

                file.CopyTo(TargetDirectory!);

                if (dialog.CancellationPending)
                {
                    return;
                }
            }
        }

        private void BrowseDirectory(MetroWindow parent)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Selecteer de doelmap";
            dialog.UseDescriptionForTitle = true;

            if (dialog.ShowDialog(parent) == true)
            {
                TargetDirectory = dialog.SelectedPath;
            }
        }

        private void ExporterViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TargetDirectory))
            {
                IsValid = !string.IsNullOrWhiteSpace(TargetDirectory) && Directory.Exists(TargetDirectory);
            }
        }

        public bool[] ConflictSolutions { get; }

        public string? TargetDirectory
        {
            get { return targetDirectory; }
            set { SetProperty(ref targetDirectory, value); }
        }

        public bool IsExportWindowOpen
        {
            get { return isExportWindowOpen; }
            set { SetProperty(ref isExportWindowOpen, value); }
        }

        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
        }

        public ICommand ShowExportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand BrowseDirectoryCommand { get; }
        public ICommand CancelCommand { get; }

        private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ExportableImageViewModel file in e.OldItems.OfType<ExportableImageViewModel>())
                {
                    files.Remove(file);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ExportableImageViewModel file in e.NewItems.OfType<ExportableImageViewModel>())
                {
                    files.Add(file);
                }
            }

            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(HasSelectedItems));
        }

        public bool HasSelectedItems
        {
            get { return Count > 0; }
        }

        public int Count
        {
            get { return Images.Count; }
        }

        public ObservableCollection<ExportableImageViewModel> Images { get; }

        public bool IsSelected(ExportableImageViewModel file)
        {
            return files.Contains(file);
        }

        public void Deselect(ExportableImageViewModel file)
        {
            Images.Remove(file);
        }

        public void Select(ExportableImageViewModel file)
        {
            Images.Add(file);
        }
    }
}
