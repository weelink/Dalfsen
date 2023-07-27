using System.Windows.Input;

namespace Dalfsen.Commands
{
    public static class CommandExtensions
    {
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            if (command is IRaiseCanExecuteChanged canExecuteChanged)
            {
                canExecuteChanged.RaiseCanExecuteChanged();
            }
        }
    }
}
