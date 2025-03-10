using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Static;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ModernSort.ViewModel.Windows
{
    internal class EditRankingWindowViewModel : ViewModelValidateble, IDialogRequestClose
    {
		private ISerializer Serializer {  get; init; }
        private IDeserializer Deserializer { get; init; }

        private RankingCategory _selectedRankingCategory;

        private RankingCategory SelectedRankingCategory
        {
            get { return _selectedRankingCategory; }
            init { _selectedRankingCategory = value; }
        }
        private bool ImageWasChange { get; set; }
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
                ImageWasChange = (value.Equals(SelectedRankingCategory.RankingIconPath)) ? false: true;
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

        public EditRankingWindowViewModel(RankingCategory rankingCategory,
            ISerializer serializer, IDeserializer deserializer)
            : this() 
        {
            SelectedRankingCategory = rankingCategory;
            Serializer = serializer;
            Deserializer = deserializer;

            CategoryTytle = SelectedRankingCategory.Tytle;
            CategoryDescryption = SelectedRankingCategory.Description;
            CategoryIconPath = SelectedRankingCategory.RankingIconPath;
        }

        private void DeleteCategoryMethod(object? parameter)
        {

            string JsonfilePath = ProjactIoWorker.UserResourcesDirrectoryPath
                + "\\" + ProjactIoWorker.RANKING_CATEGORIES_JSON;
            string selectedRankingDirectoryPath = SelectedRankingCategory.RankingDirrectoryPath;

            var existingRankings = new IoCollection<RankingCategory>();
            existingRankings.Deserialize(Deserializer, JsonfilePath);

            existingRankings = new IoCollection<RankingCategory>
                (existingRankings.Where(x => !x.ID.Equals(SelectedRankingCategory.ID)));

            if (existingRankings.Serialize(Serializer, JsonfilePath,
                mode: FileMode.OpenOrCreate))
            {
                Directory.Delete(selectedRankingDirectoryPath, true);
                CloseRequested?.Invoke(this,new DialogCloseRequestedEventArgs(true));
            }

        }

        private void EditCategoryMethod(object? parameter)
        {

            string JsonfilePath = ProjactIoWorker.UserResourcesDirrectoryPath
                + "\\" + ProjactIoWorker.RANKING_CATEGORIES_JSON;
            string selectedRankingDirectoryPath = SelectedRankingCategory.RankingDirrectoryPath;

            var existingRankings = new IoCollection<RankingCategory>();
            existingRankings.Deserialize(Deserializer, JsonfilePath);

            int indexOfSelectedRanking = existingRankings.Select( (member,index) => (member, index))
                .First(x => x.member.ID.Equals(SelectedRankingCategory.ID)).index;

            existingRankings[indexOfSelectedRanking] = new RankingCategory()
            {
                ID = SelectedRankingCategory.ID,
                Description = CategoryDescryption,
                RankingDirrectoryPath = SelectedRankingCategory.RankingDirrectoryPath,
                Tytle = CategoryTytle,
                RankingIconPath = SelectedRankingCategory.RankingIconPath
            };

            if(ImageWasChange)
                File.Copy(CategoryIconPath, SelectedRankingCategory.RankingIconPath, overwrite: true);



            if (existingRankings.Serialize(Serializer, JsonfilePath,
                mode: FileMode.OpenOrCreate))
            {
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }
    }
}
