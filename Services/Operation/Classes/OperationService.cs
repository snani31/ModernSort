using RankingEntityes.IO_Entities.Interfaces;
using ModernSort.Stores.Catalog;
using RankingEntityes.Ranking_Entityes;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Filters;

namespace ModernSort.Services.Operations
{
    internal class OperationService
    {
        private ISerializer Serializer { get; init; }
        private CatalogStore CatalogStore { get; init; }

        private Dictionary<Type?,IDeserializer> DeserializersMapping { get; init; }

        public OperationService(ISerializer serializer,CatalogStore catalogStore)
        {
            Serializer = serializer;
            CatalogStore = catalogStore;

            DeserializersMapping = new Dictionary<Type?, IDeserializer>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Выберите IoEntity объект, над которым будет произведена операция</typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
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
                deleteOperation.GetExistingElements(DeserializersMapping[typeof(T)]);
                deleteOperation.Remove();
                deleteOperation.UploadChangedElements(Serializer);
            }
            else if (operation is UpdateOperation<T> updateOperation)
            {
                updateOperation.SetCatalogData(CatalogStore);
                updateOperation.GetExistingElements(DeserializersMapping[typeof(T)]);
                updateOperation.Update();
                updateOperation.UploadChangedElements(Serializer);
            }
            return operation.OperationResult;
        }

        public void RegistrateDeserializer(Type? type,IDeserializer deserializer)
        {
            if (DeserializersMapping.ContainsKey(type))
            {
                throw new ArgumentException($"Key {type} Was already mapped to the {DeserializersMapping[type]}");
            }
            DeserializersMapping.Add(type, deserializer);
        }

    }
}
