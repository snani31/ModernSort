using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal class DeleteRankingCategoryOperation : RemoveOperation
    {
        private FileMode OperationFileMode { get; set; } = FileMode.OpenOrCreate;

        public DeleteRankingCategoryOperation()
        {

        }

        private void DeleteRemovableCategoryFromCollectionOfExistings(ref IoCollection<RankingCategory> listOfExistings,
            Guid remopvableItemID)
        {
            listOfExistings = new IoCollection<RankingCategory>
                (listOfExistings.Where(x => !x.ID.Equals(remopvableItemID)));
        }

        public override void Remove(ISerializer serializer, IDeserializer deserializer, CatalogStore catalogStore)
        {
            OperationResult = false;

            var removableCategoryID = catalogStore.SelectedRankingCategory.ID;

            var existingRankings = new IoCollection<RankingCategory>();
            existingRankings.Deserialize(deserializer, catalogStore.RankingCategoriesFilePath);

            DeleteRemovableCategoryFromCollectionOfExistings(ref existingRankings, removableCategoryID);

            if (existingRankings.Serialize(serializer,
                path: catalogStore.RankingCategoriesFilePath,
                mode: OperationFileMode))
            {
                Directory.Delete(catalogStore.RankingCatalogPath, true);
                OperationResult = true;
            }
        }
    }
}
