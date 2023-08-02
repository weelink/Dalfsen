using Bestandenselektie.HKD.Services;

namespace Bestandenselektie.HKD.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var storage = new Storage();

            ExporterViewModel = new ExporterViewModel(storage);
            StatusBarViewModel = new StatusBarViewModel();
            MainWindowViewModel = new MainWindowViewModel(storage);
            FileGridViewModel = new FileGridViewModel(StatusBarViewModel, ExporterViewModel);
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public StatusBarViewModel StatusBarViewModel { get; }
        public FileGridViewModel FileGridViewModel { get; }
        public ExporterViewModel ExporterViewModel { get; }
    }
}
