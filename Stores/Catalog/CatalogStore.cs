using RankingEntityes.Ranking_Entityes.Ranking_Categories;

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

        public string MediaObjectsFilePath
        {
            get
            {
                return $@"{RankingCatalogPath}\{mediaObjectsFileName}";
            }

        }
        #endregion

        #region CatalogPaths
        public string MediaFilesCatalogPath
        {
            get
            {
                return $@"{RankingCatalogPath}\{mediaFilesCatalogName}";
            }

        }

        public string RankingCatalogPath
        {
            get
            {
                return $@"{coreResourcesCatalogPath}\{SelectedRankingCategory?.ID.ToString()}";
            }

        } 
        #endregion

        public CatalogStore()
        {

        }

        public void SelectRankingCategory(RankingCategory rankingCategorySelected)
        {
            SelectedRankingCategory = rankingCategorySelected;
        }
         public void DropRankingSelection()
        {
            SelectedRankingCategory = null;
        }

    }
}
