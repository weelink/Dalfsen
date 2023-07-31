using System;
using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class DriveViewModel : ExplorerViewModel
    {
        public DriveViewModel(DriveInfo drive)
            : base(drive.RootDirectory)
        {
            IsExpanded = Name.Equals("c:\\", StringComparison.OrdinalIgnoreCase);

            if (IsExpanded)
            {
                LoadDirectoriesCommand.Execute(null);
            }
        }
    }
}
