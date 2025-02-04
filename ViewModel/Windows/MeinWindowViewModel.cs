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
        private IDeserializer _JsonDeserializer;
        private ISerializer _jsonSerializer;
        private IoCollection<RankingCategory> _rankingCategories;
        public ObservableCollection<RankingCategoryItemViewModel> RankingCategoriesItems
        { 
            get
            { 
                return ParseListToCollection(_rankingCategories);
            } 
        }

        public MeinWindowViewModel(IDeserializer deserializer,ISerializer serializer)
        {
            _JsonDeserializer = deserializer;
            _jsonSerializer = serializer;
            _rankingCategories = deserializer.DeserializeList<RankingCategory>(@"1.json"); ////// Исправить Рефакторинг интерфейсов IDeSerializable!!!
            //_rankingCategories = new List<RankingCategory>()
            //{
            //    new RankingCategory() {ID = 1,Tytle = "Название1",Description = "Описание1",RankingDirrectoryPath = "Путь1", RankingIconPath = "UserResources\\RankingCategory_1\\Ranking_Icon.jpg"},
            //    new RankingCategory() {ID = 2,Tytle = "Название2",Description = "Описание2",RankingDirrectoryPath = "Путь2", RankingIconPath = "UserResources\\RankingCategory_2\\Ranking_Icon.jpg"},
            //    new RankingCategory() {ID = 3,Tytle = "Название3",Description = "Описание3",RankingDirrectoryPath = "Путь3",RankingIconPath = "UserResources\\RankingCategory_3\\Ranking_Icon.jpg"},
            //    new RankingCategory() {ID = 4, Tytle = "Название4",Description = "Описание4", RankingDirrectoryPath = "Путь4",RankingIconPath = "UserResources\\RankingCategory_4\\Ranking_Icon.jpg" }
            //};
            CloseApplicationCommand = new ApplicationCloseCommand();
            CollapselicationCommand = new CollapseApplicationCommand();
        }
        public ICommand CloseApplicationCommand { get; }
        public ICommand CollapselicationCommand { get; }

        private ObservableCollection<RankingCategoryItemViewModel> ParseListToCollection(IEnumerable<RankingCategory> list)
        {
            return new ObservableCollection<RankingCategoryItemViewModel>(list.Select(x => new RankingCategoryItemViewModel(x)));
        }
    }
}
