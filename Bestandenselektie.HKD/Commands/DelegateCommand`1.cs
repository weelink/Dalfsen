using System;
using System.Windows.Input;

namespace Bestandenselektie.HKD.Commands
{
    public class DelegateCommand<TParameter> : ICommand, IRaiseCanExecuteChanged
    {
        private readonly Func<TParameter?, bool> canExecute;
        private readonly Action<TParameter?> execute;
        private bool isExecuting;

        public DelegateCommand(Action<TParameter?> executeMethod)
            : this(executeMethod, _ => true)
        {
        }

        public DelegateCommand(Action<TParameter?> executeMethod, Func<TParameter?, bool> canExecuteMethod)
        {
            execute = executeMethod;
            canExecute = canExecuteMethod;
        }

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return !isExecuting && CanExecute((TParameter?)parameter);
        }

        void ICommand.Execute(object? parameter)
        {
            isExecuting = true;
            try
            {
                RaiseCanExecuteChanged();
                Execute((TParameter?)parameter);
            }
            finally
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute(TParameter? parameter)
        {
            if (canExecute == null)
            {
                return true; 
            }

            return canExecute(parameter);
        }

        public void Execute(TParameter? parameter)
        {
            execute(parameter);
        }
    }
}
