using Bestandenselektie.HKD.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Bestandenselektie.HKD.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            Drives = new ObservableCollection<DriveViewModel>();
            LoadCommand = new DelegateCommand(() => Load());
        }

        public ICommand LoadCommand { get; }
        public ObservableCollection<DriveViewModel> Drives { get; }

        private void Load()
        {
            IEnumerable<DriveInfo> drives = DriveInfo.GetDrives().Where(drive => drive.IsReady);

            foreach (DriveInfo drive in drives)
            {
                Drives.Add(new DriveViewModel(drive));
            }
        }
    }
}
