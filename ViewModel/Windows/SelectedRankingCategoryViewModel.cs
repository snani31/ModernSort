using ModernSort.Services.Dialog;
using ModernSort.ViewModel.Items;
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
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        private IoCollection<MediaObject> _mediaObjects;
        public ObservableCollection<MediaObjectItemViewModel> MediaObjacts 
        { 
            get 
            {
                return ParseIoToCollection(_mediaObjects);
            }
        }
        public SelectedRankingCategoryViewModel()
        {
            MediaObject a = new MediaObject();
            _mediaObjects = new IoCollection<MediaObject>()
            {
                new MediaObject() { Tytle = "Первghjghjgghjghjghjgjhghjый"}, 
                new MediaObject() { Tytle = "Второй"},
                new MediaObject() { Tytle = "Третий"},
                new MediaObject() { Tytle = "Четвёртый"},
                new MediaObject() { Tytle = "Пятый"},
                new MediaObject() { Tytle = "Шестой"}
            };


        }

        private ObservableCollection<MediaObjectItemViewModel> ParseIoToCollection(IEnumerable<MediaObject> list)
        {
            return new ObservableCollection<MediaObjectItemViewModel>(list.Select(x => new MediaObjectItemViewModel(x)));
        }

    }
}
