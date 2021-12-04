using System.Threading.Tasks;
using System.Windows.Input;

namespace RegexTester.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}
