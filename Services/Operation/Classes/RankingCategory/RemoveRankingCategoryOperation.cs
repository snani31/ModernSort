using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal class RemoveRankingCategoryOperation : RemoveOperation<RankingCategory>
    {

        private Guid SelectedRankingCategoryID { get; set; }
        private string RankingCatalogPath { get; set; }

        public RemoveRankingCategoryOperation(Guid selectedRankingCategoryID)
        {
            SelectedRankingCategoryID = selectedRankingCategoryID;
            base.OperationResult = false;
        }

        public override void Remove()
        {
            EntityesCollection = new IoCollection<RankingCategory>
                (EntityesCollection.Where(x => !x.ID.Equals(SelectedRankingCategoryID)));

            Directory.Delete(RankingCatalogPath, true);
        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.RankingCategoriesFilePath;
            RankingCatalogPath = catalogStore.RankingCatalogPath;
        }
    }
}
