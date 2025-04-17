using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Items.FiltrationItems;
using RankingEntityes.Filters;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernSort.ViewModel.Pages
{
    class FiltrationPageViewModel : ViewModelBase
    {
        public event Action<ObservableCollection<MediaObject>> SetFilterableCollectionValue;
        public List<IFilter> SelectedFilters { get; set; }

        public event Action<FilterCriterion> OnEditButtonPressed;

        private FiltrationService<MediaObject> FiltrationService { get; set; }

        public ObservableCollection<MediaObjectItemViewModel> FilterableMediaObjects {  get; private set; }
        public ObservableCollection<FilterCriterionItemViewModel> FilterCriterions { get; init; }

        public ICommand AddFilterSelectionList {  get; init; }
        public ICommand RefreshFilterSelectionList { get; init; }
        public ICommand RemoveFilterFromSelectionList { get; init; }

        public ICommand OpenEditWindow { get; init; }

        public FiltrationPageViewModel()
        {
            SelectedFilters = new List<IFilter>();
            AddFilterSelectionList = new RelayCommand(AddFilterSelectionListCommandMethod);
            RemoveFilterFromSelectionList = new RelayCommand(RemoveFilterFromSelectionListCommandMethod);
            RefreshFilterSelectionList = new RelayCommand(RefreshFiltersInSelectionListCommandMethod);
            OpenEditWindow = new RelayCommand(
                (p) =>
                {
                    if (p is FilterCriterionItemViewModel filterCriterionItem)
                    OnEditButtonPressed?.Invoke(filterCriterionItem.FilterCriterion);
                });
        }

        public FiltrationPageViewModel(ObservableCollection<MediaObjectItemViewModel> filterableMediaObjects,
            OutputContentService contentService) : this()
        {
            FilterableMediaObjects = filterableMediaObjects;

            FilterCriterions = new ObservableCollection<FilterCriterionItemViewModel>(
                contentService.FilterCriterionContentService.GetUnloadedFilterCriterions()
                .Select(x => new FilterCriterionItemViewModel(x)));


            var s = new ObservableCollection<MediaObject>(contentService.MediaObjectContentService.GetUnloadedMediaObjects());

            FiltrationService = new FiltrationService<MediaObject>(s);
        }

        private void RemoveFilterFromSelectionListCommandMethod(object? parameter)
        {
            if (parameter is ConditionFilterItemViewModel filterVm)
            {
                FiltrationService.RemoveNewFilter(filterVm.Filter);
                var a = new ObservableCollection<MediaObject>(FiltrationService.CurrentFilterableCollectionState);
                SetFilterableCollectionValue(a);
            }
        }

        private void AddFilterSelectionListCommandMethod(object? parameter)
        {
            if (parameter is ConditionFilterItemViewModel filterVm)
            {
                FiltrationService.AddNewFilter(filterVm.Filter);
                var a = new ObservableCollection<MediaObject>(FiltrationService.CurrentFilterableCollectionState);
                SetFilterableCollectionValue(a);
            }
        }

        private void RefreshFiltersInSelectionListCommandMethod(object? parameter)
        {
            foreach (var filterCriterionVm in FilterCriterions)
            {
                foreach (var filterItem in filterCriterionVm.FilterItems)
                {
                    filterItem.IsFilterSelected = false;
                }
            }

            FiltrationService.RefreshFilters();
            var a = new ObservableCollection<MediaObject>(FiltrationService.CurrentFilterableCollectionState);
            SetFilterableCollectionValue(a);
        }
    }
}
