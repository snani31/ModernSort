using ModernSort.Services.Operations;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Services.Operation.Classes.FilterCriteron
{
    class CreateFilterCriterionOperation : CreateOperation
    {
        private string GUIDsFilePath { get ; set; }
        private IFilterCriterion NewFilterCriterion { get; init; }
        private string Tytle { get; init; }
        private string Description { get; init; }

        public CreateFilterCriterionOperation(IFilterCriterion newFilterCriterion,
            string tytle,string description)
        {
            NewFilterCriterion = newFilterCriterion;
            Tytle = tytle;
            Description = description;
        }

        public override void Create(ISerializer serializer)
        {
            var conditionFilterCriterion = NewFilterCriterion as ConditionFilterCriterion;

            conditionFilterCriterion.Description = Description;
            conditionFilterCriterion.Tytle = Tytle;
            conditionFilterCriterion.ID = ProjactIoWorker.GetUniqGuid(GUIDsFilePath);

            foreach (ConditionFilter filterCriterion in conditionFilterCriterion.Filters)
            {
                filterCriterion.ID = ProjactIoWorker.GetUniqGuid(GUIDsFilePath);
            }

            OperationResult = conditionFilterCriterion.Serialize(serializer,
                base.FilePath,
                OperationFileMode);
        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.FilterCriterionFilePath;
            GUIDsFilePath = catalogStore.GUIDsFilePath;
        }
    }
}
