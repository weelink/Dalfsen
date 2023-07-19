using Caliburn.Micro;
using Dalfsen.UI.ViewModels;
using System.Windows;

namespace Dalfsen.UI
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLog(type);

            Initialize();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync(typeof(ShellViewModel));
        }
    }
}
