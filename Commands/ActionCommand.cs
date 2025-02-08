using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Commands
{
    internal class ActionCommand : CommandBase
    {

        private readonly Action _action;
        private readonly Func<bool> _canExecutePredicate;
        public ActionCommand(Action action)
        {
            _action = action;
        }
        public ActionCommand(Action action,Func<bool> canExecutePredicate):this(action) 
        {
            _canExecutePredicate = canExecutePredicate;
        }
        public override void Execute(object? parameter)
        {
            _action?.Invoke();
        }
        public override bool CanExecute(object? parameter)
        {
            return _canExecutePredicate?.Invoke() ?? base.CanExecute(parameter);
        }
    }
}
