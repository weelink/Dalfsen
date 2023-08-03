using Bestandenselektie.HKD.Commands;
using Bestandenselektie.HKD.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Bestandenselektie.HKD.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly Storage storage;

        public MainWindowViewModel(Storage storage)
        {
            Drives = new ObservableCollection<DriveViewModel>();
            LoadCommand = new DelegateCommand(() => Load());
            this.storage = storage;
        }

        public ICommand LoadCommand { get; }
        public ObservableCollection<DriveViewModel> Drives { get; }

        private void Load()
        {
            Settings settings = storage.ReadSettings();
            IEnumerable<DriveInfo> drives = DriveInfo.GetDrives().Where(drive => drive.IsReady);
            
            foreach (DriveInfo drive in drives)
            {
                Drives.Add(new DriveViewModel(drive, settings));
            }
        }
    }
}
