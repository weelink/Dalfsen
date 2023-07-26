using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Dalfsen.Models;

public class ImageFileInfo : ObservableRecipient
{
    public ImageFileInfo(ImageProperties properties, StorageFile imageFile)
    {
        ImageProperties = properties;
        ImageName = imageFile.DisplayName;
        ImageFileType = imageFile.DisplayType;
        ImageFile = imageFile;
    }

    public StorageFile ImageFile
    {
        get;
    }

    public ImageProperties ImageProperties
    {
        get;
    }

    public async Task<BitmapImage> GetImageSourceAsync()
    {
        using IRandomAccessStream fileStream = await ImageFile.OpenReadAsync();

        // Create a bitmap to be the image source.
        BitmapImage bitmapImage = new();
        bitmapImage.SetSource(fileStream);

        return bitmapImage;
    }

    public async Task<BitmapImage> GetImageThumbnailAsync()
    {
        StorageItemThumbnail thumbnail = await ImageFile.GetThumbnailAsync(ThumbnailMode.PicturesView);
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

    public string ImageDimensions => $"{ImageProperties.Width} x {ImageProperties.Height}";

    public string ImageTitle
    {
        get => string.IsNullOrEmpty(ImageProperties.Title) ? ImageName : ImageProperties.Title;
        set
        {
            if (ImageProperties.Title != value)
            {
                ImageProperties.Title = value;
                _ = ImageProperties.SavePropertiesAsync();
                OnPropertyChanged();
            }
        }
    }
}
