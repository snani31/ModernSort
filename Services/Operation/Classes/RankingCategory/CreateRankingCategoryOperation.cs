using ModernSort.Stores.Catalog;
using ModernSort.Static;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal class CreateRankingCategoryOperation : CreateOperation
    {
        private FileMode OperationFileModemode { get; set; } = FileMode.Append;

        public string Tytle { get; set; }

        public string Descryption { get; set; }

        public string SelectedIconPath { get; set; }

        public CreateRankingCategoryOperation(string tytle, string descryption, string selectedIconPath)
        {
            Tytle = tytle;
            Descryption = descryption;
            SelectedIconPath = selectedIconPath;
        }

        public override void Create(ISerializer serializer, CatalogStore catalogStore)
        {
            OperationResult = false;


            Guid id = ProjactIoWorker.GetUniqGuid(catalogStore.GUIDsFilePath);

            string newRankingDirrectoryPath = catalogStore.coreResourcesCatalogPath
                + @$"\{id.ToString()}";

            string newRankingIconPath = newRankingDirrectoryPath
                + @$"\{catalogStore.rankingCategoryIconNameNoExtention}{Path.GetExtension(SelectedIconPath)}";

            Directory.CreateDirectory(newRankingDirrectoryPath);
            File.Copy(SelectedIconPath, newRankingIconPath);
            Directory.CreateDirectory(newRankingDirrectoryPath + $@"\{catalogStore.mediaFilesCatalogName}");
            File.Create(newRankingDirrectoryPath + $@"\{catalogStore.mediaObjectsFileName}");

            RankingCategory newRanking = new RankingCategory()
            {
                Description = Descryption,
                Tytle = Tytle,
                ID = id,
                RankingDirrectoryPath = newRankingDirrectoryPath,
                RankingIconPath = newRankingIconPath
            };


            OperationResult = newRanking.Serialize(serializer,
                catalogStore.RankingCategoriesFilePath,
                OperationFileModemode);
        }
    }
}
