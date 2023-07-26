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
        if (args.InvokedItem is DriveViewModel drive)
        {
            SelectedDirectory = new DirectoryViewModel(fileService, drive.Drive.RootDirectory);
        }
        else
        {
            SelectedDirectory = (DirectoryViewModel?)args.InvokedItem;
        }

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
        Dispatcher.InvokeOnUIThread(() => Drives.Clear());

        await Task.Run(async () =>
        {
            IEnumerable<DriveInfo> drives = await fileService.GetDrivesAsync(CancellationToken.None).ConfigureAwait(false);
            var viewModels = drives.Select(drive => new DriveViewModel(fileService, drive)).ToList();
            var cDrive = viewModels.SingleOrDefault(x => x.Name.Equals("C:\\", StringComparison.InvariantCultureIgnoreCase));

            if (cDrive != null)
            {
                cDrive.IsExpanded = true;
            }

            foreach (var viewModel in viewModels)
            {
                Dispatcher.InvokeOnUIThread(() => Drives.Add(viewModel));
            }
        }).ConfigureAwait(false);
    }

    public ObservableCollection<DriveViewModel> Drives
    {
        get;
    }
}
