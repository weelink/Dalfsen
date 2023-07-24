using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Dalfsen.Contracts.Services;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;

namespace Dalfsen.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;

    public ICommand MenuFileExitCommand
    {
        get;
    }

    public ICommand MenuViewsListDetailsCommand
    {
        get;
    }

    public INavigationService NavigationService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        MenuFileExitCommand = new RelayCommand(OnMenuFileExit);
        MenuViewsListDetailsCommand = new RelayCommand(OnMenuViewsListDetails);
    }

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

    private void OnMenuFileExit() => Application.Current.Exit();

    private void OnMenuViewsListDetails() => NavigationService.NavigateTo(typeof(ListDetailsViewModel).FullName!);
}
