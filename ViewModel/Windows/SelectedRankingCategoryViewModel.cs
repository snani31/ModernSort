using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Services.Searching;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
using ModernSort.Stores.Navigation;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Pages;
using RankingEntityes.Filters;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class SelectedRankingCategoryViewModel : ViewModelBase, IDialogRequestClose
    {
        private string _searchMediaObjectQuery;
        public string SearchMediaObjectQuery
        {
            get => _searchMediaObjectQuery;
            set
            {
                _searchMediaObjectQuery = value;
                OnPropertyChenged(nameof(SearchMediaObjectQuery));
            }
        }
        public bool IsFiltrationPageSelected {  get; set; }
        public bool IsMediaObjectPageSelected { get; set; }
        public bool MediaObjectsListIsNotEmpthy 
        {
            get
            {
                return (MediaObjacts?.Count > 0);
            }
        }

        public bool FilterCriterionsListIsNotEmpthy
        {
            get
            {
                return (FilterCriterions?.Count > 0);
            }
        }

        private NavigationStore NavigationStore {  get; init; }
        public ViewModelBase? CurrentPage
        {
            get
            {
                return NavigationStore.CurrentViewModel;
            }
        }

        private ObservableCollection<MediaObjectItemViewModel> _mediaObjacts;
        public ObservableCollection<MediaObjectItemViewModel> MediaObjacts
        {
            set
            {
                _mediaObjacts = value;
                OnPropertyChenged(nameof(MediaObjacts));
                OnPropertyChenged(nameof(MediaObjectsListIsNotEmpthy));           
            }
            get
            {
                return _mediaObjacts;
            }
        }

        private List<FilterCriterion> _fitlerCriterions;
        private List<FilterCriterion> FilterCriterions
        {
            get 
            { 
                return _fitlerCriterions; 
            }
            set 
            { 
                _fitlerCriterions = value;
                OnPropertyChenged(nameof(FilterCriterionsListIsNotEmpthy));
            }
        }


        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand SetNotSearchedMediaObjectsState{ get; init; }
        public ICommand SearchMediaObjact { get; init; }
        public ICommand SetMediaObjectPageValue { get; init; }
        public ICommand SetFiltrationPageValue { get; init; }
        public ICommand CreateMediaObjectWindowOpen { get; init; }
        public ICommand CreateFilterCriterionWindowOpen { get; init; }
        public RelayCommand EditMediaObjectWindowOpen { get; init; }
        public ICommand OpenMediaObjectPage { get; init; }
        public ICommand CopyAllSelectedImages { get; init; }
        public ICommand CloseDialog { get; init; }

        public  SearchingService<MediaObject> SearchingService { get; }

        public OutputContentService ContentService { get; }

        private CatalogStore CatalogStore {  get; init; }

        private OperationService OperationService { get; init; }

        private IDialogService DialogService {  get; init; }

        private FiltrationPageViewModel? CurrentFiltrationPageViewModel { get; set; }
        private SelectedMediaObjectPageViewModel? CurrentMediaObjectPageViewModel {  get; set; }

        private SelectedMediaObjectPageViewModel? _currentMediaObjectPagePresenter;

        public SelectedMediaObjectPageViewModel? CurrentMediaObjectPagePresenter
        {
            get 
            { 
                return _currentMediaObjectPagePresenter;
            }
            private set 
            {
                _currentMediaObjectPagePresenter = value;
                OnPropertyChenged(nameof(CurrentMediaObjectPagePresenter)); 
            }
        }

        public SelectedRankingCategoryViewModel() 
        {
            
            OpenMediaObjectPage = new RelayCommand(OpenMediaObjectPageView);

            NavigationStore = new NavigationStore();
            NavigationStore.CurrentViewModelChanged += () => {OnPropertyChenged(nameof(CurrentPage));};

            SetMediaObjectPageValue = new RelayCommand(
                (p) => 
                {
                    if (ContentService.MediaObjectContentService.SelectedMediaObject is null)
                    {
                        OpenMediaObjectPageView(MediaObjacts.First());
                    }
                    NavigationStore.CurrentViewModel = CurrentMediaObjectPageViewModel;
                });

            SetFiltrationPageValue = new RelayCommand(
                (p) =>
                {
                    NavigationStore.CurrentViewModel = CurrentFiltrationPageViewModel;
                });

            SearchMediaObjact = new RelayCommand(SearchMediaObjectMethod);

            SetNotSearchedMediaObjectsState = new RelayCommand(
                (p) =>
                {
                    MediaObjacts = new ObservableCollection<MediaObjectItemViewModel>(
                        SearchingService.SearchElements
                        .Select(
                            x => new MediaObjectItemViewModel(x, CatalogStore.MediaFilesCatalogPath)
                            )
                            );
                    SearchMediaObjectQuery = String.Empty;
                });

            CopyAllSelectedImages = new RelayCommand(
                (p) =>
                {
                    ProjactIoWorker.CopySelectedFilesToDestinationFolder(CatalogStore.MediaFilesCatalogPath, 
                        MediaObjacts.SelectMany(x => x.Paths).ToList(),destinationFolderName: "Your images");
                });

        }

        public SelectedRankingCategoryViewModel(OutputContentService contentService,CatalogStore catalogStore,OperationService operationService,RankingCategory selectedRankingCategory,
            IDialogService dialogService): this()
        {
            DialogService = dialogService;
            OperationService = operationService;
            ContentService = contentService;
            CatalogStore = catalogStore;

            ContentService.MediaObjectContentService.OnMediaObjectSelectedChange += (object? parameter) =>
            {
                EditMediaObjectWindowOpen?.OnCanExecuteChanged();
            };

            CreateMediaObjectWindowOpen = new RelayCommand(
                (p) => 
                {
                    var viewModel = new CreateMediaObjectViewModel(OperationService,ContentService);

                    if (DialogService.ShowDialog(viewModel) ?? false)
                    {
                        InitStartEntities();
                    }
                });

            CreateFilterCriterionWindowOpen = new RelayCommand(
                (p) =>
                {
                    var viewModel = new CreateFilterCriterionViewModel(OperationService);
                    if (DialogService.ShowDialog(viewModel) ?? false)
                    {
                        InitStartEntities();
                    }
                });

            EditMediaObjectWindowOpen = new RelayCommand(
                (p) =>
                {
                    EditMediaObjectViewModel editMediaObjectViewModel = new EditMediaObjectViewModel(OperationService,ContentService);
                    if (DialogService.ShowDialog(editMediaObjectViewModel) ?? false)
                    {
                        InitStartEntities();
                    }
                },
                () => 
                {
                    if (ContentService.MediaObjectContentService.SelectedMediaObject is not null)
                    {
                        return true;
                    }
                    return false;
                }
                );

            CloseDialog = new RelayCommand(
                (p) =>
                {
                    ContentService.DropSelectionOfRankingCategory();
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                });

            SearchingService = new SearchingService<MediaObject>(minLenghtForSearchQueryWordsDecrission:3);

            InitStartEntities();
        }

        private void InitStartEntities()
        {
            MediaObjacts = new ObservableCollection<MediaObjectItemViewModel>(
                ContentService.MediaObjectContentService
                .GetUnloadedMediaObjects()
                .Select(x => new MediaObjectItemViewModel(x, CatalogStore.MediaFilesCatalogPath)));

            IsMediaObjectPageSelected = false;
            OnPropertyChenged(nameof(IsMediaObjectPageSelected));

            IsFiltrationPageSelected = false;
            OnPropertyChenged(nameof(IsFiltrationPageSelected));

            FilterCriterions = new List<FilterCriterion>(ContentService.FilterCriterionContentService.GetUnloadedFilterCriterions());

            NavigationStore.CurrentViewModel = null;

            SearchingService.SearchElements = new List<MediaObject>(MediaObjacts.Select(x => x.MediaObject));

            CurrentFiltrationPageViewModel = new FiltrationPageViewModel(MediaObjacts,
                ContentService,
                FilterCriterions);
            CurrentFiltrationPageViewModel.SetFilterableCollectionValue +=
                (mediaObjects) =>
                {
                    var mediaObjectsItemsResult = mediaObjects.Select(x => new MediaObjectItemViewModel(x, CatalogStore.MediaFilesCatalogPath));
                    MediaObjacts = new ObservableCollection<MediaObjectItemViewModel>(mediaObjectsItemsResult);
                    SearchingService.SearchElements = MediaObjacts.Select(x => x.MediaObject);
                };
            CurrentFiltrationPageViewModel.OnEditButtonPressed += OpenEditFilterCriterionPage;
        }

        private void OpenMediaObjectPageView(object? parameter)
        {
            if( parameter is not null and MediaObjectItemViewModel mediaObjectItem)
            {
                ContentService.MediaObjectContentService.SelectMediaObject(mediaObjectItem.MediaObject);

                if (CurrentPage is SelectedMediaObjectPageViewModel)
                {
                    NavigationStore.CurrentViewModel = new SelectedMediaObjectPageViewModel(ContentService);
                }
                else
                {
                    CurrentMediaObjectPageViewModel = new SelectedMediaObjectPageViewModel(ContentService);
                }
            }
        }

        private void OpenEditFilterCriterionPage(FilterCriterion filterCriterion)
        {
            ContentService.FilterCriterionContentService.SelectFilterCriterion(filterCriterion);
            var editFilterCriterionViewModel = new EditFilterCriterionViewModel(OperationService,ContentService);

            if (DialogService.ShowDialog(editFilterCriterionViewModel) ?? false)
            {
                InitStartEntities();
            }
            ContentService.FilterCriterionContentService.DropSelectionOfFilterCriterion();
        }

        private void SearchMediaObjectMethod(object? parameter)
        {
            List<MediaObject> ExistingMediaObjects = new List<MediaObject>(MediaObjacts.Select(x => x.MediaObject));
            ExistingMediaObjects = (List<MediaObject>)SearchingService.SerchElements(SearchMediaObjectQuery);

            var mediaObjectsItemsResult = ExistingMediaObjects.Select(x => new MediaObjectItemViewModel(x, CatalogStore.MediaFilesCatalogPath));

            MediaObjacts = new ObservableCollection<MediaObjectItemViewModel>(mediaObjectsItemsResult);
        }

    }
}
