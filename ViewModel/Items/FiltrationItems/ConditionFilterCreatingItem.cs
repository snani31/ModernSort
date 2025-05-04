using ModernSort.Commands;
using RankingEntityes.Filters;
using System.ComponentModel.DataAnnotations;

namespace ModernSort.ViewModel.Items.FiltrationItems
{
    class ConditionFilterCreatingItem: ViewModelValidateble
    {
        public ConditionFilter conditionFilter { get; init; }

        public Func<bool> CanExecuteByValidation
        {
            get
            {
                return base.CanExecuteByValidation;
            }
        }

        [Required(ErrorMessage = "Tytle can not be Empthy")]
        public string ConditionTytle
        { 
            get 
            { 
                return conditionFilter.ConditionTytle;
            }
            set
            {
                conditionFilter.ConditionTytle = value;
                Validate(nameof(ConditionTytle), value);
            }
        }

        public ConditionFilterCreatingItem(ConditionFilter filter)
        {
            conditionFilter = filter;
        }


        public ConditionFilterCreatingItem(ConditionFilter filter, RelayCommand relayCommand) : this(filter)
        {
            base.PostValidationChange += relayCommand.OnCanExecuteChanged;
        }
    }
}
