using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using ModernSort.Services;

namespace ModernSort.ViewModel.Windows
{
    internal class AddNewRankingCategoryViewModel: ViewModelValidateble, IDialogRequestClose
    {
        private OperationService OperationService { get; init; }
        private OutputContentService ContentService { get; init; }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        private string _selectedRankingIconPath;
        [Required(ErrorMessage = "You must select image")]
        public string SelectedRankingIconPath 
        {
            get
            {
                return _selectedRankingIconPath;
            }
            private set 
            {
                _selectedRankingIconPath = value;
                OnPropertyChenged(nameof(SelectedRankingIconPath));
                Validate(nameof(SelectedRankingIconPath), value);
            } 
        }
        private string _newRankingTytle;
        [Required(ErrorMessage = "Title can not be Empthy")]
        [MaxLength(30, ErrorMessage = "Max Lenght for Title is 30 symbols")]
        public string NewRankingTytle
        {
            get { return _newRankingTytle; }
            set 
            { 
                _newRankingTytle = value;
                Validate(nameof(NewRankingTytle), value);
            }
        }

        private string _newRankingDescryption;

        [Required(ErrorMessage = "Description can not be Empthy")]
        [MaxLength(100, ErrorMessage = "Max Lenght for Description is 100 symbols")]
        public string NewRankingDescryption
        {
            get { return _newRankingDescryption; }
            set 
            { 
                _newRankingDescryption = value;
                Validate(nameof(NewRankingDescryption), value);
            }
        }
        public ICommand CloseDialog {  get; init; }
        public ICommand SelectImageFile { get; init; }
        public RelayCommand MakeNewRanking { get; init; }
        public AddNewRankingCategoryViewModel()
        {
            CloseDialog = new RelayCommand((p) => CloseRequested?.Invoke(this,new DialogCloseRequestedEventArgs(false)));

            SelectImageFile = new RelayCommand(
                (p) =>
                {
                    string? selectedImagePath = ProjactIoWorker.FilePickerGetImagePathScalar();
                    if (selectedImagePath is not null && selectedImagePath != String.Empty)
                    SelectedRankingIconPath = selectedImagePath;
                }
                );

            MakeNewRanking = new RelayCommand(
                MakeNewRankingMethod,
                base.CanExecuteByValidation);
            base.PostValidationChange += MakeNewRanking.OnCanExecuteChanged;

        }

        public AddNewRankingCategoryViewModel(OperationService operationService,OutputContentService contentService) : this()
        {
            OperationService = operationService;
            ContentService = contentService;
        }

        private void MakeNewRankingMethod(object? parameter)
        {
            IOperation operation = new CreateRankingCategoryOperation(NewRankingTytle,
                NewRankingDescryption,
                SelectedRankingIconPath);

            bool AddCategoryWasSucsessfullyComplete = OperationService.InvokeOperation<RankingCategory>(operation);

            if (AddCategoryWasSucsessfullyComplete)
            {
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }
    }
}
