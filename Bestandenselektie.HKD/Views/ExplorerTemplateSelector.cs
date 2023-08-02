using Bestandenselektie.HKD.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Bestandenselektie.HKD.Views
{
    public class ExplorerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DriveTemplate { get; set; } = null!;
        public DataTemplate DirectoryTemplate { get; set; } = null!;
        public DataTemplate ProcessedDirectoryTemplate { get; set; } = null!;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DriveViewModel)
            {
                return DriveTemplate;
            }

            if (item is DirectoryViewModel directory)
            {
                return directory.IsProcessed ? ProcessedDirectoryTemplate : DirectoryTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
