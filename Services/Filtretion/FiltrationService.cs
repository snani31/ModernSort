using ModernSort.ViewModel.Items;
using RankingEntityes.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Services
{
    internal class FiltrationService<TFilterableType> where TFilterableType : IFilterable
    {
        private IList<TFilterableType> BaseFilterableCollectionState { get; set; }

        public IEnumerable<TFilterableType> CurrentFilterableCollectionState { get; private set; }

        private List<IFilter> SelectedFilters { get; set; }

        public FiltrationService(IList<TFilterableType> baseFilterableCollectionState)
        {
            SelectedFilters = new List<IFilter>();
            BaseFilterableCollectionState = baseFilterableCollectionState;
            CurrentFilterableCollectionState = BaseFilterableCollectionState;
        }
        /// <summary>
        /// Метод, который обнуляет фильтры
        /// </summary>
        public void ReloadFiltration()
        {

        }

        public void AddNewFilter(IFilter filter)
        {
            IList<TFilterableType> fil = CurrentFilterableCollectionState.ToList();
            SelectedFilters.Add(filter);

            foreach (var tempFilter in SelectedFilters)
            {
                foreach (var ss in CurrentFilterableCollectionState)
                {
                    if (!tempFilter.CompliantToFilter(ss))
                    {
                        fil.Remove(ss);
                    }
                }
            }

            CurrentFilterableCollectionState = fil;

        }

        public void RefreshFilters()
        {
            SelectedFilters.Clear();
            CurrentFilterableCollectionState = BaseFilterableCollectionState;
        }

        public void RemoveNewFilter(IFilter filter)
        {
            SelectedFilters.Remove(filter);
            IList<TFilterableType> fil = BaseFilterableCollectionState.Where(x => SelectedFilters.
                                                   All(f => f.CompliantToFilter(x))).ToList();

            CurrentFilterableCollectionState = fil;
        }
    }
}
