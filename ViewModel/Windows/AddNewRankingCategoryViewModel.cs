using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Static;
using RankingEntityes.IO_Entities.Enums;
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

        public AddNewRankingCategoryViewModel(ISerializer serializer) : this()
        {
            Serializer = serializer;
        }

        private void MakeNewRankingMethod(object? parameter)
        {
            try
            {
                Guid id = ProjactIoWorker.GetUniqGuid(ProjactIoWorker.UserResourcesDirrectoryPath 
                    + @$"\{ProjactIoWorker.PROJACT_GUIDS_FILE}");

                string newRankingDirrectoryPath = ProjactIoWorker.UserResourcesDirrectoryPath 
                    + @$"\{id.ToString()}";

                string newRankingIconPath = newRankingDirrectoryPath
                    + @$"\{ProjactIoWorker.RANKING_CATEGORY_ICON_TYTLE}{Path.GetExtension(SelectedRankingIconPath)}";

                Directory.CreateDirectory(newRankingDirrectoryPath);
                File.Copy(SelectedRankingIconPath, newRankingIconPath);
                Directory.CreateDirectory(newRankingDirrectoryPath + @"\Media");
                File.Create(newRankingDirrectoryPath + @"\MediaObjacts.json");
                RankingCategory newRanking = new RankingCategory()
                {
                    Description = NewRankingDescryption,
                    Tytle = NewRankingTytle,
                    ID = id,
                    RankingDirrectoryPath = newRankingDirrectoryPath,
                    RankingIconPath = newRankingIconPath
                };
                //Если процесс сериализации новой категории прошел успешно - закрыть текущее окно с параметров true
                if (newRanking.Serialize(Serializer,
                    ProjactIoWorker.UserResourcesDirrectoryPath + @"\RankingCategories.json",
                    FileMode.Append)) 
                    CloseRequested?.Invoke(this,new DialogCloseRequestedEventArgs(true));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
