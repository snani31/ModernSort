using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal abstract class UpdateOperation<T> : IOperation 
        where T : IoEntity
    {
        private FileMode OperationFileMode { get; set; } = FileMode.OpenOrCreate;
        protected IoCollection<T> EntityesCollection { get; set; }
        protected string FilePath { private get; set; }
        public bool OperationResult { get; set; }
        public abstract void Update();

        public void GetExistingElements(IDeserializer deserializer)
        {
            EntityesCollection = new IoCollection<T>();
            EntityesCollection.Deserialize(deserializer, FilePath);
        }
        public virtual void UploadChangedElements(ISerializer serializer)
        {
            OperationResult = EntityesCollection.Serialize(serializer,
                FilePath,
                mode: OperationFileMode);
        }
        public abstract void SetCatalogData(CatalogStore catalogStore);
    }
}
