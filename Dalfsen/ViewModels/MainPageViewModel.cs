using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Contracts.ViewModels;
using Dalfsen.Core.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

namespace Dalfsen.ViewModels;

public partial class MainPageViewModel : ObservableRecipient, INavigationAware
{
    private readonly IFileService fileService;

    [ObservableProperty]
    private DirectoryViewModel? selectedDirectory;

    public MainPageViewModel(IFileService fileService)
    {
        this.fileService = fileService;
        Drives = new ObservableCollection<DriveViewModel>();
    }

    public void OnDirectorySelected(object? sender, TreeViewItemInvokedEventArgs args)
    {
        SelectedDirectory = (DirectoryViewModel?)args.InvokedItem;
        args.Handled = true;
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        LoadDevicesAsync();
    }

    private async void LoadDevicesAsync()
    {
        await Task.Run(async () =>
        {

            IEnumerable<DriveInfo> drives = await fileService.GetDrivesAsync().ConfigureAwait(false);
            var viewModels = drives.Select(drive => new DriveViewModel(fileService, drive)).ToList();
            var cDrive = viewModels.SingleOrDefault(x => x.Name.Equals("C:\\", StringComparison.InvariantCultureIgnoreCase));

            if (cDrive != null)
            {
                cDrive.IsExpanded = true;
            }

            Dispatcher.InvokeOnUIThread(() =>
            {
                Drives.Clear();
                foreach (var drive in viewModels)
                {
                    Drives.Add(drive);
                }
            });
        });
    }

    public ObservableCollection<DriveViewModel> Drives
    {
        get;
    }
}
