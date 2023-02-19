using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StrategyManagerSolution.MVVMUtils
{
    internal class Command : ICommand
    {
        public event Action<object?> Called;
        public event EventHandler? CanExecuteChanged;
        private bool _canExecute = true;
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        public bool CanExecute(object? parameter)
        {
            return _canExecute;
        }
        public void Execute(object? parameter)
        {
            Called?.Invoke(parameter);
        }
        public Command(Action<object?> func)
        {
            Called += func;
        }
    }
}
