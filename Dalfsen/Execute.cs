using System;
using System.Windows.Threading;

namespace Dalfsen
{
    public static class Execute
    {
        public static Dispatcher Dispatcher { get; set; } = null!;

        public static void OnUIThread(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}
