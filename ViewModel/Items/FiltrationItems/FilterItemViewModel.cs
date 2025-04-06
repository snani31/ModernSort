using RankingEntityes.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.ViewModel.Items.FiltrationItems
{
    internal class FilterItemViewModel<T> : ViewModelBase,IFilterViewModel
        where T : Filter
    {
        public T Filter {  get; set; }
        private bool _isFilterSelected;
        public bool IsFilterSelected 
        { 
            get 
            {
                return _isFilterSelected;
            }

            set 
            {
                _isFilterSelected = value;
                OnPropertyChenged(nameof(IsFilterSelected));
            } 
        }
    }
}
