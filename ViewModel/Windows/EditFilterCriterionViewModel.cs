using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operation;
using ModernSort.Services.Operations;
using Newtonsoft.Json;
using RankingEntityes.Filters;
using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
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

        [MaxLength(20, ErrorMessage = $"Tytle can not be bigger then 20 symbols")]
        [Required(ErrorMessage = "You neet to write some Tytle")]

        private string _title;
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
        [MaxLength(100, ErrorMessage = $"Description can not be bigger then 100 symbols")]
        [Required(ErrorMessage = "You neet to write some Description")]
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

        public ObservableCollection<Filter> SelectedFilters { get; init; }

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
        public ICommand EditFilterCriterion { get; init; }
        public ICommand CloseDialog { get; init; }

        #endregion


        private EditFilterCriterionViewModel()
        {
            RemoveFilterCriterion = new RelayCommand(RemoveSelectedFilterCriterion);
            EditFilterCriterion = new RelayCommand(EditSelectedFilterCriterion);
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

            SelectedFilters = new ObservableCollection<Filter>(AlreadyExistingFilters.ToList());
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
                ,SelectedFilters
                ,mediaObjects);

            if (OperationService.InvokeOperation<FilterCriterion>(editFilterCriterionOperation))
            {
                CloseRequested.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }

        }

        private void RemoveSelectedFilterFromListMethod(object? parameter)
        {

            if (parameter is Filter filter)
            {
                SelectedFilters.Remove(filter);

                if (AlreadyExistingFilters.Contains(filter))
                    RemovableAfterEditFilters.Add(filter);
                else if(NewCreatedFilters.Contains(filter))
                    NewCreatedFilters.Remove(filter);
            }
        }

        private IEnumerable<Filter> GetEditableOfExistingFilters()
        {
            var resultList = new List<Filter>();

            foreach (var filter in SelectedFilters) 
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
                SelectedFilters.Add(newFilter);
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
