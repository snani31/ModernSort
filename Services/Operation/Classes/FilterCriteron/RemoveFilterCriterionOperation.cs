using ModernSort.Services.Operations;
using ModernSort.Stores.Catalog;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.Windows.Documents;

namespace ModernSort.Services.Operation
{
    internal class RemoveFilterCriterionOperation : RemoveOperation<FilterCriterion>
    {
        #region ViewModel Data
        private IoCollection<MediaObject> EditableMediaObjects { get; init; }
        private Guid RemovableFilterCriteriumGUID { get; init; }
        private IEnumerable<Filter> BeforeEditFilters {  get; init; }
        #endregion

        #region Catalog Store Data

        private string MediaObjectsFilePath {  get; set; }

        #endregion

        #region Local Propertyes

        #endregion

        public RemoveFilterCriterionOperation(IEnumerable<Filter> beforeEditFilters, IEnumerable<MediaObject> mediaObjects, 
            Guid removableFilterCriteriumGUID)
        {
            BeforeEditFilters = beforeEditFilters;
            EditableMediaObjects = new IoCollection<MediaObject>(mediaObjects);
            RemovableFilterCriteriumGUID = removableFilterCriteriumGUID;
        }

        public override void Remove()
        {
            RemoveFiltersFromMediaObjects(new Queue<Filter>(BeforeEditFilters), EditableMediaObjects);
            base.EntityesCollection = new IoCollection<FilterCriterion>
                (EntityesCollection.Where(x => !x.ID.Equals(RemovableFilterCriteriumGUID)));

        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.FilterCriterionFilePath;
            MediaObjectsFilePath = catalogStore.MediaObjectsFilePath;
        }

        private void RemoveFiltersFromMediaObjects(Queue<Filter> removableFilters,IEnumerable<MediaObject> mediaObjects)
        {
            if (removableFilters.Count <= 0) return;

            Filter removableFilter = removableFilters.Dequeue();

            List<Filter> filtersOfMediaObjectTmp;
            foreach (var mediaObjectTmp in mediaObjects)
            {
                filtersOfMediaObjectTmp = new List<Filter>(mediaObjectTmp.MatchFilters);
                if (filtersOfMediaObjectTmp.Contains(removableFilter))
                {
                    filtersOfMediaObjectTmp = new List<Filter>(mediaObjectTmp.MatchFilters);
                    filtersOfMediaObjectTmp.Remove(removableFilter);
                    mediaObjectTmp.MatchFilters = filtersOfMediaObjectTmp;
                } 
            }
            RemoveFiltersFromMediaObjects(removableFilters, mediaObjects);
        }

        public override void UploadChangedElements(ISerializer serializer)
        {
            if (EditableMediaObjects.Serialize(serializer, MediaObjectsFilePath, System.IO.FileMode.OpenOrCreate))
                base.UploadChangedElements(serializer);
            else
                base.OperationResult = false;
        }
    }
}
