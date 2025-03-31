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
        private FilterCriterion NewFilterCriterion { get; init; }

        public CreateFilterCriterionOperation(FilterCriterion newFilterCriterion)
        {
            NewFilterCriterion = newFilterCriterion;
        }

        public override void Create(ISerializer serializer)
        {

            NewFilterCriterion.ID = ProjactIoWorker.GetUniqGuid(GUIDsFilePath);

            foreach (ConditionFilter filterCriterion in NewFilterCriterion.Filters)
            {
                filterCriterion.ID = ProjactIoWorker.GetUniqGuid(GUIDsFilePath);
            }

            
            OperationResult = NewFilterCriterion.Serialize(serializer,
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
