using ModernSort.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.Collections.ObjectModel;
using ModernSort.ViewModel.Items;
using System.Linq;
using RankingEntityes.IO_Entities.Interfaces;

namespace ModernSort.ViewModel
{
    class MeinWindowViewModel : ViewModelBase
    {
        private IDeserializer _Deserializer {  get; init; }
        private ISerializer _Serializer { get; init; }
        private IoCollection<RankingCategory> _rankingCategories;
        public ObservableCollection<RankingCategoryItemViewModel> RankingCategoriesItems
        { 
            get
            { 
                return ParseIoToCollection(_rankingCategories);
            } 
        }

        public MeinWindowViewModel(IDeserializer deserializer,ISerializer serializer)
        {
            _Deserializer = deserializer;
            _Serializer = serializer;

            _rankingCategories = new IoCollection<RankingCategory>();
            _rankingCategories.Deserialize(deserializer, @"RankingCategories.json");

            CloseApplicationCommand = new ApplicationCloseCommand();
            CollapselicationCommand = new CollapseApplicationCommand();
        }
        public ICommand CloseApplicationCommand { get; }
        public ICommand CollapselicationCommand { get; }

        /// <summary>
        /// Метод преобразует IoCollection в ObservableCollection
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private ObservableCollection<RankingCategoryItemViewModel> ParseIoToCollection(IEnumerable<RankingCategory> list)
        {
            return new ObservableCollection<RankingCategoryItemViewModel>(list.Select(x => new RankingCategoryItemViewModel(x)));
        }
    }
}
