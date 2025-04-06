using Microsoft.Xaml.Behaviors.Core;
using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
using ModernSort.Stores.Navigation;
using ModernSort.View.Pages;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Items.FiltrationItems;
using ModernSort.ViewModel.Pages;
using Newtonsoft.Json.Linq;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class SelectedRankingCategoryViewModel : ViewModelBase, IDialogRequestClose
    {
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
            }
            get
            {
                return _mediaObjacts;
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
                        //ParseIoToCollection();
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

            CurrentMediaObjectPageViewModel = new SelectedMediaObjectPageViewModel(ContentService);
            NavigationStore.CurrentViewModel = CurrentMediaObjectPageViewModel;

            CurrentFiltrationPageViewModel = new FiltrationPageViewModel(MediaObjacts, ContentService);
            CurrentFiltrationPageViewModel.SetFilterableCollectionValue +=
                (d) =>
                {
                    var a = d.Select(x => new MediaObjectItemViewModel(x, CatalogStore.MediaFilesCatalogPath));
                    MediaObjacts = new ObservableCollection<MediaObjectItemViewModel>(a);
                };
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

    }
}
