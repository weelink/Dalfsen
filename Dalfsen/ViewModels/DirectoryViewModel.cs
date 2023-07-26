using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Core.Contracts.Services;

namespace Dalfsen.ViewModels;

public partial class DirectoryViewModel : ObservableRecipient
{
    private readonly IFileService fileService;
    private IList<DirectoryViewModel> directories = new List<DirectoryViewModel>();
    [ObservableProperty]
    private DirectoryInfo directory;

    public DirectoryViewModel(IFileService fileService, DirectoryInfo directory)
    {
        this.fileService = fileService;
        this.directory = directory;

        Directories = new ObservableCollection<DirectoryViewModel>();
    }

    public virtual string Name => Directory.Name;

    public ObservableCollection<DirectoryViewModel> Directories
    {
        get;
    }

    public async void LoadDirectoriesAsync()
    {
        Dispatcher.InvokeOnUIThread(() => Directories.Clear());

        await Task.Run(async () =>
        {
            var directories = await fileService.GetDirectoriesAsync(Directory, CancellationToken.None).ConfigureAwait(false);
            foreach (var directory in directories)
            {
                var viewModel = new DirectoryViewModel(fileService, directory);
                Dispatcher.InvokeOnUIThread(() => Directories.Add(viewModel));
            }
        }).ConfigureAwait(false);
    }
}
