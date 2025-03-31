using ModernSort.Services.RankingContent;
using ModernSort.Stores.Catalog;
using ModernSort.ViewModel.Pages;
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
    internal class OutputContentService
    {
        private CatalogStore CatalogStore {  get; init; }
        private IDeserializer Deserializer { get; init; }
        public RankingCategory? SelectedRankingCategory { get; private set; }

        private MediaObjectContentService _mediaObjectContentService;
        public MediaObjectContentService MediaObjectContentService 
        { 
            get 
            {
                return (SelectedRankingCategory is null) ? throw new Exception() : _mediaObjectContentService;
            } 
            init
            {
                _mediaObjectContentService = value;
            } 
        }

        private FilterCriterionsContentService _filterCriterionContentService;
        public FilterCriterionsContentService FilterCriterionContentService
        {
            get
            {
                return (SelectedRankingCategory is null) ? throw new Exception() : _filterCriterionContentService;
            }
            init
            {
                _filterCriterionContentService = value;
            }
        }


        public OutputContentService(CatalogStore catalogStore,IDeserializer deserializer)
        {
            CatalogStore = catalogStore;
            Deserializer = deserializer;
            MediaObjectContentService = new MediaObjectContentService(Deserializer,CatalogStore);
            FilterCriterionContentService = new FilterCriterionsContentService(Deserializer, CatalogStore);
        }

        public OutputContentService(CatalogStore catalogStore, IDeserializer deserializer, 
            FilterCriterionsContentService filterCriterionsContentService, MediaObjectContentService mediaObjectContentService)
        {
            CatalogStore = catalogStore;
            Deserializer = deserializer;
            FilterCriterionContentService = filterCriterionsContentService;
            MediaObjectContentService = mediaObjectContentService;
        }


        public void SelectRankingCategory(RankingCategory rankingCategory)
        {
            SelectedRankingCategory = rankingCategory;
            CatalogStore.CalibrateRankingCategory(SelectedRankingCategory);
        }

        public void DropSelectionOfRankingCategory()
        {
            FilterCriterionContentService.DropSelectionOfFilterCriterion();
            MediaObjectContentService.DropSelectionOfMEdiaObject();
            SelectedRankingCategory = null;
        }

        public IoCollection<RankingCategory> GetUnloadedRankingCategories()
        {
            IoCollection<RankingCategory> existingRankings = new IoCollection<RankingCategory>();
            existingRankings.Deserialize(Deserializer,CatalogStore.RankingCategoriesFilePath);
            return existingRankings;
        }

    }
}
