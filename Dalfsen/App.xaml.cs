using System;
using System.Windows;
using System.Windows.Threading;

namespace Dalfsen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Execute.Dispatcher = Dispatcher.CurrentDispatcher;
        }
    }
}
