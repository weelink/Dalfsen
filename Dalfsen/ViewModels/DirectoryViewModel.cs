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
        if (directories.Count > 0)
        {
            return;
        }

        await Task.Run(async () =>
        {
            var directories = await fileService.GetDirectoriesAsync(Directory).ConfigureAwait(false);
            var viewModels = directories.Select(directory => new DirectoryViewModel(fileService, directory)).ToList();

            this.directories = viewModels;
        }).ConfigureAwait(false);

        Dispatcher.InvokeOnUIThread(() =>
        {
            Directories.Clear();
            foreach (var directory in directories)
            {
                Directories.Add(directory);
            }
        });
    }
}
