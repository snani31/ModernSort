using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;

namespace ModernSort.Services.Operations
{
    internal abstract class UpdateOperation : IOperation
    {

        public bool OperationResult { get; set; }
        public abstract void Update(ISerializer serializer, IDeserializer deserializer, CatalogStore catalogStore);
    }
}
