using ModernSort.Commands;
using ModernSort.Stores.Catalog;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class AddNewRankingCategoryViewModel: ViewModelValidateble, IDialogRequestClose
    {
        private CatalogStore CatalogStore {  get; init; }
        private OperationService OperationService { get; init; }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        private ISerializer Serializer {  get; init; }
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
        [Required(ErrorMessage = "Tytle is required field")]
        [MaxLength(30, ErrorMessage = "Tytle can not be bigger then 30 symbols")]
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

        [Required(ErrorMessage = $"Descryption is required field")]
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

        public AddNewRankingCategoryViewModel(OperationService operationService,CatalogStore rankingCatalogStore,ISerializer serializer) : this()
        {
            CatalogStore = rankingCatalogStore;
            Serializer = serializer;
            OperationService = operationService;
        }

        private void MakeNewRankingMethod(object? parameter)
        {
            IOperation operation = new CreateRankingCategoryOperation(NewRankingTytle,
                NewRankingDescryption,
                SelectedRankingIconPath);

            bool AddCategoryWasSucsessfullyComplete = OperationService.InvokeOperation(operation);

            if (AddCategoryWasSucsessfullyComplete)
            {
                CatalogStore.DropRankingSelection();
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }
    }
}
