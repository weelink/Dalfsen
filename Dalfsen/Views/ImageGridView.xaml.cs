using Dalfsen.Models;
using Dalfsen.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Dalfsen.Views;

public sealed partial class ImageGridView : UserControl
{
    public static readonly DependencyProperty DirectoryProperty =
        DependencyProperty.Register("Directory", typeof(DirectoryViewModel), typeof(ImageGridView), new PropertyMetadata(null, new PropertyChangedCallback(OnDirectoryChanged)));

    public ImageGridView()
    {
        ViewModel = App.GetService<ImageGridViewModel>();

        InitializeComponent();
    }

    public DirectoryViewModel? Directory
    {
        get => (DirectoryViewModel?)GetValue(DirectoryProperty);
        set => SetValue(DirectoryProperty, value);
    }

    public ImageGridViewModel ViewModel
    {
        get;
    }

    private static void OnDirectoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ImageGridView)d;
        control.ViewModel.Directory = (DirectoryViewModel)e.NewValue;
    }

    private void ImageGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.InRecycleQueue)
        {
            var templateRoot = (Grid)args.ItemContainer.ContentTemplateRoot;
            var image = (Image)templateRoot.FindName("ItemImage");
            image.Source = null;
        }

        if (args.Phase == 0)
        {
            args.RegisterUpdateCallback(ShowImage);
            args.Handled = true;
        }
    }

    private async void ShowImage(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (args.Phase == 1)
        {
            // It's phase 1, so show this item's image.
            var templateRoot = (Grid)args.ItemContainer.ContentTemplateRoot;
            var image = (Image)templateRoot.FindName("ItemImage");
            var item = (ImageFileInfo)args.Item;
            image.Source = await item.GetImageThumbnailAsync();
        }
    }
}
