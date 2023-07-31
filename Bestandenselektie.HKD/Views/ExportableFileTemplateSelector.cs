using Bestandenselektie.HKD.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Bestandenselektie.HKD.Views
{
    public class ExportableFileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; } = null!;
        public DataTemplate VideoTemplate { get; set; } = null!;
        public DataTemplate PdfTemplate { get; set; } = null!;
        public DataTemplate WordTemplate { get; set; } = null!;
        public DataTemplate ExcelTemplate { get; set; } = null!;
        public DataTemplate RoutesTemplate { get; set; } = null!;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ExportableImageViewModel)
            {
                return ImageTemplate;
            }

            if (item is ExportableVideoViewModel)
            {
                return VideoTemplate;
            }

            if (item is ExportablePdfViewModel)
            {
                return PdfTemplate;
            }

            if (item is ExportableWordViewModel)
            {
                return WordTemplate;
            }

            if (item is ExportableExcelViewModel)
            {
                return ExcelTemplate;
            }

            if (item is ExportableRoutesViewModel)
            {
                return RoutesTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
