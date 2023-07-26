using Microsoft.UI.Dispatching;

namespace Dalfsen;

public static class Dispatcher
{
    public static DispatcherQueue Queue
    {
        get; set;
    } = null!;

    public static void InvokeOnUIThread(DispatcherQueueHandler callback)
    {
        Queue.TryEnqueue(callback);
    } 
}
