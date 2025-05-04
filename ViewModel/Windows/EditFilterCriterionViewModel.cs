using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operation;
using ModernSort.Services.Operations;
using ModernSort.ViewModel.Items.FiltrationItems;
using Newtonsoft.Json;
using RankingEntityes.Filters;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class EditFilterCriterionViewModel : ViewModelValidateble, IDialogRequestClose
    {
        #region Services

        private OperationService OperationService { get; init; }
        public OutputContentService ContentService { get; init; }

        #endregion

        #region Observable propertyes

        private string _title;

        [Required(ErrorMessage = "Tytle can not be Empthy")]
        [MaxLength(30, ErrorMessage = "Max Lenght for Tytle is 30 symbols")]
        public string Tytle
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChenged(nameof(Tytle));
                Validate(nameof(Tytle), value);
            }
        }

        private string _descriptyon;
        [Required(ErrorMessage = "Description can not be Empthy")]
        [MaxLength(100, ErrorMessage = "Max Lenght for Description is 100 symbols")]
        public string Descriptyon
        {
            get
            {
                return _descriptyon;
            }
            set
            {
                _descriptyon = value;
                OnPropertyChenged(nameof(Descriptyon));
                Validate(nameof(Descriptyon), value);
            }
        }

        #region Events

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        #endregion
        [MinLength(1, ErrorMessage = "You need to create just 1 filter at least")]
        public ObservableCollection<ConditionFilterCreatingItem> SelectedFilters { get; init; }

        #endregion

        #region Propertyes
        /// <summary>
        /// Список фильтров, которые были закреплены за критерием до его редактирования
        /// </summary>
        private ReadOnlyCollection<Filter> AlreadyExistingFiltersDeepCopy { get; init; }
        /// <summary>
        /// Список фильтров, которые после осуществления операции редактирования критерия будут удалены из информационной базы
        /// </summary>
        private List<Filter> RemovableAfterEditFilters { get; init; }
        private List<Filter> NewCreatedFilters { get; init; }
        private ReadOnlyCollection<Filter> AlreadyExistingFilters { get; init; }
        #endregion

        #region

        public ICommand RemoveFilterCriterion { get; init; }
        public ICommand RemoveSelectedFilterFromList { get; init; }
        public ICommand CreateNewFilterInList { get; init; }
        public RelayCommand EditFilterCriterion { get; init; }
        public ICommand CloseDialog { get; init; }

        #endregion


        private EditFilterCriterionViewModel()
        {
            RemoveFilterCriterion = new RelayCommand(RemoveSelectedFilterCriterion);
            EditFilterCriterion = new RelayCommand(EditSelectedFilterCriterion,base.CanExecuteByValidation);
            base.PostValidationChange += EditFilterCriterion.OnCanExecuteChanged;

            RemoveSelectedFilterFromList = new RelayCommand(RemoveSelectedFilterFromListMethod);
            CreateNewFilterInList = new RelayCommand(NewFilterCreateClosure());

            CloseDialog = new RelayCommand(
                (p) =>
                {
                    CloseRequested.Invoke(this, new DialogCloseRequestedEventArgs(false));
                });

            RemovableAfterEditFilters = new List<Filter>();
            NewCreatedFilters = new List<Filter>();
        }

        public EditFilterCriterionViewModel(OperationService operationService, OutputContentService contentService): this()
        {
            OperationService = operationService;
            ContentService = contentService;

            FilterCriterion selectedFilterCriterion = ContentService.FilterCriterionContentService.SelectedFilterCritetion;

            Descriptyon = selectedFilterCriterion.Description;
            Tytle = selectedFilterCriterion.Tytle;



            AlreadyExistingFiltersDeepCopy = new ReadOnlyCollection<Filter>(selectedFilterCriterion.Filters.ToList());

            AlreadyExistingFilters = new ReadOnlyCollection<Filter>(AlreadyExistingFiltersDeepCopy.Select(filter => filter.DeepClone()).ToList());

            SelectedFilters = new ObservableCollection<ConditionFilterCreatingItem>(
                AlreadyExistingFilters.ToList()
                .Select(x => 
                new ConditionFilterCreatingItem((ConditionFilter)x, EditFilterCriterion)));

            foreach (var filterCreatingItem in SelectedFilters)
            {
                EditFilterCriterion.CanExecutePredicate += filterCreatingItem.CanExecuteByValidation;
            }

            SelectedFilters.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                Validate(nameof(SelectedFilters), SelectedFilters);
            };
        }

        private void RemoveSelectedFilterCriterion(object? parameter)
        {

            var mediaObjects = ContentService.MediaObjectContentService.GetUnloadedMediaObjects();
            var removeFilterCriterionOpetation = new RemoveFilterCriterionOperation(AlreadyExistingFiltersDeepCopy, mediaObjects,
                ContentService.FilterCriterionContentService.SelectedFilterCritetion.ID);

            if (OperationService.InvokeOperation<FilterCriterion>(removeFilterCriterionOpetation))
            {
                CloseRequested.Invoke(this,new DialogCloseRequestedEventArgs(true));
            }

        }

        private void EditSelectedFilterCriterion(object? parameter)
        {
            var mediaObjects = ContentService.MediaObjectContentService.GetUnloadedMediaObjects();
            IOperation editFilterCriterionOperation = new UpdateFilterCriterionOperation(
                Tytle,
                Descriptyon,
                ContentService.FilterCriterionContentService.SelectedFilterCritetion.ID
                ,RemovableAfterEditFilters
                , GetEditableOfExistingFilters()
                , NewCreatedFilters
                ,SelectedFilters.Select(x => x.conditionFilter)
                ,mediaObjects);

            if (OperationService.InvokeOperation<FilterCriterion>(editFilterCriterionOperation))
            {
                CloseRequested.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }

        }

        private void RemoveSelectedFilterFromListMethod(object? parameter)
        {

            if (parameter is ConditionFilterCreatingItem filterItem)
            {
                Filter filter = filterItem.conditionFilter;
                SelectedFilters.Remove(filterItem);

                filterItem.ConditionTytle = "removed anyway";
                EditFilterCriterion.CanExecutePredicate -= filterItem.CanExecuteByValidation;

                if (AlreadyExistingFilters.Contains(filter))
                    RemovableAfterEditFilters.Add(filter);
                else if(NewCreatedFilters.Contains(filter))
                    NewCreatedFilters.Remove(filter);
            }
        }

        private IEnumerable<Filter> GetEditableOfExistingFilters()
        {
            var resultList = new List<Filter>();

            foreach (var filter in SelectedFilters.Select(x => x.conditionFilter)) 
            {
                foreach (var existingFilter in AlreadyExistingFiltersDeepCopy)
                {
                    if(filter.IsMatchByID(existingFilter) & !filter.Equals(existingFilter))
                    {
                        resultList.Add(filter);
                        continue;
                    }
                }
            }

            return resultList;
        }

        private Action<object?> NewFilterCreateClosure()
        {
            uint newFilterStartNumber = 0;
            return (object? p) =>
            {
                string newFilterBaseTytle = $"New Filter {++newFilterStartNumber}";
                Filter newFilter = CreateFilterCommandMethod(newFilterBaseTytle);

                var newConditionFilterCreationItem = new ConditionFilterCreatingItem((ConditionFilter)newFilter, EditFilterCriterion);
                SelectedFilters.Add(newConditionFilterCreationItem);
                EditFilterCriterion.CanExecutePredicate += newConditionFilterCreationItem.CanExecuteByValidation;
                NewCreatedFilters.Add(newFilter);
            };
        }

        private Filter CreateFilterCommandMethod(string newFilterBaseTytle)
        {
            ConditionFilter newFilter = new ConditionFilter()
            {
                ID = Guid.NewGuid(),
                ConditionTytle = newFilterBaseTytle
            };
            return newFilter;
        }

    }
}
