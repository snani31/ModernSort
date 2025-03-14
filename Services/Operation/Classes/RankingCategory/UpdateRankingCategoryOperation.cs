using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal class UpdateRankingCategoryOperation : UpdateOperation<RankingCategory>
    {
        #region ViewModel Data
        private string Tytle { get; init; }
        private string Description { get; init; }
        private string NewImageFileBasePath { get; init; }
        private bool IconWasChanged { get; init; }
        private string ExistingRankingIconPath { get; set; }
        private Guid SelectedRankingGUID { get; set; }
        #endregion

        #region CatalogStore Data

        private string SelectedRankingCatalogPath {  get; set; }

        #endregion

        public UpdateRankingCategoryOperation(string tytle, string description, string newImageFileBasePath, 
            bool iconWasChanged, RankingCategory updatebeleRankingCategory)
        {
            OperationResult = false;

            Tytle = tytle;
            Description = description;
            NewImageFileBasePath = newImageFileBasePath;

            IconWasChanged = iconWasChanged;

            SelectedRankingGUID = updatebeleRankingCategory.ID;
            ExistingRankingIconPath = updatebeleRankingCategory.RankingIconPath;
        }

        public override void Update()
        {

            int indexOfSelectedRanking = EntityesCollection.Select((member, index) => (member, index))
                .First(x => x.member.ID.Equals(SelectedRankingGUID)).index;

            string newIconInCatalogPath = ExistingRankingIconPath;
            if (IconWasChanged)
            {
                File.Delete(ExistingRankingIconPath);
                newIconInCatalogPath = Path.ChangeExtension(ExistingRankingIconPath, Path.GetExtension(NewImageFileBasePath));
                File.Copy(NewImageFileBasePath, newIconInCatalogPath, overwrite: true);
            }

            EntityesCollection[indexOfSelectedRanking] = new RankingCategory()
            {
                ID = SelectedRankingGUID,
                Description = Description,
                RankingDirrectoryPath = SelectedRankingCatalogPath,
                Tytle = Tytle,
                RankingIconPath = newIconInCatalogPath
            };

        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.RankingCategoriesFilePath;
            SelectedRankingCatalogPath = catalogStore.RankingCatalogPath;
        }
    }
}
