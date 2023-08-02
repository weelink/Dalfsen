using Bestandenselektie.HKD.Collections;
using Bestandenselektie.HKD.Commands;
using Bestandenselektie.HKD.Extensions;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Bestandenselektie.HKD.ViewModels
{
    public class FileGridViewModel : ViewModel
    {
        private static readonly string[] ImageExtensions = new[] {
            ".apng", ".avif", ".gif", ".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png", ".webp", ".bmp", ".ico", ".cur", ".tif", ".tiff"
        };

        private static readonly string[] VideoExtensions = new[] {
            ".webm", ".mkv", ".flv", ".flv", ".vob", ".ogv", ".ogg", ".drc", ".gif", ".gifv", ".mng", ".avi", ".MTS", ".M2TS", ".TS", ".mov",
            ".qt", ".wmv", ".yuv", ".rm", ".rmvb", ".viv", ".asf", ".amv", ".mp4", ".m4p", ".m4v", ".mpg", ".mp2", ".mpeg", ".mpe", ".mpv",
            ".mpg", ".mpeg", ".m2v", ".m4v", ".svi", ".3gp", ".3g2", ".mxf", ".roq", ".nsv", ".flv", ".f4v", ".f4p", ".f4a", ".f4b"
        };

        private static readonly string[] PdfExtensions = new[] { ".pdf" };
        private static readonly string[] WordExtensions = new[] { ".doc", ".docx", ".odt", ".rtf" };
        private static readonly string[] ExcelExtensions = new[] { ".csv", ".ods", ".xls", ".xlsx", ".xps" };
        private static readonly string[] RouteExtensions = new[] { ".gpx" };

        private static string[] SupportedExtensions =>
            ImageExtensions
            .Concat(VideoExtensions)
            .Concat(PdfExtensions)
            .Concat(WordExtensions)
            .Concat(ExcelExtensions)
            .Concat(RouteExtensions)
            .ToArray();

        private ExplorerViewModel? directory;
        private bool includeSubdirectories;
        private bool? allSelected;
        private bool? allImagesSelected;
        private bool? allVideosSelected;
        private bool? allPdfsSelected;
        private bool? allWordsSelected;
        private bool? allExcelsSelected;
        private bool? allRoutesSelected;
        private int numberOfFilesToCheck;
        private int numberOfFilesChecked;
        private string totalSize;
        private CancellationTokenSource? cancellationTokenSource;
        private readonly StatusBarViewModel statusIndicator;

        public FileGridViewModel(StatusBarViewModel statusIndicator, ExporterViewModel exporter)
        {
            this.statusIndicator = statusIndicator;
            Exporter = exporter;
            Files = new SmartCollection<ExportableFileViewModel>();
            totalSize = string.Empty;
            AllImagesSelected = AllExcelsSelected = AllPdfsSelected = AllRoutesSelected = AllVideosSelected = AllWordsSelected = AllSelected = false;

            LoadFilesCommand = new AsyncDelegateCommand(() => Task.Run(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);
                    LoadFiles();
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() => Mouse.OverrideCursor = null);
                }
            }));

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Directory) || args.PropertyName == nameof(IncludeSubdirectories))
                {
                    OnPropertyChanged(nameof(HasDirectory));
                    cancellationTokenSource?.Cancel();
                    LoadFilesCommand.Execute(null);
                }
            };

            exporter.Files.CollectionChanged += (sender, args) =>
            {
                if (exporter.Files.Count == 0)
                {
                    AllImagesSelected = AllExcelsSelected = AllPdfsSelected = AllRoutesSelected = AllVideosSelected = AllWordsSelected = AllSelected = false;
                }
            };

            SelectAllCommand = new DelegateCommand<bool?>(isChecked =>
            {
                Select<ExportableFileViewModel>(isChecked);
                AllSelected = isChecked;
                AllImagesSelected = AllExcelsSelected = AllPdfsSelected = AllRoutesSelected = AllVideosSelected = AllWordsSelected = isChecked;
            });

            SelectImagesCommand = new DelegateCommand<bool?>(isChecked =>
            {
                Select<ExportableImageViewModel>(isChecked);
                AllSelected = AllImagesSelected == true && AllExcelsSelected == true && AllPdfsSelected == true && AllRoutesSelected == true && AllVideosSelected == true && AllWordsSelected == true;
            });

            SelectVideosCommand = new DelegateCommand<bool?>(isChecked =>
            {
                Select<ExportableVideoViewModel>(isChecked);
                AllSelected = AllImagesSelected == true && AllExcelsSelected == true && AllPdfsSelected == true && AllRoutesSelected == true && AllVideosSelected == true && AllWordsSelected == true;
            });

            SelectPdfsCommand = new DelegateCommand<bool?>(isChecked =>
            {
                Select<ExportablePdfViewModel>(isChecked);
                AllSelected = AllImagesSelected == true && AllExcelsSelected == true && AllPdfsSelected == true && AllRoutesSelected == true && AllVideosSelected == true && AllWordsSelected == true;
            });

            SelectWordsCommand = new DelegateCommand<bool?>(isChecked =>
            {
                Select<ExportableWordViewModel>(isChecked);
                AllSelected = AllImagesSelected == true && AllExcelsSelected == true && AllPdfsSelected == true && AllRoutesSelected == true && AllVideosSelected == true && AllWordsSelected == true;
            });

            SelectExcelsCommand = new DelegateCommand<bool?>(isChecked =>
            {
                Select<ExportableExcelViewModel>(isChecked);
                AllSelected = AllImagesSelected == true && AllExcelsSelected == true && AllPdfsSelected == true && AllRoutesSelected == true && AllVideosSelected == true && AllWordsSelected == true;
            });

            SelectRoutesCommand = new DelegateCommand<bool?>(isChecked =>
            {
                Select<ExportableRoutesViewModel>(isChecked);
                AllSelected = AllImagesSelected == true && AllExcelsSelected == true && AllPdfsSelected == true && AllRoutesSelected == true && AllVideosSelected == true && AllWordsSelected == true;
            });
        }

        private void Select<TViewModel>(bool? isChecked) where TViewModel : ExportableFileViewModel
        {
            foreach (var item in Files.OfType<TViewModel>())
            {
                item.IsSelected = isChecked == true;
            }
        }

        public ExporterViewModel Exporter { get; }

        public ExplorerViewModel? Directory
        {
            get { return directory; }
            set { SetProperty(ref directory, value); }
        }

        public string TotalSize
        {
            get { return totalSize; }
            set { SetProperty(ref totalSize, value); }
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

        public bool HasDirectory
        {
            get { return Directory != null; }
        }

        public bool? AllSelected
        {
            get { return allSelected; }
            set { SetProperty(ref allSelected, value); }
        }

        public bool? AllImagesSelected
        {
            get { return allImagesSelected; }
            set { SetProperty(ref allImagesSelected, value); }
        }

        public bool? AllVideosSelected
        {
            get { return allVideosSelected; }
            set { SetProperty(ref allVideosSelected, value); }
        }

        public bool? AllPdfsSelected
        {
            get { return allPdfsSelected; }
            set { SetProperty(ref allPdfsSelected, value); }
        }

        public bool? AllWordsSelected
        {
            get { return allWordsSelected; }
            set { SetProperty(ref allWordsSelected, value); }
        }

        public bool? AllExcelsSelected
        {
            get { return allExcelsSelected; }
            set { SetProperty(ref allExcelsSelected, value); }
        }

        public bool? AllRoutesSelected
        {
            get { return allRoutesSelected; }
            set { SetProperty(ref allRoutesSelected, value); }
        }

        public SmartCollection<ExportableFileViewModel> Files { get; }
        public ICommand LoadFilesCommand { get; }

        public ICommand SelectAllCommand { get; }
        public ICommand SelectImagesCommand { get; }
        public ICommand SelectVideosCommand { get; }
        public ICommand SelectPdfsCommand { get; }
        public ICommand SelectWordsCommand { get; }
        public ICommand SelectExcelsCommand { get; }
        public ICommand SelectRoutesCommand { get; }

        private void LoadFiles()
        {
            if (Directory == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                AllSelected = null;
                statusIndicator.Indeterminate = true;
            });

            cancellationTokenSource?.Cancel();

            using (cancellationTokenSource = new CancellationTokenSource())
            {
                var cancellationToken = cancellationTokenSource.Token;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Files.Clear();
                    statusIndicator.Reset();
                    statusIndicator.Minimum = 0;
                    statusIndicator.Text = "Bezig met zoeken";
                    statusIndicator.Indeterminate = true;
                    AllSelected = false;
                });

                try
                {
                    IEnumerable<FileInfo> files = Directory.Directory
                        .EnumerateFiles("*", new EnumerationOptions
                        {
                            IgnoreInaccessible = true,
                            RecurseSubdirectories = IncludeSubdirectories
                        })
                        .Where(file => IsSupported(file, cancellationToken) && file.Length > 0)
                        .ToList();

                    long size = files.Sum(file => file.Length);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TotalSize = size > 0 ? $"Totale grootte: {size.ToSize()}" : string.Empty;
                        statusIndicator.Indeterminate = false;
                        statusIndicator.Maximum = files.Count();
                    }, DispatcherPriority.Normal, cancellationToken);

                    IEnumerable<ExportableFileViewModel> viewModels =
                        files
                            .Chunk(100)
                            .SelectMany(subfiles => CreateFiles(subfiles))
                            .ToList();

                    Application.Current.Dispatcher.Invoke(() => Files.AddRange(viewModels), DispatcherPriority.Normal, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(() => statusIndicator.Reset());
                }

                cancellationTokenSource = null;
            }
        }

        private IEnumerable<ExportableFileViewModel> CreateFiles(FileInfo[] subfiles)
        {
            var files = subfiles.Select(file => CreateExportableVieModel(file)).OfType<ExportableFileViewModel>().ToList();

            Application.Current.Dispatcher.Invoke(() => statusIndicator.Value += subfiles.Length);

            return files;
        }

        private ExportableFileViewModel? CreateExportableVieModel(FileInfo file)
        {
            var extension = file.Extension.ToLowerInvariant();

            ExplorerViewModel directory = Directory!.GetDirectoryViewModelFor(file) ?? Directory!;

            if (ImageExtensions.Contains(extension))
            {
                return new ExportableImageViewModel(directory, file, Exporter);
            }

            if (VideoExtensions.Contains(extension))
            {
                return new ExportableVideoViewModel(directory, file, Exporter);
            }

            if (PdfExtensions.Contains(extension))
            {
                return new ExportablePdfViewModel(directory, file, Exporter);
            }

            if (WordExtensions.Contains(extension))
            {
                return new ExportableWordViewModel(directory, file, Exporter);
            }

            if (ExcelExtensions.Contains(extension))
            {
                return new ExportableExcelViewModel(directory, file, Exporter);
            }

            if (RouteExtensions.Contains(extension))
            {
                return new ExportableRoutesViewModel(directory, file, Exporter);
            }

            return null;
        }

        private bool IsSupported(FileInfo file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return SupportedExtensions.Contains(file.Extension.ToLowerInvariant());
        }
    }
}
