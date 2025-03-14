using RankingEntityes.IO_Entities.Interfaces;
using ModernSort.Stores.Catalog;
using RankingEntityes.Ranking_Entityes;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;

namespace ModernSort.Services.Operations
{
    internal class OperationService
    {
        private ISerializer Serializer { get; init; }

        private IDeserializer Deserializer { get; init; }

        private CatalogStore CatalogStore { get; init; }

        public OperationService(ISerializer serializer, IDeserializer deserializer, CatalogStore catalogStore)
        {
            Serializer = serializer;
            Deserializer = deserializer;
            CatalogStore = catalogStore;
        }

        public bool InvokeOperation<T>(IOperation operation)
            where T : IoEntity
        {
            if (operation is CreateOperation createOperation)
            {
                createOperation.SetCatalogData(CatalogStore);
                createOperation.Create(Serializer);
            }
            else if (operation is RemoveOperation<T> deleteOperation)
            {
                deleteOperation.SetCatalogData(CatalogStore);
                deleteOperation.GetExistingElements(Deserializer);
                deleteOperation.Remove();
                deleteOperation.UploadChangedElements(Serializer);
            }
            else if (operation is UpdateOperation<T> updateOperation)
            {
                updateOperation.SetCatalogData(CatalogStore);
                updateOperation.GetExistingElements(Deserializer);
                updateOperation.Update();
                updateOperation.UploadChangedElements(Serializer);
            }
            return operation.OperationResult;
        }
    }
}
