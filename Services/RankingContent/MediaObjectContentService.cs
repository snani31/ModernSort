using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Services
{
    internal class MediaObjectContentService
    {

        public event Action<object?> OnMediaObjectSelectedChange;
        private IDeserializer Deserializer { get; init; }
        public CatalogStore CatalogStore { get; init; }
        private MediaObject? _selectedMediaObject;
        public MediaObject? SelectedMediaObject
        {
            get => _selectedMediaObject; 
            private set
            {
                _selectedMediaObject = value;
                OnMediaObjectSelectedChange?.Invoke(value);
            }
        }

        private IoCollection<MediaObject> ExistingMediaObjects { get; set; }

        public MediaObjectContentService(IDeserializer deserializer, CatalogStore catalogStore)
        {
            Deserializer = deserializer;
            CatalogStore = catalogStore;
            ExistingMediaObjects = new IoCollection<MediaObject>();
        }

        public void SelectMediaObject(MediaObject mediaObject)
        {
            SelectedMediaObject = ExistingMediaObjects.First(x => x.ID.Equals(mediaObject.ID)) ?? throw new ArgumentException();
        }

        public void DropSelectionOfMEdiaObject()
        {
            SelectedMediaObject = null;
        }

        public IoCollection<MediaObject> GetUnloadedMediaObjects()
        {
            ExistingMediaObjects.Deserialize(Deserializer, CatalogStore.MediaObjectsFilePath);
            return ExistingMediaObjects;
        }

        public IEnumerable<string> GetFilesFullPathsOfSelectedMediaObject()
        {
            if (SelectedMediaObject is null) throw new Exception(message: "При попытке получить список полных путей всех медиа файлов заданный MediaObject был null");

            return SelectedMediaObject.Paths.Select(x => CatalogStore.MediaFilesCatalogPath + "\\" + x);
        }
    }
}
