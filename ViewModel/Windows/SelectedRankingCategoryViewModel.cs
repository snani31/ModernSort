using Microsoft.Xaml.Behaviors.Core;
using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
using ModernSort.View.Pages;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Pages;
using Newtonsoft.Json.Linq;
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
        private RankingCategory _selectedRankingCategory;

        public RankingCategory SelectedRankingCategory { get => _selectedRankingCategory;init => _selectedRankingCategory = value; }

        private IoCollection<MediaObject> _mediaObjects;

        public ObservableCollection<MediaObjectItemViewModel> MediaObjacts
        {
            get
            {
                return ParseIoToCollection(_mediaObjects);
            }
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand CreateMediaObjectWindowOpen { get; init; }
        public RelayCommand EditMediaObjectWindowOpen { get; init; }
        public ICommand OpenMediaObjectPage { get; init; }
        public ICommand CloseDialog { get; init; }
        public OutputContentService ContentService { get; }
        private CatalogStore CatalogStore {  get; init; }
        private OperationService OperationService { get; init; }

        private IDialogService DialogService {  get; init; }

        private IDeserializer Deserializer { get; init; }

        private ISerializer Serializer { get; init; }

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

        }

        public SelectedRankingCategoryViewModel(OutputContentService contentService,CatalogStore catalogStore,OperationService operationService,RankingCategory selectedRankingCategory,
            IDialogService dialogService, ISerializer serializer, IDeserializer deserializer): this()
        {
            Deserializer = deserializer;
            Serializer = serializer;
            SelectedRankingCategory = selectedRankingCategory;
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
                    var viewModel = new CreateMediaObjectViewModel(OperationService);

                    if (DialogService.ShowDialog(viewModel) ?? false)
                    {
                        _mediaObjects = ContentService.MediaObjectContentService.GetUnloadedMediaObjects();
                        OnPropertyChenged(nameof(MediaObjacts));
                    }
                });

            EditMediaObjectWindowOpen = new RelayCommand(
                (p) =>
                {
                    EditMediaObjectViewModel editMediaObjectViewModel = new EditMediaObjectViewModel(OperationService,ContentService);
                    if (DialogService.ShowDialog(editMediaObjectViewModel) ?? false)
                    {
                        _mediaObjects = ContentService.MediaObjectContentService.GetUnloadedMediaObjects();
                        OnPropertyChenged(nameof(MediaObjacts));
                        CurrentMediaObjectPagePresenter = null;
                        OnPropertyChenged(nameof(CurrentMediaObjectPagePresenter));
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

            _mediaObjects = new IoCollection<MediaObject>();
            _mediaObjects = ContentService.MediaObjectContentService.GetUnloadedMediaObjects();

        }

        private ObservableCollection<MediaObjectItemViewModel> ParseIoToCollection(IEnumerable<MediaObject> mediaObjects)
        {
            return new ObservableCollection<MediaObjectItemViewModel>(mediaObjects.Select(x => new MediaObjectItemViewModel(x,SelectedRankingCategory.RankingDirrectoryPath + @"\Media")));
        }

        private void OpenMediaObjectPageView(object? parameter)
        {
            if(parameter is not null and MediaObjectItemViewModel MediaObjectItem)
            {
                ContentService.MediaObjectContentService.SelectMediaObject(MediaObjectItem.MediaObject);
                CurrentMediaObjectPagePresenter = new SelectedMediaObjectPageViewModel(ContentService);

            }

        }

    }
}
