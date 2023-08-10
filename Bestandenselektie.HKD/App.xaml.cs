using System;
using System.IO;
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

            MessageBox.Show("Er is een onverwachte fout opgetreden. Op het bureaublad staat het bestand 'fout.txt' met de details.", "Er is een fout opgetreden", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
