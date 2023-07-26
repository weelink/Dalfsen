using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Dalfsen.Core.Contracts.Services;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace Dalfsen.Models;

public partial class ImageFileInfo : ObservableRecipient
{
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private BitmapImage imageSource;

    [ObservableProperty]
    private string path;
    private readonly IExporter exporter;

    public ImageFileInfo(ImageProperties properties, StorageFile imageFile, BitmapImage imageSource, string path, FileInfo fileInfo, IExporter exporter)
    {
        ImageProperties = properties;
        ImageName = imageFile.DisplayName;
        ImageFileType = imageFile.DisplayType;
        ImageFile = imageFile;
        Width = ImageProperties.Width;
        Height = ImageProperties.Height;
        this.imageSource = imageSource;
        this.path = path;
        FileInfo = fileInfo;
        this.exporter = exporter;
        isSelected = exporter.Contains(fileInfo);
    }

    public void SelectCommand(object? sender, RoutedEventArgs e)
    {
        if (IsSelected)
        {
            exporter.Add(FileInfo);
        }
        else
        {
            exporter.Remove(FileInfo);
        }
    }

    public StorageFile ImageFile
    {
        get;
    }
    public FileInfo FileInfo { get; }
    public ImageProperties ImageProperties
    {
        get;
    }

    public async Task<BitmapImage> GetImageThumbnailAsync()
    {
        IRandomAccessStream thumbnail = await ImageFile.GetThumbnailAsync(ThumbnailMode.PicturesView);

        thumbnail ??= await ImageFile.OpenReadAsync();

        // Create a bitmap to be the image source.
        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(thumbnail);
        thumbnail.Dispose();

        return bitmapImage;
    }

    public string ImageName
    {
        get;
    }

    public string ImageFileType
    {
        get;
    }

    public uint Width
    {
        get;
    }

    public uint Height
    {
        get;
    }

    public string ImageDimensions => $"{Width} x {Height}";

    public string ImageTitle => ImageName;
}
