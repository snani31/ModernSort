using RankingEntityes.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.ViewModel.Items.FiltrationItems
{
    internal class FilterCriterionItemViewModel
    {
        private FilterCriterion FilterCriterion { get; init; }

        public string Tytle 
        {
            get 
            {
                return FilterCriterion.Tytle;
            }
        }

        public string Description
        {
            get
            {
                return FilterCriterion.Description;
            }
        }

        public  List<IFilterViewModel> FilterItems { get; set; }

        public FilterCriterionItemViewModel(FilterCriterion filterCriterion) 
        {
            FilterCriterion = filterCriterion;

            if (FilterCriterion is ConditionFilterCriterion conditionFilter)
            {
                var a = FilterCriterion.Filters.Select(x => new ConditionFilterItemViewModel(x as ConditionFilter));
                FilterItems = new List<IFilterViewModel>(a);

            }

            
            
        }
    }
}
