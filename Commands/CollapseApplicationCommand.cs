using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernSort.Commands
{
    class CollapseApplicationCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            MinimizeWondowSize();
            void MinimizeWondowSize()
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }
    }
}
