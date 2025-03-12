using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;

namespace ModernSort.Services.Operations
{
    internal abstract class CreateOperation : IOperation
    {
        public bool OperationResult { get; set; }
        public abstract void Create(ISerializer serializer, CatalogStore catalogStore);
    }
}
