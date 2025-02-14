using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.ViewModel.Items
{
    internal class MediaObjectItemViewModel
    {
        private MediaObject _mediaObject;
        public MediaObject MediaObject { get => _mediaObject; set => _mediaObject = value; }
        public string ID
        {
            get { return MediaObject.ID.ToString(); }
        }
        public string Tytle
        {
            get { return MediaObject.Tytle; }
        }
        public string Descryption
        {
            get { return MediaObject.Description; }
        }
        public List<string> Paths
        {
            get
            {
                return MediaObject.Paths;
            }
        }
        public MediaObjectItemViewModel(MediaObject mediaObjact)
        {
            MediaObject = mediaObjact;
        }
    }
}
