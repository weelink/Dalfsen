using Dalfsen.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Dalfsen.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        ViewModel = App.GetService<MainPageViewModel>();
        InitializeComponent();
    }

    public MainPageViewModel ViewModel
    {
        get;
    }
}
