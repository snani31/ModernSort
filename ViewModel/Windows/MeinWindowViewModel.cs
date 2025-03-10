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
using System.IO;
using System.Reflection.Metadata;

namespace ModernSort.ViewModel
{
    class MeinWindowViewModel : ViewModelBase, IDialogRequestClose
    {
        public ICommand OpenNewRankingWindow { get; init; }
        public ICommand OpenEditRankingWindow { get; init; }
        public ICommand OpenSelectedRankingWindow { get; init; }
        private IDialogService DialogService { get; init; }
        private IDeserializer Deserializer {  get; init; }
        private ISerializer Serializer { get; init; }

        private RankingCategoryItemViewModel _selectedRankingCategory;
        private IoCollection<RankingCategory> _rankingCategories;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand CloseApplication { get; }

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
            DialogService = dialogService;
            OpenEditRankingWindow = new RelayCommand(OpenEditRankingCategoryWindow);
            OpenNewRankingWindow = new RelayCommand(GetOpenNewRankingWindow);
            OpenSelectedRankingWindow = new RelayCommand(OpenSelectedRankingCategoryWindow2);
            Deserializer = deserializer;
            Serializer = serializer;

            _rankingCategories = new IoCollection<RankingCategory>();
           _rankingCategories.Deserialize(Deserializer, ProjactIoWorker.UserResourcesDirrectoryPath + @"\RankingCategories.json");

            CloseApplication = new RelayCommand(
                (a) => 
                {
                   Environment.Exit(0);
                });

        }

        private void GetOpenNewRankingWindow(object? parameter)
        {
            var addNewRankingCategoryViewModel = new AddNewRankingCategoryViewModel(Serializer);

            bool? result = DialogService.ShowDialog(addNewRankingCategoryViewModel);

            if (result == true) // если добавление новой категории прошло успешно
            {
                _rankingCategories.Deserialize(Deserializer, ProjactIoWorker.UserResourcesDirrectoryPath + @"\RankingCategories.json");
                OnPropertyChenged(nameof(RankingCategoriesItems));
            }
            
        }

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
            RankingCategory selectedCategory = _rankingCategories.First(x => x.ID.ToString() == SelectedRankingCategory.ID);
            var addNewRankingCategoryViewModel = new SelectedRankingCategoryViewModel(selectedCategory, DialogService,Serializer,Deserializer);
            bool? result = DialogService.ShowDialog(addNewRankingCategoryViewModel);
        }

        private void OpenSelectedRankingCategoryWindow2(object? parameter)
        {
            if (parameter is RankingCategoryItemViewModel selectedRankingItemVM)
            {
                RankingCategory selectedCategory = _rankingCategories.First(x => x.ID.ToString().Equals(selectedRankingItemVM.ID));
                var addNewRankingCategoryViewModel = new SelectedRankingCategoryViewModel(selectedCategory, DialogService, Serializer, Deserializer);
                bool? result = DialogService.ShowDialog(addNewRankingCategoryViewModel);
            }
           
        }

        private void OpenEditRankingCategoryWindow(object? parameter)
        {
            if (parameter is not null and RankingCategoryItemViewModel rankingItem)
            {
                RankingCategory selectedCategory = _rankingCategories.First(x => x.ID.ToString() == rankingItem.ID);
                var addNewRankingCategoryViewModel = new EditRankingWindowViewModel(selectedCategory,Serializer,Deserializer);
                bool? result = DialogService.ShowDialog(addNewRankingCategoryViewModel);

                if(result ?? false)
                {
                    _rankingCategories.Deserialize(Deserializer, ProjactIoWorker.UserResourcesDirrectoryPath + @"\RankingCategories.json");
                    OnPropertyChenged(nameof(RankingCategoriesItems));
                }

            }
            
        }
    }
}
