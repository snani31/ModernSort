using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operation.Classes.FilterCriteron;
using ModernSort.Services.Operations;
using ModernSort.ViewModel.Items.FiltrationItems;
using RankingEntityes.Filters;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class CreateFilterCriterionViewModel : ViewModelValidateble, IDialogRequestClose
    {

        private OperationService OperationService { get; init; }

        private ObservableCollection<ConditionFilterCreatingItem> _createdFilters;
        [MinLength(1, ErrorMessage = "You need to create just 1 filter at least")]
        public ObservableCollection<ConditionFilterCreatingItem> CreatedFilters {
            get
            {
                return _createdFilters;
            }
            init 
            {
                _createdFilters = value;
                Validate(nameof(CreatedFilters),value);
            }
        }

        public RelayCommand CreateFilterCriterionCommand { get; init; }

        public ICommand RemoveFilterFromList { get; init; }
        public ICommand CreateFilter {  get; init; }
        public ICommand CloseDialog { get; init; }


        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        private string _title;

        [Required(ErrorMessage = "Title can not be empthy")]
        [MaxLength(60, ErrorMessage = "Max lenght for title is 60 symbols")]
        public string Tytle
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Validate(nameof(Tytle), value);
            }
        }

        private string _descriptyon;

        [Required(ErrorMessage = "Description can not be empthy")]
        [MaxLength(500, ErrorMessage = "Max lenght for description is 500 symbols")]
        public string Descriptyon
        {
            get
            {
                return _descriptyon;
            }
            set
            {
                _descriptyon = value;
                Validate(nameof(Descriptyon), value);
            }
        }


        private CreateFilterCriterionViewModel()
        {
            CloseDialog = new RelayCommand(
    (p) => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
            CreatedFilters = new ObservableCollection<ConditionFilterCreatingItem>();
            CreatedFilters.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                Validate(nameof(CreatedFilters), CreatedFilters);
            };

            RemoveFilterFromList = new RelayCommand(RemoveFilterFromListCommandMethod);

            CreateFilterCriterionCommand = new RelayCommand(CreateFilterCriterionCommandMethod, 
                base.CanExecuteByValidation);
            base.PostValidationChange += CreateFilterCriterionCommand.OnCanExecuteChanged;

            CreateFilter = new RelayCommand(NewFilterCreateClosure());
        }

        public CreateFilterCriterionViewModel(OperationService operationService): this()
        {
            OperationService = operationService;
        }

        private void CreateFilterCriterionCommandMethod(object? parameter)
        {
            ConditionFilterCriterion newConditionFilterCriterion = new ConditionFilterCriterion();
            newConditionFilterCriterion.Filters = CreatedFilters.Select(x => x.conditionFilter);
            newConditionFilterCriterion.Tytle = Tytle;
            newConditionFilterCriterion.Description = Descriptyon;

            IOperation createFilterCriterionOperation = new CreateFilterCriterionOperation(newConditionFilterCriterion);

            bool filterCriterionWasSuccesfullyCompleted =
                OperationService.InvokeOperation<ConditionFilterCriterion>(createFilterCriterionOperation);

            if (filterCriterionWasSuccesfullyCompleted)
            {
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }

        private Action<object?> NewFilterCreateClosure()
        {
            uint newFilterStartNumber = 0;
            return (object? p) =>
            {
                string newFilterBaseTytle = $"New filter {++newFilterStartNumber}";
                var newConditionFilterCreationItem = new ConditionFilterCreatingItem((ConditionFilter)CreateFilterCommandMethod(newFilterBaseTytle), CreateFilterCriterionCommand);
                CreatedFilters.Add(newConditionFilterCreationItem);
                CreateFilterCriterionCommand.CanExecutePredicate += newConditionFilterCreationItem.CanExecuteByValidation;
            };
        }

        private Filter CreateFilterCommandMethod(string newFilterBaseTytle)
        {
            ConditionFilter newFilter = new ConditionFilter() {
                ID = Guid.NewGuid(),
                ConditionTytle = newFilterBaseTytle };
            return newFilter;
        }

        private void RemoveFilterFromListCommandMethod(object? parameter)
        {
            if (parameter is ConditionFilterCreatingItem removableFilterItem and not null)
            {
                CreatedFilters.Remove(removableFilterItem);
                removableFilterItem.ConditionTytle = "removed anyway";
                CreateFilterCriterionCommand.CanExecutePredicate -= removableFilterItem.CanExecuteByValidation;
            }
        }
    }
}
