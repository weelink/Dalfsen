using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Core.Contracts.Services;

namespace Dalfsen.ViewModels;

public partial class DriveViewModel : ObservableRecipient
{
    private readonly IFileService fileService;
    private readonly DriveInfo drive;

    [ObservableProperty]
    private bool isExpanded;

    public DriveViewModel(IFileService fileService, DriveInfo drive)
    {
        this.fileService = fileService;
        this.drive = drive;
        Directories = new ObservableCollection<DirectoryViewModel>();
    }

    public string Name => drive.Name;

    public ObservableCollection<DirectoryViewModel> Directories
    {
        get;
    }

    public async void LoadDirectoriesAsync()
    {
        if (Directories.Count > 0)
        {
            return;
        }

        await Task.Run(async () =>
        {
            var directories = await fileService.GetDirectoriesAsync(drive.RootDirectory).ConfigureAwait(false);
            var viewModels = directories.Select(directory => new DirectoryViewModel(fileService, directory)).ToList();

            Dispatcher.InvokeOnUIThread(() =>
            {
                Directories.Clear();
                foreach (var viewModel in viewModels)
                {
                    Directories.Add(viewModel);
                }
            });
        }).ConfigureAwait(false);
    }
}
