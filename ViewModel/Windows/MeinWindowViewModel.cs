using ModernSort.Commands;
using System.Windows.Input;
using System.Windows;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.Collections.ObjectModel;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Windows;
using ModernSort.Services.Dialog;
using RankingEntityes.IO_Entities.Classes;
using ModernSort.Stores.Catalog;
using ModernSort.Services.Operations;
using ModernSort.Services;
using ModernSort.Services.UITheme;

namespace ModernSort.ViewModel
{
    class MeinWindowViewModel : ViewModelBase, IDialogRequestClose
    {
        public ICommand OpenNewRankingWindow { get; init; }
        public ICommand OpenEditRankingWindow { get; init; }
        public ICommand OpenSelectedRankingWindow { get; init; }
        public ICommand CloseApplication { get; }
        public ICommand ChangeUITheme { get; init; }
        private IDialogService DialogService { get; init; }
        private OperationService OperationService { get; init; }
        private OutputContentService ContentService { get; init; }

        private UIThemeService ThemeService { get; init; }

        private CatalogStore CatalogStore { get; init; }

        private IoCollection<RankingCategory> _rankingCategories;

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ObservableCollection<RankingCategoryItemViewModel> RankingCategoriesItems
        {
            get
            { 
                return ParseIoToCollection(_rankingCategories);
            } 
        }

        public MeinWindowViewModel(OutputContentService outputContentService,OperationService operationService, 
            CatalogStore catalogService,IDialogService dialogService,UIThemeService themeService)
        {
            DialogService = dialogService;
            CatalogStore = catalogService;
            OperationService = operationService;
            ContentService = outputContentService;
            ThemeService = themeService;

            OpenEditRankingWindow = new RelayCommand(OpenEditRankingCategoryWindow);
            OpenNewRankingWindow = new RelayCommand(GetOpenNewRankingWindow);
            OpenSelectedRankingWindow = new RelayCommand(OpenSelectedRankingCategoryWindow);

            _rankingCategories = ContentService.GetUnloadedRankingCategories();

            ChangeUITheme = new RelayCommand(
                (p) =>
                {
                    ThemeService.ChangeTheme();
                }
                );

            CloseApplication = new RelayCommand(
                (a) => 
                {
                   Environment.Exit(0);
                });

        }

        private void GetOpenNewRankingWindow(object? parameter)
        {
            var addNewRankingCategoryViewModel = new AddNewRankingCategoryViewModel(OperationService,ContentService);

            bool? result = DialogService.ShowDialog(addNewRankingCategoryViewModel);

            if (result == true) // если добавление новой категории прошло успешно
            {
                _rankingCategories = ContentService.GetUnloadedRankingCategories();
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
        private void OpenSelectedRankingCategoryWindow(object? parameter)
        {
            if (parameter is RankingCategoryItemViewModel selectedRankingItemVM)
            {
                RankingCategory selectedCategory = _rankingCategories.First(x => x.ID.ToString().Equals(selectedRankingItemVM.ID));

                ContentService.SelectRankingCategory(selectedCategory);

                var selectedRankingCategoryViewModel = new SelectedRankingCategoryViewModel(ContentService,CatalogStore,OperationService,
                    selectedCategory, DialogService);

                if (DialogService.ShowDialog(selectedRankingCategoryViewModel) ?? false)
                {
                    MessageBox.Show("adasd");
                }
            }
           
        }

        private void OpenEditRankingCategoryWindow(object? parameter)
        {
            if (parameter is not null and RankingCategoryItemViewModel rankingItem)
            {
                RankingCategory selectedCategory = _rankingCategories.First(x => x.ID.ToString() == rankingItem.ID);

                ContentService.SelectRankingCategory(selectedCategory);

                var addNewRankingCategoryViewModel = new EditRankingWindowViewModel(OperationService,ContentService);
                bool? ExistingRankingCategoriesListWasChanged = DialogService.ShowDialog(addNewRankingCategoryViewModel);

                if(ExistingRankingCategoriesListWasChanged ?? false)
                {
                    _rankingCategories = ContentService.GetUnloadedRankingCategories();
                    OnPropertyChenged(nameof(RankingCategoriesItems));
                }

            }
            
        }
    }
}
