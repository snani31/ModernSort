using ModernSort.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Stores.Navigation
{
    internal class NavigationStore
    {
        public event Action CurrentViewModelChanged;

        private ViewModelBase? DefaultViewModel { get; init; }

        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public NavigationStore()
        {
            CurrentViewModelClear();
        }

        public NavigationStore(ViewModelBase? defaultViewModel)
        {
            DefaultViewModel = defaultViewModel;
            CurrentViewModelClear();
        }

        /// <summary>
        /// Возникает, чтобы уведомить модель о том, что представление данных изменилось 
        /// </summary>
        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
        /// <summary>
        /// Use this method to set default value for СurrentviewModel
        /// </summary>
        public void CurrentViewModelClear()
        {
            CurrentViewModel = DefaultViewModel;
        }
    }
}
