using Dalfsen.Collections;
using Dalfsen.Commands;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Dalfsen.ViewModels
{
    public class ImageGridViewModel : ViewModel
    {
        private ExplorerViewModel? directory;
        private bool includeSubdirectories;
        private bool? allSelected;
        private int numberOfFilesToCheck;
        private int numberOfFilesChecked;
        private CancellationTokenSource? cancellationTokenSource;
        private readonly StatusBarViewModel statusIndicator;

        public ImageGridViewModel(StatusBarViewModel statusIndicator, ExporterViewModel exporter)
        {
            this.statusIndicator = statusIndicator;
            Exporter = exporter;
            Images = new SmartCollection<ExportableImageViewModel>();
            LoadImagesCommand = new AsyncDelegateCommand(() => Task.Run(() => LoadImagesAsync()));

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Directory) || args.PropertyName == nameof(IncludeSubdirectories))
                {
                    cancellationTokenSource?.Cancel();
                }
            };
        }

        public ExporterViewModel Exporter { get; }

        public ExplorerViewModel? Directory
        {
            get { return directory; }
            set { SetProperty(ref directory, value); }
        }

        public bool IncludeSubdirectories
        {
            get { return includeSubdirectories; }
            set { SetProperty(ref includeSubdirectories, value); }
        }

        public int NumberOfFilesToCheck
        {
            get { return numberOfFilesToCheck; }
            set { SetProperty(ref numberOfFilesToCheck, value); }
        }

        public int NumberOfFilesChecked
        {
            get { return numberOfFilesChecked; }
            set { SetProperty(ref numberOfFilesChecked, value); }
        }

        public bool? AllSelected
        {
            get { return allSelected; }
            set { SetProperty(ref allSelected, value); }
        }

        public SmartCollection<ExportableImageViewModel> Images { get; }
        public ICommand LoadImagesCommand { get; }

        private void LoadImagesAsync()
        {
            if (Directory == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() => statusIndicator.Indeterminate = true);

            cancellationTokenSource?.Cancel();

            using (cancellationTokenSource = new CancellationTokenSource())
            {
                var cancellationToken = cancellationTokenSource.Token;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Images.Clear();
                    statusIndicator.Reset();
                    statusIndicator.Minimum = 0;
                    statusIndicator.Text = "Bezig met zoeken";
                    statusIndicator.Indeterminate = true;
                    AllSelected = false;
                });

                try
                {
                    IEnumerable<FileInfo> files = Directory.Directory.EnumerateFiles("*", new EnumerationOptions
                    {
                        IgnoreInaccessible = true,
                        RecurseSubdirectories = IncludeSubdirectories
                    }).ToList();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        statusIndicator.Indeterminate = false;
                        statusIndicator.Maximum = files.Count();
                    }, DispatcherPriority.Normal, cancellationToken);

                    IEnumerable<ExportableImageViewModel> viewModels = files.Chunk(100).SelectMany(subfiles => NewMethod(Directory.Directory, subfiles, cancellationToken)).ToList();

                    Application.Current.Dispatcher.Invoke(() => Images.AddRange(viewModels), DispatcherPriority.Normal, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        statusIndicator.Reset();
                    });
                }
                cancellationTokenSource = null;
            }
        }

        private IEnumerable<ExportableImageViewModel> NewMethod(DirectoryInfo directory, FileInfo[] subfiles, CancellationToken cancellationToken)
        {
            var files = subfiles.Where(file => file.Length > 0 && IsImage(file, cancellationToken)).Select(file => new ExportableImageViewModel(directory, file, Exporter)).ToList();

            Application.Current.Dispatcher.Invoke(() => statusIndicator.Value += subfiles.Length);

            return files;
        }

        private bool IsImage(FileInfo file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var extensions = new[] { ".apng", ".avif", ".gif", ".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png", ".webp", ".bmp", ".ico", ".cur", ".tif", ".tiff" };

            return extensions.Contains(file.Extension.ToLowerInvariant());
        }
    }
}
 