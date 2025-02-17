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
using ModernSort.View.Windows;
using ModernSort.ViewModel.Windows;
using ModernSort.Static;
using System.Windows.Media.Animation;
using ModernSort.Services.Dialog;
using RankingEntityes.IO_Entities.Classes;

namespace ModernSort.ViewModel
{
    class MeinWindowViewModel : ViewModelBase
    {
        public ICommand OpenNewRankingWindow { get; init; }
        private IDialogService _DialogService { get; init; }
        private IDeserializer _Deserializer {  get; init; }
        private ISerializer _Serializer { get; init; }

        private RankingCategoryItemViewModel _selectedRankingCategory;
        private IoCollection<RankingCategory> _rankingCategories;
        public RankingCategoryItemViewModel SelectedRankingCategory 
        { 
            get => _selectedRankingCategory; 
            set
            {
                if (value is not null)
                {
                    _selectedRankingCategory = value;
                    OpenSelectedRankingCategoryWindow();
                }
            }
        }
        public ObservableCollection<RankingCategoryItemViewModel> RankingCategoriesItems
        {
            get
            { 
                return ParseIoToCollection(_rankingCategories);
            } 
        }

        public MeinWindowViewModel(IDeserializer deserializer,ISerializer serializer,IDialogService dialogService)
        {
            _DialogService = dialogService;
            OpenNewRankingWindow = new ActionCommand(GetOpenNewWindow);
            _Deserializer = deserializer;
            _Serializer = serializer;

            _rankingCategories = new IoCollection<RankingCategory>();
           _rankingCategories.Deserialize(_Deserializer, ProjactIoWorker.UserResourcesDirrectoryPath + @"\RankingCategories.json");

            CloseApplicationCommand = new ApplicationCloseCommand();
            CollapselicationCommand = new CollapseApplicationCommand();
        }

        private void GetOpenNewWindow()
        {
            var addNewRankingCategoryViewModel = new AddNewRankingCategoryViewModel(_Serializer);

            bool? result = _DialogService.ShowDialog(addNewRankingCategoryViewModel);

            if (result == true) // если добавление новой категории прошло успешно
            {
                _rankingCategories.Deserialize(_Deserializer, ProjactIoWorker.UserResourcesDirrectoryPath + @"\RankingCategories.json");
                OnPropertyChenged(nameof(RankingCategoriesItems));
            }
            
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
        /// <summary>
        /// Используется для открытия окна с заданной выбранной категорией ранжира
        /// </summary>
        private void OpenSelectedRankingCategoryWindow()
        {
            RankingCategory selectedmember = _rankingCategories.First(x => x.ID.ToString() == SelectedRankingCategory.ID);
            var addNewRankingCategoryViewModel = new SelectedRankingCategoryViewModel(selectedmember, _DialogService,_Serializer,_Deserializer);
            bool? result = _DialogService.ShowDialog(addNewRankingCategoryViewModel);
        }
    }
}
