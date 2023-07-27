namespace Dalfsen.ViewModels
{
    public class ImageGridViewModel : ViewModel
    {
        private ExplorerViewModel? explorerViewModel;

        public ExplorerViewModel? ExplorerViewModel
        {
            get { return explorerViewModel; }
            set { SetProperty(ref explorerViewModel, value); }
        }
    }
}
