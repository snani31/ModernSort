using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Services.Operatios
{
    internal class RamoveMediaObjectOperation : RemoveOperation<MediaObject>
    {
        #region ViewModel Data
        private Guid SelectedMediaObjectID { get; set; }
        #endregion

        public RamoveMediaObjectOperation(IEnumerable<string> beforeEditFilePaths,
Guid selectedMediaObjectID)
        {
            BeforeEditFilePaths = beforeEditFilePaths;
            SelectedMediaObjectID = selectedMediaObjectID;
        }

        public IEnumerable<string> BeforeEditFilePaths { get; init; }

        public override void Remove()
        {

            base.EntityesCollection = new IoCollection<MediaObject>
                (EntityesCollection.Where(x => !x.ID.Equals(SelectedMediaObjectID)));

            foreach (var filePath in BeforeEditFilePaths)
            {
                File.Delete(filePath);
            }

        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.MediaObjectsFilePath;
        }
    }
}
