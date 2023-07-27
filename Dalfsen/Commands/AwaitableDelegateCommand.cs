using System;
using System.Threading.Tasks;

namespace Dalfsen.Commands
{
    public class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand
    {
        public AwaitableDelegateCommand(Func<Task> executeMethod)
            : base(_ => executeMethod())
        {
        }

        public AwaitableDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(_ => executeMethod(), _ => canExecuteMethod())
        {
        }
    }
}
