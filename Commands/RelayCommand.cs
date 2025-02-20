using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Commands
{
    internal class RelayCommand : CommandBase
    {
        private event Action<object?> _action;
        private event Func<bool> _canExecutePredicate;
        public RelayCommand(Action<object?> action)
        {
            _action = action;
        }
        public RelayCommand(Action<object?> action, Func<bool> canExecutePredicate) : this(action)
        {
            _canExecutePredicate = canExecutePredicate;
        }
        public override void Execute(object? parameter)
        {
            _action?.Invoke(parameter);
        }
        public override bool CanExecute(object? parameter)
        {
            return _canExecutePredicate?.Invoke() ?? base.CanExecute(parameter);
        }
    }
}
