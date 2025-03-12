using ModernSort.Commands;
using ModernSort.Stores.Catalog;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class EditRankingWindowViewModel : ViewModelValidateble, IDialogRequestClose
    {
        private OperationService OperationService {  get; init; }
        private CatalogStore CatalogStore { get; init; }
        private bool IconWasChange { get; set; }
        private string _categoryDescryption;

        [Required(ErrorMessage = "Descryption can not be Empthy")]
        [MaxLength(100, ErrorMessage = "Max Lenght for Descryption is 100 symbols")]
        public string CategoryDescryption
        {
            get { return _categoryDescryption; }
            set 
            { 
                _categoryDescryption = value;
                OnPropertyChenged(nameof(CategoryDescryption));
                Validate(nameof(CategoryDescryption), value);
            }
        }

        private string _categoryTytle;

        [Required(ErrorMessage ="Tytle can not be Empthy")]
        [MaxLength(30,ErrorMessage ="Max Lenght for Tytle is 30 symbols")]
        public string CategoryTytle
        {
            get { return _categoryTytle; }
            set
            {
                _categoryTytle = value;
                OnPropertyChenged(nameof(CategoryTytle));
                Validate(nameof(CategoryTytle),value);
            }
        }

        private string _categoryIconPath;

        public string CategoryIconPath
        {
            get { return _categoryIconPath; }
            set 
            {
                if (value is null || value == String.Empty) return;
                _categoryIconPath = value;
                OnPropertyChenged(nameof(CategoryIconPath));
                IconWasChange = (value.Equals(CatalogStore.SelectedRankingCategory.RankingIconPath)) ? false: true;
            }
        }

        public RelayCommand EditCategory { get; init; }
        public ICommand DeleteCategory { get; init; }
        public ICommand CloseDialog { get; init; }
        public ICommand EditCategoryImage { get; init; }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public EditRankingWindowViewModel()
        {
            EditCategory = new RelayCommand(EditCategoryMethod
                , base.CanExecuteByValidation);
            base.PostValidationChange += EditCategory.OnCanExecuteChanged;

            CloseDialog = new RelayCommand(
                (p) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                }
                );

            EditCategoryImage = new RelayCommand(
                (p) => 
                {
                    CategoryIconPath = ProjactIoWorker.FilePickerGetImagePathScalar();
                }
                );
            DeleteCategory = new RelayCommand(DeleteCategoryMethod);
        }

        public EditRankingWindowViewModel(OperationService operationService, CatalogStore catalogStore)
            : this()
        {
            OperationService = operationService;
            CatalogStore = catalogStore;

            CategoryTytle = CatalogStore.SelectedRankingCategory.Tytle;
            CategoryDescryption = CatalogStore.SelectedRankingCategory.Description;
            CategoryIconPath = CatalogStore.SelectedRankingCategory.RankingIconPath;
        }

        private void DeleteCategoryMethod(object? parameter)
        {
            IOperation deleteCategoryOperation = new DeleteRankingCategoryOperation();
            bool RemoveRankingWasSuccsesfullyCompleted = OperationService.InvokeOperation(deleteCategoryOperation);

            if (RemoveRankingWasSuccsesfullyCompleted)
            {
                CatalogStore.DropRankingSelection();
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }

        private void EditCategoryMethod(object? parameter)
        {
            IOperation operation = new UpdateRankingCategoryOperation(tytle:CategoryTytle,
                description:CategoryDescryption,
                newImageFileBasePath: CategoryIconPath, 
                iconWasChanged: IconWasChange);

            bool UpdateRankingWasSuccesfullyCompleted = OperationService.InvokeOperation(operation);

            if (UpdateRankingWasSuccesfullyCompleted)
            {
                CatalogStore.DropRankingSelection();
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }
    }
}
