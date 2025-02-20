using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;

namespace ModernSort.ViewModel.Items
{
    internal class RankingCategoryItemViewModel : ViewModelBase
    {
        private readonly RankingCategory _rankingCategor;

        public string ID
        {
            get { return _rankingCategor.ID.ToString(); }
        }
        public string Tytle
        {
            get { return _rankingCategor.Tytle; }
        }
        public string Descryption
        {
            get { return _rankingCategor.Description; }
        }
        public string RankingDirrectoryPath
        {
            get { return _rankingCategor.RankingDirrectoryPath; }
        }
        public string RankingIconPath
        {
            get { return _rankingCategor.RankingIconPath; }
        }
        public RankingCategoryItemViewModel(RankingCategory rankingCategory)
        {
            _rankingCategor = rankingCategory;
        }
    }
}
