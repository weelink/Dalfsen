using Avalonia.Controls;
using Dalfsen.ViewModels;

namespace Dalfsen.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        DataContext = new MainViewModel();
    }
}
