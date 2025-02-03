using ModernSort.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace ModernSort.ViewModel
{
    class MeinWindowViewModel : ViewModelBase
    {
        public MeinWindowViewModel()
        {
            CloseApplicationCommand = new ApplicationCloseCommand();
            CollapselicationCommand = new CollapseApplicationCommand();
        }
        public ICommand CloseApplicationCommand { get; }
        public ICommand CollapselicationCommand { get; }
    }
}
