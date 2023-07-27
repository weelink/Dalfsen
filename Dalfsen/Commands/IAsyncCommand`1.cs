using System.Threading.Tasks;
using System.Windows.Input;

namespace Dalfsen.Commands
{
    public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
    {
        Task ExecuteAsync(T obj);
        bool CanExecute(object obj);
        ICommand Command { get; }
    }
}
