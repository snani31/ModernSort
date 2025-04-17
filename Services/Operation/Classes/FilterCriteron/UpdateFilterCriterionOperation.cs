using ModernSort.Services.Operations;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Services.Operation
{
    internal class UpdateFilterCriterionOperation : UpdateOperation<FilterCriterion>
    {
        
        #region ViewModel Data
        private string Tytle {  get; init; }
        private string Description { get; init; }
        private IoCollection<MediaObject> EditableMediaObjects { get; init; }
        private Guid EditableFilterCriteriumGUID { get; init; }
        private IEnumerable<Filter> RemovableEditFilters { get; init; }
        private IEnumerable<Filter> UpdatableEditFilters { get; init; }
        private IEnumerable<Filter> NewCreatedFilters { get; init; }

        private IEnumerable<Filter> AllFilters { get; init; }
        #endregion

        #region Catalog Store Data

        private string MediaObjectsFilePath { get; set; }
        private string GUIDsFilePath { get; set; }

        #endregion

        #region Local Propertyes

        #endregion

        public UpdateFilterCriterionOperation(
            string tytle,
            string description,
            Guid editableFilterCriteriumGUID,
            IEnumerable<Filter> removableEditFilters, 
            IEnumerable<Filter> updatableEditFilters,
            IEnumerable<Filter> newCreatedFilters,
            IEnumerable<Filter> allFilters,
            IEnumerable<MediaObject> editableMediaObjects
            )
        {
            EditableFilterCriteriumGUID = editableFilterCriteriumGUID;
            RemovableEditFilters = removableEditFilters;
            UpdatableEditFilters = updatableEditFilters;
            NewCreatedFilters = newCreatedFilters;
            AllFilters = allFilters;
            Tytle = tytle;
            Description = description;
            EditableMediaObjects = new IoCollection<MediaObject>(editableMediaObjects);
        }

        public override void Update()
        {
            RemoveFiltersFromMediaObjects(new Queue<Filter>(RemovableEditFilters), EditableMediaObjects);
            UpdateFiltersInMediaObjects(new Queue<Filter>(UpdatableEditFilters), EditableMediaObjects);

            int indexOfSelectedFilterCriterium = base.EntityesCollection.Select((member, index) => (member, index))
               .First(x => x.member.ID.Equals(EditableFilterCriteriumGUID)).index;


            foreach (var filter in AllFilters)
            {
                foreach (var newCreatedFilter in NewCreatedFilters)
                {
                    if (filter.Equals(newCreatedFilter))
                        filter.ID = ProjactIoWorker.GetUniqGuid(GUIDsFilePath);
                }
            }

            EntityesCollection[indexOfSelectedFilterCriterium] = new ConditionFilterCriterion()
            {
                ID = EditableFilterCriteriumGUID,
                Description = this.Description,
                Tytle = this.Tytle,
                Filters = AllFilters
            };
        }

        private void UpdateFiltersInMediaObjects(Queue<Filter> updatableFilters, IEnumerable<MediaObject> mediaObjects)
        {
            if (updatableFilters.Count <= 0) return;
            Filter updatableFilter = updatableFilters.Dequeue();

            List<Filter> filtersOfMediaObjectTmp;
            int indexOfUpdatableFilterTmp;

            foreach (var mediaObjectTmp in mediaObjects)
            {
                filtersOfMediaObjectTmp = new List<Filter>(mediaObjectTmp.MatchFilters);

                indexOfUpdatableFilterTmp = filtersOfMediaObjectTmp.FindIndex(filter => filter.IsMatchByID(updatableFilter));
                if (indexOfUpdatableFilterTmp < 0) continue;

                filtersOfMediaObjectTmp[indexOfUpdatableFilterTmp] = updatableFilter;
                mediaObjectTmp.MatchFilters = filtersOfMediaObjectTmp;
            }

            UpdateFiltersInMediaObjects(updatableFilters,mediaObjects);

        }

        private void RemoveFiltersFromMediaObjects(Queue<Filter> removableFilters, IEnumerable<MediaObject> mediaObjects)
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

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.FilterCriterionFilePath;
            MediaObjectsFilePath = catalogStore.MediaObjectsFilePath;
            GUIDsFilePath = catalogStore.GUIDsFilePath;
        }

    }
}
