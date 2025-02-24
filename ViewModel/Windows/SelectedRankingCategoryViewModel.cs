using Microsoft.Xaml.Behaviors.Core;
using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Static;
using ModernSort.View.Pages;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Pages;
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

        public MediaObjectItemViewModel SelectedMediaObjectItem
        {
            set 
            {
                if (value is null) return;
                MediaObject a = _mediaObjects.First(x => x.ID.ToString() == value.ID);
                CurrentMediaObject = new SelectedMediaObjectPageViewModel(a, 
                    SelectedRankingCategory.RankingDirrectoryPath + @"\Media");
                OnPropertyChenged(nameof(CurrentMediaObject));
            }
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand CreateMediaObjectWindowOpen { get; init; }
        public ICommand CloseDialog { get; init; }

        private IDialogService DialogService {  get; init; }

        private IDeserializer Deserializer { get; init; }

        private ISerializer Serializer { get; init; }

        public SelectedMediaObjectPageViewModel CurrentMediaObject {  get; private set; }

        public SelectedRankingCategoryViewModel() 
        {

        }

        public SelectedRankingCategoryViewModel(RankingCategory selectedRankingCategory,
            IDialogService dialogService, ISerializer serializer, IDeserializer deserializer): this()
        {
            Deserializer = deserializer;
            Serializer = serializer;
            SelectedRankingCategory = selectedRankingCategory;
            DialogService = dialogService;
            CreateMediaObjectViewModel viewModel = new CreateMediaObjectViewModel(Serializer, SelectedRankingCategory);

            CloseDialog = new RelayCommand(
                (p) =>
                {
                    CloseRequested?.Invoke(this,new DialogCloseRequestedEventArgs(false));
                });

            CreateMediaObjectWindowOpen = new RelayCommand(
                (p) => 
                {

                    if (DialogService.ShowDialog(viewModel) ?? false)
                    {
                        _mediaObjects.Deserialize(Deserializer, $@"{ProjactIoWorker.UserResourcesDirrectoryPath}\{SelectedRankingCategory.ID}\MediaObjacts.json");
                        OnPropertyChenged(nameof(MediaObjacts));
                    }
                });

            _mediaObjects = new IoCollection<MediaObject>();

            _mediaObjects.Deserialize(Deserializer, $@"{ProjactIoWorker.UserResourcesDirrectoryPath}\{SelectedRankingCategory.ID}\MediaObjacts.json");

        }

        private ObservableCollection<MediaObjectItemViewModel> ParseIoToCollection(IEnumerable<MediaObject> mediaObjects)
        {
            return new ObservableCollection<MediaObjectItemViewModel>(mediaObjects.Select(x => new MediaObjectItemViewModel(x,SelectedRankingCategory.RankingDirrectoryPath + @"\Media")));
        }

    }
}
