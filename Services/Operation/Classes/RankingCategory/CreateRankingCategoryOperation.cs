using ModernSort.Stores.Catalog;
using ModernSort.Static;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal class CreateRankingCategoryOperation : CreateOperation
    {
        #region VModel Data
        public string Tytle { get; set; }

        public string Descryption { get; set; }

        public string SelectedIconPath { get; set; }
        #endregion

        #region Catalog Data

        private string GUIDsFilePath { get; set; }
        private string CoreResourcesCatalogPath { get; set; }
        private string RankingCategoryIconNameNoExtention { get; set; }
        private string MediaObjectsFileName { get; set; }
        private string MediaFilesCatalogName { get; set; }

        #endregion

        public CreateRankingCategoryOperation(string tytle, string descryption, string selectedIconPath)
        {
            OperationResult = false;
            Tytle = tytle;
            Descryption = descryption;
            SelectedIconPath = selectedIconPath;
        }

        public override void Create(ISerializer serializer)
        {
            Guid id = ProjactIoWorker.GetUniqGuid(GUIDsFilePath);

            string newRankingDirrectoryPath = CoreResourcesCatalogPath
                + @$"\{id.ToString()}";

            string newRankingIconPath = newRankingDirrectoryPath
                + @$"\{RankingCategoryIconNameNoExtention}{Path.GetExtension(SelectedIconPath)}";



            Directory.CreateDirectory(newRankingDirrectoryPath);
            File.Copy(SelectedIconPath, newRankingIconPath);
            Directory.CreateDirectory(newRankingDirrectoryPath + $@"\{MediaFilesCatalogName}");
            using(File.Create(newRankingDirrectoryPath + $@"\{MediaObjectsFileName}")) { }

            
            RankingCategory newRanking = new RankingCategory()
            {
                Description = Descryption,
                Tytle = Tytle,
                ID = id,
                RankingDirrectoryPath = newRankingDirrectoryPath,
                RankingIconPath = newRankingIconPath
            };

            OperationResult = newRanking.Serialize(serializer,
                base.FilePath,
                OperationFileMode);
        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.RankingCategoriesFilePath;
            MediaFilesCatalogName = catalogStore.mediaFilesCatalogName;
            MediaObjectsFileName = catalogStore.mediaObjectsFileName;
            RankingCategoryIconNameNoExtention = catalogStore.rankingCategoryIconNameNoExtention;
            CoreResourcesCatalogPath = catalogStore.coreResourcesCatalogPath;
            GUIDsFilePath = catalogStore.GUIDsFilePath;

        }
    }
}
