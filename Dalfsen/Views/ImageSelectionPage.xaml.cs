using Dalfsen.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Dalfsen.Views;

public sealed partial class ImageSelectionPage : Page
{
    public ImageSelectionViewModel ViewModel
    {
        get;
    }

    public ImageSelectionPage()
    {
        ViewModel = App.GetService<ImageSelectionViewModel>();
        InitializeComponent();
    }
}
