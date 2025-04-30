using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Stores.Catalog;
using ModernSort.Stores.Navigation;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Pages;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class SelectedRankingCategoryViewModel : ViewModelBase, IDialogRequestClose
    {
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


        public ICommand SetMediaObjectPageValue { get; init; }
        public ICommand SetFiltrationPageValue { get; init; }
        public ICommand CreateMediaObjectWindowOpen { get; init; }
        public ICommand CreateFilterCriterionWindowOpen { get; init; }
        public RelayCommand EditMediaObjectWindowOpen { get; init; }
        public ICommand OpenMediaObjectPage { get; init; }
        public ICommand CloseDialog { get; init; }
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


        }

        public SelectedRankingCategoryViewModel(OutputContentService contentService,CatalogStore catalogStore,OperationService operationService,RankingCategory selectedRankingCategory,
            IDialogService dialogService, ISerializer serializer, IDeserializer deserializer): this()
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

            CurrentMediaObjectPageViewModel = new SelectedMediaObjectPageViewModel(ContentService);
            NavigationStore.CurrentViewModel = null;

            CurrentFiltrationPageViewModel = new FiltrationPageViewModel(MediaObjacts,
                ContentService,
                FilterCriterions);
            CurrentFiltrationPageViewModel.SetFilterableCollectionValue +=
                (mediaObjects) =>
                {
                    var mediaObjectsItemsResult = mediaObjects.Select(x => new MediaObjectItemViewModel(x, CatalogStore.MediaFilesCatalogPath));
                    MediaObjacts = new ObservableCollection<MediaObjectItemViewModel>(mediaObjectsItemsResult);
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

    }
}
