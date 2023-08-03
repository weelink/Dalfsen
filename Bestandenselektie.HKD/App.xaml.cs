using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Bestandenselektie.HKD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var file = Path.Join(path, "fout.txt");
            var log = e.Exception.ToString();
            File.WriteAllText(file, log);
        }
    }
}
