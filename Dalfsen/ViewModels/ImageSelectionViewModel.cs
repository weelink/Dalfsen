using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Contracts.ViewModels;
using Dalfsen.Core.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

namespace Dalfsen.ViewModels;

public partial class ImageSelectionViewModel : ObservableRecipient, INavigationAware
{
    private readonly IFileService fileService;

    [ObservableProperty]
    private DirectoryViewModel? selectedDirectory;

    public ImageSelectionViewModel(IFileService fileService)
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
        IEnumerable<DriveInfo> drives = await fileService.GetDrivesAsync();
        var viewModels = drives.Select(drive => new DriveViewModel(fileService, drive)).ToList();
        var cDrive = viewModels.SingleOrDefault(x => x.Name.Equals("C:\\", StringComparison.InvariantCultureIgnoreCase));

        if (cDrive != null)
        {
            cDrive.IsExpanded = true;
        }

        Drives = new ObservableCollection<DriveViewModel>(viewModels);
    }

    public ObservableCollection<DriveViewModel> Drives
    {
        get; private set;
    }
}
