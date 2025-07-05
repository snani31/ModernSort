using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using ModernSort.Services;

namespace ModernSort.ViewModel.Windows
{
    internal class EditRankingWindowViewModel : ViewModelValidateble, IDialogRequestClose
    {
        private OperationService OperationService {  get; init; }
        private OutputContentService ContentService { get; init; }
        private bool IconWasChange { get; set; }
        private string _categoryDescryption;

        [Required(ErrorMessage = "Description can not be empthy")]
        [MaxLength(500, ErrorMessage = "Max lenght for description is 500 symbols")]
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
        [MaxLength(60,ErrorMessage ="Max lenght for title is 60 symbols")]
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
                IconWasChange = (value.Equals(ContentService.SelectedRankingCategory.RankingIconPath)) ? false: true;
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

        public EditRankingWindowViewModel(OperationService operationService, OutputContentService contentService)
            : this()
        {
            ContentService = contentService;
            OperationService = operationService;

            CategoryTytle = contentService.SelectedRankingCategory.Tytle;
            CategoryDescryption = contentService.SelectedRankingCategory.Description;
            CategoryIconPath = contentService.SelectedRankingCategory.RankingIconPath;
        }

        private void DeleteCategoryMethod(object? parameter)
        {
            IOperation deleteCategoryOperation = new RemoveRankingCategoryOperation(ContentService.SelectedRankingCategory.ID);
            bool RemoveRankingWasSuccsesfullyCompleted = OperationService.InvokeOperation<RankingCategory>(deleteCategoryOperation);

            if (RemoveRankingWasSuccsesfullyCompleted)
            {
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }

        private void EditCategoryMethod(object? parameter)
        {
            IOperation operation = new UpdateRankingCategoryOperation(tytle:CategoryTytle,
                description:CategoryDescryption,
                newImageFileBasePath: CategoryIconPath, 
                iconWasChanged: IconWasChange,
                updatebeleRankingCategory: ContentService.SelectedRankingCategory);

            bool UpdateRankingWasSuccesfullyCompleted = OperationService.InvokeOperation<RankingCategory>(operation);

            if (UpdateRankingWasSuccesfullyCompleted)
            {
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }
    }
}
