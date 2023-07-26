using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Core.Contracts.Services;
using Dalfsen.Models;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Dalfsen.ViewModels;

public partial class ImageGridViewModel : ObservableRecipient
{
    [ObservableProperty]
    private DirectoryViewModel? directory;
    private readonly IFileService fileService;

    public ImageGridViewModel(IFileService fileService)
    {
        Images = new ObservableCollection<ImageFileInfo>();
        this.fileService = fileService;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        Images.Clear();
        LoadImagesAsync();
    }

    protected override void OnDeactivated()
    {
        Images.Clear();
    }

    public ObservableCollection<ImageFileInfo> Images
    {
        get;
    }

    public async void LoadImagesAsync()
    {
        if (Images.Count > 0 || Directory == null)
        {
            return;
        }

        var images = await Task.Run(async () =>
        {
            IEnumerable<FileInfo> images = await fileService.GetImagesAsync(Directory.Directory, false).ConfigureAwait(false);
            IEnumerable<Task<ImageFileInfo?>> models = images.Select(image => LoadImageAsync(image));

            ImageFileInfo?[] result = await Task.WhenAll(models).ConfigureAwait(false);

            var validImages = result.Where(image => image != null).Select(image => image!).ToList();
            
            return validImages;
        }).ConfigureAwait(false);

        Dispatcher.InvokeOnUIThread(() =>
        {
            Images.Clear();
            foreach (var image in images)
            {
                Images.Add(image);
            }
        });
    }

    private async Task<ImageFileInfo?> LoadImageAsync(FileInfo file)
    {
        try
        {
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(file.FullName);
            ImageProperties properties = await storageFile.Properties.GetImagePropertiesAsync();

            return new ImageFileInfo(properties, storageFile);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
