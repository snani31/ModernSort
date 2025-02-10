using ModernSort.Commands;
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
    internal class AddNewRankingCategoryViewModel: ViewModelValidateble
    {
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;
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



        public ICommand BackToPastWindow {  get; init; }
        public ICommand SelectImageFile { get; init; }
        public ActionCommand MakeNewRankingCommand { get; init; }
        public AddNewRankingCategoryViewModel()
        {
            SelectImageFile = new ActionCommand(SelectImage);
            MakeNewRankingCommand = new ActionCommand(MakeNewRanking, base.CanExecuteByValidation);
            base.PostValidationChange += MakeNewRankingCommand.OnCanExecuteChanged;
        }
        public AddNewRankingCategoryViewModel(ISerializer serializer,IDeserializer deserializer) : this()
        {
            _serializer = serializer;
            _deserializer = deserializer;
        }

        private void SelectImage()
        {
            SelectedImagePath = ProjactIoWorker.FilePickerGetImage();
        }

        private void MakeNewRanking()
        {
            int id = 2; ////////////////////////
            string newRankingDirrectoryPath = ProjactIoWorker.UserResourcesDirrectoryPath + @$"\{id}";
            string newRankingIconPath = newRankingDirrectoryPath + @$"\{ProjactIoWorker.RANKING_CATEGORY_ICON_TYTLE}.jpg";
            Directory.CreateDirectory(newRankingDirrectoryPath);
            File.Copy(SelectedImagePath, newRankingIconPath);
            Directory.CreateDirectory(newRankingDirrectoryPath + @"\Media");
            RankingCategory newRanking = new RankingCategory()
            {
                Description = NewRankingDescryption,
                Tytle = NewRankingTytle,
                ID = id,
                RankingDirrectoryPath = newRankingDirrectoryPath,
                RankingIconPath = newRankingIconPath
            };
            newRanking.Serialize(_serializer, ProjactIoWorker.UserResourcesDirrectoryPath + @"\RankingCategories.json");
        }

    }
}
