using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace Dalfsen
{
    public static class Execute2
    {
        public static void OnUIThread(Action action, DispatcherPriority priority)
        {
            Debug.WriteLine("OnUITHread");
            System.Windows.Application.Current.Dispatcher.Invoke(action);
        }
    }
}
