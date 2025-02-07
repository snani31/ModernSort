using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Commands
{
    internal class OpenNewWindowCommand : CommandBase
    {
        private readonly Action _action;
        public OpenNewWindowCommand(Action action)
        {
            _action = action;
        }
        public override void Execute(object? parameter)
        {
            _action?.Invoke();
        }
    }
}
