using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using ModernSort.Services;

namespace ModernSort.Stores.Catalog
{
    internal class CatalogStore
    {
        public RankingCategory? SelectedRankingCategory { get; private set; }

        #region FileNames

        internal required string mediaObjectsFileName;

        internal required string rankingCategoriesFileName;

        internal required string GUIDsFileName;

        internal required string rankingCategoryIconNameNoExtention;
        #endregion

        #region CatalogNames
        internal required string mediaFilesCatalogName;

        internal required string coreResourcesCatalogPath;
        #endregion

        #region FilePaths
        public string GUIDsFilePath
        {
            get
            {
                return $@"{coreResourcesCatalogPath}\{GUIDsFileName}";
            }

        }

        public string RankingCategoriesFilePath
        {
            get
            {
                return $@"{coreResourcesCatalogPath}\{rankingCategoriesFileName}";
            }

        }

        private string _mediaObjectsFilePath;
        public string MediaObjectsFilePath
        {
            get
            {
                return _mediaObjectsFilePath;
            }

            private set
            {
                _mediaObjectsFilePath = value;
            }

        }

        private string _rankingCategoryIconPath;

        public string RankingCategoryIconPath
        {
            get { return _rankingCategoryIconPath; }
            private set { _rankingCategoryIconPath = value; }
        }


        #endregion

        #region CatalogPaths

        private string _mediaFilesCatalogPath;

        public string MediaFilesCatalogPath
        {
            get 
            { 
                return _mediaFilesCatalogPath;
            }

            private set 
            { 
                _mediaFilesCatalogPath = value;
            }
        }

        private string _rankingCatalogPath;

        public string RankingCatalogPath
        {
            get
            { 
                return _rankingCatalogPath; 
            }

            private set
            {
                _rankingCatalogPath = value;
            }
        }

        #endregion

        public CatalogStore()
        {

        }

        public void DropRankingSelection()
        {
            SelectedRankingCategory = null;
        }

        public void CalibrateRankingCategory(RankingCategory rankingCategorySelected)
        {
            RankingCatalogPath = rankingCategorySelected.RankingDirrectoryPath;
            MediaObjectsFilePath = $@"{RankingCatalogPath}\{mediaObjectsFileName}";
            MediaFilesCatalogPath = $@"{RankingCatalogPath}\{mediaFilesCatalogName}";
            RankingCategoryIconPath = rankingCategorySelected.RankingIconPath;
        }

    }
}
