namespace Dalfsen.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ExporterViewModel = new ExporterViewModel();
            StatusBarViewModel = new StatusBarViewModel();
            MainWindowViewModel = new MainWindowViewModel();
            ImageGridViewModel = new ImageGridViewModel(StatusBarViewModel, ExporterViewModel);
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public StatusBarViewModel StatusBarViewModel { get; }
        public ImageGridViewModel ImageGridViewModel { get; }
        public ExporterViewModel ExporterViewModel { get; }
    }
}
