using RankingEntityes.IO_Entities.Interfaces;
using ModernSort.Stores.Catalog;

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

        public bool InvokeOperation(IOperation operation)
        {
            if (operation is CreateOperation createOperation)
            {
                createOperation.Create(Serializer, CatalogStore);
                return createOperation.OperationResult;
            }
            else if (operation is RemoveOperation deleteOperation)
            {
                deleteOperation.Remove(Serializer,Deserializer, CatalogStore);
                return deleteOperation.OperationResult;
            }
            else if (operation is UpdateOperation updateOperation)
            {
                updateOperation.Update(Serializer, Deserializer, CatalogStore);
                return updateOperation.OperationResult;
            }
            return false;
        }
    }
}
