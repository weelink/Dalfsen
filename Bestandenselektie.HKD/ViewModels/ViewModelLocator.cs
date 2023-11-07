using Bestandenselektie.HKD.Services;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var storage = new Storage();
            var referenceData = new ReferenceData(storage);

            ExporterViewModel = new ExporterViewModel(storage, referenceData);
            StatusBarViewModel = new StatusBarViewModel();
            MainWindowViewModel = new MainWindowViewModel(storage);
            FileGridViewModel = new FileGridViewModel(StatusBarViewModel, ExporterViewModel, referenceData);
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public StatusBarViewModel StatusBarViewModel { get; }
        public FileGridViewModel FileGridViewModel { get; }
        public ExporterViewModel ExporterViewModel { get; }
    }
}
