using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;

namespace ModernSort.Services.Operations
{
    internal abstract class RemoveOperation : IOperation
    {
        public bool OperationResult { get; set; }

        public abstract void Remove(ISerializer serializer, IDeserializer deserializer, CatalogStore catalogStore);
    }
}
