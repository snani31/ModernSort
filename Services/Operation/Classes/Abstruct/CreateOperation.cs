using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal abstract class CreateOperation : IOperation
    {
        public bool OperationResult { get; set; }
        protected FileMode OperationFileMode { get; set; } = FileMode.Append;
        protected string FilePath { get; set; }
        public abstract void Create(ISerializer serializer);

        public abstract void SetCatalogData(CatalogStore catalogStore);
    }
}
