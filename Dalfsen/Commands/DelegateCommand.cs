using System;

namespace Dalfsen.Commands
{
    public class AsyncDelegateCommand : DelegateCommand<object>
    {
        public AsyncDelegateCommand(Action executeMethod)
            : base(_ => executeMethod())
        {
        }

        public AsyncDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(_ => executeMethod(), _ => canExecuteMethod())
        {
        }
    }
}
