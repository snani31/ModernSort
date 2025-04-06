using RankingEntityes.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.ViewModel.Items.FiltrationItems
{
    internal class ConditionFilterItemViewModel : FilterItemViewModel<ConditionFilter>
    {
        public string ConditionTytle 
        {
            get 
            {
                return Filter.ConditionTytle;
            }
        }
        public ConditionFilterItemViewModel(ConditionFilter filter)
        {
            Filter = filter;
        }
    }
}
