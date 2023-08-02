using Bestandenselektie.HKD.Services;
using System;
using System.IO;

namespace Bestandenselektie.HKD.ViewModels
{
    public class DriveViewModel : ExplorerViewModel
    {
        public DriveViewModel(DriveInfo drive, Settings settings)
            : base(drive.RootDirectory, settings)
        {
            IsExpanded = Name.Equals("c:\\", StringComparison.OrdinalIgnoreCase);

            if (IsExpanded)
            {
                LoadDirectoriesCommand.Execute(null);
            }
        }
    }
}
