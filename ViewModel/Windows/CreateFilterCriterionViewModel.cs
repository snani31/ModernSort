using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operation.Classes.FilterCriteron;
using ModernSort.Services.Operations;
using ModernSort.ViewModel.Items;
using RankingEntityes.Filters;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class CreateFilterCriterionViewModel : ViewModelValidateble, IDialogRequestClose
    {

        private OperationService OperationService { get; init; }

        private ObservableCollection<Filter> _createdFilters;
        [MinLength(1, ErrorMessage = "You need to create just 1 filter at Least")]
        public ObservableCollection<Filter> CreatedFilters {
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


        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        private string _title;

        [MaxLength(20, ErrorMessage = $"Tytle can not be bigger then 20 symbols")]
        [Required(ErrorMessage = "You neet to write some Tytle")]
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
                Validate(nameof(Descriptyon), value);
            }
        }


        private CreateFilterCriterionViewModel()
        {
            CreatedFilters = new ObservableCollection<Filter>();
            CreatedFilters.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                Validate(nameof(CreatedFilters), CreatedFilters);
            };

            RemoveFilterFromList = new RelayCommand(RemoveFilterFromListCommandMethod);

            CreateFilterCriterionCommand = new RelayCommand(CreateFilterCriterionCommandMethod, 
                base.CanExecuteByValidation);
            base.PostValidationChange += CreateFilterCriterionCommand.OnCanExecuteChanged;

            CreateFilter = new RelayCommand( 
                (p)
                => 
                {
                    uint newFilterStartNumber = 0;
                    string newFilterBaseTytle = $"New Filter {++newFilterStartNumber}";
                    CreatedFilters.Add(CreateFilterCommandMethod(newFilterBaseTytle));
                });
        }

        public CreateFilterCriterionViewModel(OperationService operationService): this()
        {
            OperationService = operationService;
        }

        private void CreateFilterCriterionCommandMethod(object? parameter)
        {
            ConditionFilterCriterion newConditionFilterCriterion = new ConditionFilterCriterion();
            newConditionFilterCriterion.Filters = CreatedFilters;

            IOperation createFilterCriterionOperation = new CreateFilterCriterionOperation(newConditionFilterCriterion, Tytle, Descriptyon);

            bool filterCriterionWasSuccesfullyCompleted =
                OperationService.InvokeOperation<ConditionFilterCriterion>(createFilterCriterionOperation);

            if (filterCriterionWasSuccesfullyCompleted)
            {
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }

        private Filter CreateFilterCommandMethod(string newFilterBaseTytle)
        {
            ConditionFilter newFilter = new ConditionFilter() { ConditionTytle = newFilterBaseTytle };
            return newFilter;
        }

        private void RemoveFilterFromListCommandMethod(object? parameter)
        {
            if (parameter is Filter removableFilter and not null)
            {
                CreatedFilters.Remove(removableFilter);
            }
        }
    }
}
