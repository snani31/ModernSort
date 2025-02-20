using ModernSort.Commands;
using ModernSort.Services.Dialog;
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
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        private readonly ISerializer _serializer;
        private string _selectedImagePath;
        [Required(ErrorMessage = "You must select image")]
        public string SelectedImagePath 
        {
            get
            {
                return _selectedImagePath;
            }
            private set 
            {
                _selectedImagePath = value;
                OnPropertyChenged(nameof(SelectedImagePath));
                Validate(nameof(SelectedImagePath), value);
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
        public ICommand CloseDialogCommand {  get; init; }
        public ICommand SelectImageFile { get; init; }
        public RelayCommand MakeNewRankingCommand { get; init; }
        public AddNewRankingCategoryViewModel()
        {
            CloseDialogCommand = new RelayCommand((p) => CloseRequested?.Invoke(this,new DialogCloseRequestedEventArgs(false)));
            SelectImageFile = new RelayCommand((p) => SelectedImagePath = ProjactIoWorker.FilePickerGetImage());
            MakeNewRankingCommand = new RelayCommand(
                MakeNewRanking,
                base.CanExecuteByValidation);
            base.PostValidationChange += MakeNewRankingCommand.OnCanExecuteChanged;

        }

        public AddNewRankingCategoryViewModel(ISerializer serializer) : this()
        {
            _serializer = serializer;
        }

        private void MakeNewRanking(object? parameter)
        {
            try
            {
                Guid id = ProjactIoWorker.GetUniqGuid(ProjactIoWorker.UserResourcesDirrectoryPath 
                    + @$"\{ProjactIoWorker.PROJACT_GUIDS_FILE}");

                string newRankingDirrectoryPath = ProjactIoWorker.UserResourcesDirrectoryPath 
                    + @$"\{id.ToString()}";

                string newRankingIconPath = newRankingDirrectoryPath 
                    + @$"\{ProjactIoWorker.RANKING_CATEGORY_ICON_TYTLE}.jpg";

                Directory.CreateDirectory(newRankingDirrectoryPath);
                File.Copy(SelectedImagePath, newRankingIconPath);
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
                if (newRanking.Serialize(_serializer, ProjactIoWorker.UserResourcesDirrectoryPath 
                    + @"\RankingCategories.json")) 
                    CloseRequested?.Invoke(this,new DialogCloseRequestedEventArgs(true));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
