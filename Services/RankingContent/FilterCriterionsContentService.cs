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

namespace ModernSort.Services.RankingContent
{
    internal class FilterCriterionsContentService
    {
        public event Action<object?> OnFilterCriterionSelectedChange;
        private IDeserializer Deserializer { get; init; }
        public CatalogStore CatalogStore { get; init; }
        private FilterCriterion? _selectedFilterCriterion;
        public FilterCriterion? SelectedFilterCritetion
        {
            get => _selectedFilterCriterion;
            private set
            {
                _selectedFilterCriterion = value;
                OnFilterCriterionSelectedChange?.Invoke(value);
            }
        }

        private IoCollection<FilterCriterion> ExistingFilterCriterions { get; set; }

        public FilterCriterionsContentService(IDeserializer deserializer, CatalogStore catalogStore)
        {
            Deserializer = deserializer;
            CatalogStore = catalogStore;
            ExistingFilterCriterions = new IoCollection<FilterCriterion>();
        }

        public void SelectFilterCriterion(FilterCriterion filterCriterion)
        {
            SelectedFilterCritetion = ExistingFilterCriterions.First(x => x.ID.Equals(filterCriterion.ID)) ?? throw new ArgumentException();
        }

        public void DropSelectionOfFilterCriterion()
        {
            SelectedFilterCritetion = null;
        }

        public IoCollection<FilterCriterion> GetUnloadedFilterCriterions()
        {
            ExistingFilterCriterions.Deserialize(Deserializer, CatalogStore.FilterCriterionFilePath);
            return ExistingFilterCriterions;
        }

    }
}
