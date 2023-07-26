using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Core.Contracts.Services;

namespace Dalfsen.ViewModels;

public partial class DriveViewModel : ObservableRecipient
{
    private readonly IFileService fileService;

    [ObservableProperty]
    private bool isExpanded;

    public DriveViewModel(IFileService fileService, DriveInfo drive)
    {
        this.fileService = fileService;
        Drive = drive;
        Directories = new ObservableCollection<DirectoryViewModel>();
    }

    public string Name => Drive.Name;

    public ObservableCollection<DirectoryViewModel> Directories
    {
        get;
    }
    public DriveInfo Drive { get; }

    public async void LoadDirectoriesAsync()
    {
        Dispatcher.InvokeOnUIThread(() => Directories.Clear());

        await Task.Run(async () =>
        {
            var directories = await fileService.GetDirectoriesAsync(Drive.RootDirectory, CancellationToken.None).ConfigureAwait(false);

            foreach (var directory in directories)
            {
                var viewModel = new DirectoryViewModel(fileService, directory);
                Dispatcher.InvokeOnUIThread(() => Directories.Add(viewModel));
            }
        }).ConfigureAwait(false);
    }
}
