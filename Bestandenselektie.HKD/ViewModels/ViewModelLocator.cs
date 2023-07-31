namespace Bestandenselektie.HKD.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ExporterViewModel = new ExporterViewModel();
            StatusBarViewModel = new StatusBarViewModel();
            MainWindowViewModel = new MainWindowViewModel();
            FileGridViewModel = new FileGridViewModel(StatusBarViewModel, ExporterViewModel);
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public StatusBarViewModel StatusBarViewModel { get; }
        public FileGridViewModel FileGridViewModel { get; }
        public ExporterViewModel ExporterViewModel { get; }
    }
}
