using Avalonia;
using Avalonia.Controls;

namespace Dalfsen.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
    }
}
