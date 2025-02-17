using Microsoft.Xaml.Behaviors.Core;
using ModernSort.Services.Dialog;
using ModernSort.Static;
using ModernSort.ViewModel.Items;
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

namespace ModernSort.ViewModel.Windows
{
    internal class SelectedRankingCategoryViewModel : ViewModelBase, IDialogRequestClose
    {
        private RankingCategory _rankingCategory;
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        private IoCollection<MediaObject> _mediaObjects;
        public ActionCommand MediaObjectCreateWindowOpen {  get; init; }
        private IDialogService _DialogService {  get; init; }
        public ObservableCollection<MediaObjectItemViewModel> MediaObjacts 
        { 
            get 
            {
                return ParseIoToCollection(_mediaObjects);
            }
        }

        public RankingCategory RankingCategory { get => _rankingCategory; set => _rankingCategory = value; }

        private IDeserializer Deserializer { get; init; }

        private ISerializer Serializer { get; init; }

        public SelectedRankingCategoryViewModel() 
        {

        }

        public SelectedRankingCategoryViewModel(RankingCategory SelectedRankingCategory,
            IDialogService dialogService, ISerializer serializer, IDeserializer deserializer): this()
        {
            Deserializer = deserializer;
            Serializer = serializer;
            RankingCategory = SelectedRankingCategory;
            _DialogService = dialogService;
            CreateMediaObjectViewModel viewModel = new CreateMediaObjectViewModel(_serializer, RankingCategory);

            MediaObjectCreateWindowOpen = new ActionCommand(
                () => 
                {
                    _DialogService.ShowDialog(viewModel);
                });

            _mediaObjects = new IoCollection<MediaObject>();

            _mediaObjects.Deserialize(Deserializer, $@"{ProjactIoWorker.UserResourcesDirrectoryPath}\{RankingCategory.ID}\MediaObjacts.json");
        }

        private ObservableCollection<MediaObjectItemViewModel> ParseIoToCollection(IEnumerable<MediaObject> list)
        {
            return new ObservableCollection<MediaObjectItemViewModel>(list.Select(x => new MediaObjectItemViewModel(x,RankingCategory.RankingDirrectoryPath + @"\Media")));
        }

    }
}
