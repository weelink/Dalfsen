using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dalfsen.Core.Contracts.Services;

namespace Dalfsen.ViewModels;

public partial class DirectoryViewModel : ObservableRecipient
{
    private readonly IFileService fileService;
    private readonly DirectoryInfo directory;
    private IList<DirectoryViewModel> directories = new List<DirectoryViewModel>();

    public DirectoryViewModel(IFileService fileService, DirectoryInfo directory)
    {
        this.fileService = fileService;
        this.directory = directory;

        Directories = new ObservableCollection<DirectoryViewModel>();
    }

    public virtual string Name => directory.Name;

    public ObservableCollection<DirectoryViewModel> Directories
    {
        get; private set;
    }

    public async void LoadDirectoriesAsync()
    {
        if (directories.Count > 0)
        {
            return;
        }

        await Task.Run(async () =>
        {
            var directories = await fileService.GetDirectoriesAsync(directory).ConfigureAwait(false);
            var viewModels = directories.Select(directory => new DirectoryViewModel(fileService, directory)).ToList();

            this.directories = viewModels;
        }).ConfigureAwait(false);

        Directories = new ObservableCollection<DirectoryViewModel>(directories);
    }
}
