using Dalfsen.Collections;
using Dalfsen.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int numberOfFilesToCheck;
        private int numberOfFilesChecked;
        private CancellationTokenSource? cancellationTokenSource;
        private StatusBarViewModel statusIndicator;

        public ImageGridViewModel(StatusBarViewModel statusIndicator)
        {
            this.statusIndicator = statusIndicator;
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
                    statusIndicator.Reset();
                    statusIndicator.Minimum = 0;
                    statusIndicator.Text = "Bezig met zoeken";
                    statusIndicator.Indeterminate = true;
                    Images.Clear();
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
                catch (OperationCanceledException e)
                {
                    Debug.WriteLine(e.ToString());
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
            var files = subfiles.Where(file => file.Length > 0 && IsImage(file, cancellationToken)).Select(file => new ExportableImageViewModel(directory, file)).ToList();

            Application.Current.Dispatcher.Invoke(() => statusIndicator.Value += subfiles.Length);

            return files;
        }

        private bool IsImage(FileInfo file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var extensions = new[] {
                ".apng",
".avif",
".gif",
".jpg",
".jpeg",
".jfif",
".pjpeg",
".pjp",
".png",
".svg",
".webp",
".bmp",
".ico",
".cur",
".tif",
".tiff"
            };

            return extensions.Contains(file.Extension.ToLowerInvariant());
        }
    }
}
