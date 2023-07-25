using Dalfsen.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Dalfsen.Views;

public class DirectoryBrowserTemplateSelector : DataTemplateSelector
{
    public DataTemplate DriveTemplate { get; set; } = null!;
    public DataTemplate DirectoryTemplate { get; set; } = null!;

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is DriveViewModel)
        {
            return DriveTemplate;
        }

        return DirectoryTemplate;
    }
}
