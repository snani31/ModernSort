
namespace ModernSort.Commands
{
    internal class RelayCommand : CommandBase
    {
        private event Action<object?> _action;
        private event Func<bool> _canExecutePredicate;
        public event Func<bool> CanExecutePredicate 
        {
            add
            { 
                _canExecutePredicate += value; 
            }
            remove 
            { 
                _canExecutePredicate -= value;
            } 
        }
        public RelayCommand(Action<object?> action)
        {
            _action = action;
        }

        public RelayCommand(Action<object?> action, Func<bool> canExecutePredicate) : this(action)
        {
            CanExecutePredicate += canExecutePredicate;
        }

        public override void Execute(object? parameter)
        {
            _action?.Invoke(parameter);
        }

        public override bool CanExecute(object? parameter)
        {
            bool result = true;

            if (_canExecutePredicate is null)
                return true;

            var delegates = _canExecutePredicate?.GetInvocationList();

            if (delegates.Length == 1)
                return _canExecutePredicate?.Invoke() ?? base.CanExecute(parameter);

            foreach (var @delegate in delegates) 
            {
                result = result && (bool)@delegate.DynamicInvoke();
            }

            return result;
        }
    }
}
