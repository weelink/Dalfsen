using Avalonia.Controls;
using Avalonia.Input;
using Dalfsen.ViewModels;

namespace Dalfsen.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new FilesPageViewModel();
    }

    private void SelectedPath_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var vm = (FilesPageViewModel)DataContext!;
            vm.SelectedPath = ((TextBox)sender!).Text;
        }
    }

}
