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

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.LoadingDirectory))
        {
            TheProgressRing.IsActive = ViewModel.LoadingDirectory;
            BezigMetLaden.Visibility = ViewModel.LoadingDirectory ? Visibility.Visible : Visibility.Collapsed;
            AnnuleerLaden.Visibility = BezigMetLaden.Visibility;
        }
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
}
