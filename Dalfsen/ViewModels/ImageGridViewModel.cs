using Dalfsen.Collections;
using Dalfsen.Commands;
using FileTypeChecker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Dalfsen.ViewModels
{
    public class ImageGridViewModel : ViewModel
    {
        private ExplorerViewModel? directory;
        private bool includeSubdirectories;
        private bool isLoading;
        private int numberOfFilesToCheck;
        private int numberOfFilesChecked;

        public ImageGridViewModel()
        {
            Images = new SmartCollection<ExportableImageViewModel>();
            LoadImagesCommand = new AsyncDelegateCommand(() => LoadImagesAsync());
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

        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
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

        private async Task LoadImagesAsync()
        {
            if (Directory == null)
            {
                return;
            }

            NumberOfFilesChecked = 0;
            IsLoading = true;

            try
            {
                Images.Clear();

                IEnumerable<FileInfo> files = Directory.Directory.EnumerateFiles("*", new EnumerationOptions
                {
                    IgnoreInaccessible = true,
                    RecurseSubdirectories = IncludeSubdirectories
                });

                NumberOfFilesToCheck = files.Count();

                IEnumerable<ExportableImageViewModel> viewModels = await Task.Run(() =>
                {
                    IEnumerable<ExportableImageViewModel> viewModels =
                        files
                        .Where((file, index) =>
                        {
                            Execute.OnUIThread(() => NumberOfFilesChecked++);
                            return file.Length > 0 && IsImage(file);
                        })
                        .AsParallel()
                        .Select(file => new ExportableImageViewModel(Directory.Directory, file))
                        .ToList();

                    return viewModels;
                });

                Images.AddRange(viewModels);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool IsImage(FileInfo file)
        {
            try
            {
                using (FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return FileTypeValidator.IsImage(fileStream);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
