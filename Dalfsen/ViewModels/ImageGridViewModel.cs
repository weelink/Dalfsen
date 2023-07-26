using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Core.Contracts.Services;
using Dalfsen.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace Dalfsen.ViewModels;

public partial class ImageGridViewModel : ObservableRecipient
{
    [ObservableProperty]
    private DirectoryViewModel? directory;

    [ObservableProperty]
    private bool loadingDirectory;

    [ObservableProperty]
    private bool includeSubdirectories;

    [ObservableProperty]
    private string cancelCaption;

    [ObservableProperty]
    private bool isNotCancelling;

    private readonly IFileService fileService;
    private readonly IExporter exporter;
    private CancellationTokenSource? cancellationTokenSource;

    public ImageGridViewModel(IFileService fileService, IExporter exporter)
    {
        cancellationTokenSource = new CancellationTokenSource();
        Images = new ObservableCollection<ImageFileInfo>();
        this.fileService = fileService;
        this.exporter = exporter;
        cancelCaption = "Annuleren";
        isNotCancelling = true;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Directory) || e.PropertyName == nameof(IncludeSubdirectories))
        {
            Cancel();
            LoadImagesAsync(cancellationTokenSource!.Token);
        }

        base.OnPropertyChanged(e);
    }

    protected override void OnDeactivated()
    {
        Images.Clear();
    }

    public ObservableCollection<ImageFileInfo> Images
    {
        get;
    }

    public void CancelLoading(object? source, RoutedEventArgs e)
    {
        Cancel();
    }

    private void Cancel()
    {
        Dispatcher.InvokeOnUIThread(() =>
        {
            CancelCaption = "Bezig met annuleren...";
            IsNotCancelling = false;
        });

        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
    }

    public async void LoadImagesAsync(CancellationToken cancellationToken)
    {
        Dispatcher.InvokeOnUIThread(() =>
        {
            CancelCaption = "Annuleren";
            IsNotCancelling = true;
            Images.Clear();
            LoadingDirectory = true;
        });
        try
        {
            await Task.Run(async () =>
            {
                IEnumerable<FileInfo> images = await fileService.GetImagesAsync(Directory!.Directory, IncludeSubdirectories, cancellationToken).ConfigureAwait(false);
                IEnumerable<Task> models = images.Where(_ => !cancellationToken.IsCancellationRequested).Select(image => LoadImageAsync(image, cancellationToken));

                await Task.WhenAll(models).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception) { }
        finally
        {
            Dispatcher.InvokeOnUIThread(() =>
            {
                LoadingDirectory = false;
                IsNotCancelling = true;
            });
        }
    }

    private async Task LoadImageAsync(FileInfo file, CancellationToken cancellationToken)
    {
        try
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(file.FullName);
            ImageProperties properties = await storageFile.Properties.GetImagePropertiesAsync();

            if (properties.Width == 0 || properties.Height == 0)
            {
                return;
            }

            Dispatcher.InvokeOnUIThread(async () =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    using (IRandomAccessStream fileStream = await storageFile.OpenReadAsync())
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.SetSource(fileStream);

                        var path = Path.GetRelativePath(Directory!.Directory.FullName, file.FullName!);
                        var fileInfo = new ImageFileInfo(properties, storageFile, bitmapImage, path, file, exporter);

                        Images.Add(fileInfo);
                    }
                }
            });
        }
        catch (Exception)
        {
        }
    }
}
