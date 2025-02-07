using ModernSort.Stores;
using ModernSort.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Commands
{
    internal class NavigationCommand : CommandBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<ViewModelBase> _createViewModel;

        public NavigationCommand(NavigationStore navigationStore, Func<ViewModelBase> CreateViewModel)
        {
            _navigationStore = navigationStore;
            this._createViewModel = CreateViewModel;
        }

        public override void Execute(object parameter)
        {
            ViewModelBase localViewModel = _createViewModel();
            if (_navigationStore.CurrentViewModel.GetType() != localViewModel.GetType())
            {
                _navigationStore.CurrentViewModel = localViewModel;
            }
        }
    }
}
