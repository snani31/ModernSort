using ModernSort.Services.Dialog;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.ViewModel.Windows
{
    internal class EditRankingWindowViewModel : ViewModelValidateble, IDialogRequestClose
    {
		private ISerializer _serializer;

		private ISerializer Serializer
        {
			get { return _serializer; }
			init { _serializer = value; }
		}

		private RankingCategory _selectedRankingCategory;

        private RankingCategory SelectedRankingCategory
        {
            get { return _selectedRankingCategory; }
            init { _selectedRankingCategory = value; }
        }

        private string _categoryDescryption;

        public string CategoryDescryption
        {
            get { return _categoryDescryption; }
            set 
            { 
                _categoryDescryption = value;
                OnPropertyChenged(nameof(CategoryDescryption));
            }
        }

        private string _categoryTytle;

        public string CategoryTytle
        {
            get { return _categoryTytle; }
            set
            {
                _categoryTytle = value;
                OnPropertyChenged(nameof(CategoryTytle));
            }
        }

        private string _categoryIconPath;

        public string CategoryIconPath
        {
            get { return _categoryIconPath; }
            set 
            { 
                _categoryIconPath = value;
                OnPropertyChenged(nameof(CategoryIconPath));
            }
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public EditRankingWindowViewModel(RankingCategory rankingCategory,ISerializer serializer)
        {
            SelectedRankingCategory = rankingCategory;
            Serializer = serializer;

            CategoryTytle = SelectedRankingCategory.Tytle;
            CategoryDescryption = SelectedRankingCategory.Description;
            CategoryIconPath = SelectedRankingCategory.RankingIconPath;
        }



	}
}
