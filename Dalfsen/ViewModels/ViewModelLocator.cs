namespace Dalfsen.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            StatusBarViewModel = new StatusBarViewModel();
            MainWindowViewModel = new MainWindowViewModel();
            ImageGridViewModel = new ImageGridViewModel(StatusBarViewModel);
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public StatusBarViewModel StatusBarViewModel { get; }
        public ImageGridViewModel ImageGridViewModel { get; }
    }
}
