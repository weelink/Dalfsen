using System.Windows.Input;

namespace Bestandenselektie.HKD.Commands
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
