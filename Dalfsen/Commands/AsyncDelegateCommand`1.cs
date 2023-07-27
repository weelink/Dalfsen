using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dalfsen.Commands
{
    public class AsyncDelegateCommand<TParameter> : IAsyncCommand<TParameter?>, ICommand
    {
        private readonly Func<TParameter?, Task> execute;
        private readonly DelegateCommand<TParameter?> underlyingCommand;
        private bool isExecuting;

        public AsyncDelegateCommand(Func<TParameter?, Task> executeMethod)
            : this(executeMethod, _ => true)
        {
        }

        public AsyncDelegateCommand(Func<TParameter?, Task> executeMethod, Func<TParameter?, bool> canExecuteMethod)
        {
            execute = executeMethod;
            underlyingCommand = new DelegateCommand<TParameter?>(x => { }, canExecuteMethod);
        }

        public async Task ExecuteAsync(TParameter? obj)
        {
            try
            {
                isExecuting = true;
                RaiseCanExecuteChanged();
                await execute(obj);
            }
            finally
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public ICommand Command { get { return this; } }

        public bool CanExecute(object? parameter)
        {
            return !isExecuting && underlyingCommand.CanExecute((TParameter?)parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { underlyingCommand.CanExecuteChanged += value; }
            remove { underlyingCommand.CanExecuteChanged -= value; }
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync((TParameter?)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            underlyingCommand.RaiseCanExecuteChanged();
        }
    }
}
