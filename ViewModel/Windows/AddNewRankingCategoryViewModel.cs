using ModernSort.Commands;
using ModernSort.Static;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class AddNewRankingCategoryViewModel: ViewModelValidateble
    {
        private string _selectedImagePath;
        [Required(ErrorMessage = "you must select picture")]
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



        public ICommand BackToPastWindow {  get; init; }
        public ICommand SelectImageFile { get; init; }
        public ActionCommand MakeNewRankingCommand { get; init; }
        public AddNewRankingCategoryViewModel() 
        {
            SelectImageFile = new ActionCommand(SelectImage);
            MakeNewRankingCommand = new ActionCommand(MakeNewRanking,CanExecuteByValidation);
            base.PostValidationChange += MakeNewRankingCommand.OnCanExecuteChanged;
        }

        private void SelectImage()
        {
            SelectedImagePath = ProjactIoWorker.FilePickerGetImage();
        }

        private void MakeNewRanking() // заглушка
        {
            MessageBox.Show($"Добавлена новая категория ранжира под именем {NewRankingTytle}");
        }

    }
}
