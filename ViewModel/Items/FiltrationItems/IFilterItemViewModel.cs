using RankingEntityes.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.ViewModel.Items.FiltrationItems
{
    internal interface IFilterViewModel
    {
        public bool IsFilterSelected { get; set; }

        public void SelectFilterIfContains(IEnumerable<IFilter> filters);
    }
}
