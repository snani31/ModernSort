using Microsoft.Xaml.Behaviors.Core;
using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Static;
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

        private MediaObjectItemViewModel _selectedMediaObjectItem;
        public MediaObjectItemViewModel SelectedMediaObjectItem
        {
            private get 
            {
                return _selectedMediaObjectItem;
            }
            set 
            {
                _selectedMediaObjectItem = value;
                EditMediaObjectWindowOpen?.OnCanExecuteChanged();
            }
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand CreateMediaObjectWindowOpen { get; init; }
        public RelayCommand EditMediaObjectWindowOpen { get; init; }
        public ICommand OpenMediaObjectPage { get; init; }
        public ICommand CloseDialog { get; init; }

        private IDialogService DialogService {  get; init; }

        private IDeserializer Deserializer { get; init; }

        private ISerializer Serializer { get; init; }

        private SelectedMediaObjectPageViewModel _currentMediaObject;
        public SelectedMediaObjectPageViewModel CurrentMediaObject
        {
            get 
            { 
                return _currentMediaObject;
            }
            private set 
            {
                _currentMediaObject = value;
                OnPropertyChenged(nameof(CurrentMediaObject)); 
            }
        }

        public SelectedRankingCategoryViewModel() 
        {
            OpenMediaObjectPage = new RelayCommand(OpenMediaObjectPageView);
            CloseDialog = new RelayCommand(
                (p) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                });
        }

        public SelectedRankingCategoryViewModel(RankingCategory selectedRankingCategory,
            IDialogService dialogService, ISerializer serializer, IDeserializer deserializer): this()
        {
            Deserializer = deserializer;
            Serializer = serializer;
            SelectedRankingCategory = selectedRankingCategory;
            DialogService = dialogService;
            CreateMediaObjectViewModel viewModel = new CreateMediaObjectViewModel(Serializer, SelectedRankingCategory);

            CreateMediaObjectWindowOpen = new RelayCommand(
                (p) => 
                {

                    if (DialogService.ShowDialog(viewModel) ?? false)
                    {
                        _mediaObjects.Deserialize(Deserializer, $@"{ProjactIoWorker.UserResourcesDirrectoryPath}\{SelectedRankingCategory.ID}\MediaObjacts.json");
                        OnPropertyChenged(nameof(MediaObjacts));
                    }
                });

            EditMediaObjectWindowOpen = new RelayCommand(
                (p) =>
                {
                    MediaObject selectedMediaObj = _mediaObjects.First(x => x.ID.ToString().Equals(SelectedMediaObjectItem.ID));
                    EditMediaObjectViewModel editMediaObjectViewModel = new EditMediaObjectViewModel(SelectedRankingCategory,selectedMediaObj, Serializer, Deserializer);
                    if (DialogService.ShowDialog(editMediaObjectViewModel) ?? false)
                    {
                        _mediaObjects.Deserialize(Deserializer, $@"{ProjactIoWorker.UserResourcesDirrectoryPath}\{SelectedRankingCategory.ID}\MediaObjacts.json");
                        OnPropertyChenged(nameof(MediaObjacts));
                        CurrentMediaObject = null;
                        OnPropertyChenged(nameof(CurrentMediaObject));
                    }
                },
                () => 
                {
                    if (SelectedMediaObjectItem is not null)
                    {
                        return true;
                    }
                    return false;
                }
                );

            _mediaObjects = new IoCollection<MediaObject>();

            _mediaObjects.Deserialize(Deserializer, $@"{ProjactIoWorker.UserResourcesDirrectoryPath}\{SelectedRankingCategory.ID}\MediaObjacts.json");

        }

        private ObservableCollection<MediaObjectItemViewModel> ParseIoToCollection(IEnumerable<MediaObject> mediaObjects)
        {
            return new ObservableCollection<MediaObjectItemViewModel>(mediaObjects.Select(x => new MediaObjectItemViewModel(x,SelectedRankingCategory.RankingDirrectoryPath + @"\Media")));
        }

        private void OpenMediaObjectPageView(object? parameter)
        {
            if(parameter is not null and MediaObjectItemViewModel MediaObjectItem)
            {
                MediaObject mediaObjectSelected = _mediaObjects.First(x => x.ID.ToString() == MediaObjectItem.ID);
                CurrentMediaObject = new SelectedMediaObjectPageViewModel(mediaObjectSelected,
                SelectedRankingCategory.RankingDirrectoryPath + @"\Media");
            }

        }

    }
}
