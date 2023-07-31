using System;
using System.Threading.Tasks;

namespace Bestandenselektie.HKD.Commands
{
    public class AsyncDelegateCommand : AsyncDelegateCommand<object>, IAsyncCommand
    {
        public AsyncDelegateCommand(Func<Task> executeMethod)
            : base(_ => executeMethod())
        {
        }

        public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(_ => executeMethod(), _ => canExecuteMethod())
        {
        }
    }
}
