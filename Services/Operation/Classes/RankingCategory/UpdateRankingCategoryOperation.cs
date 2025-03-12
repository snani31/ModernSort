using ModernSort.Stores.Catalog;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal class UpdateRankingCategoryOperation : UpdateOperation
    {
        private FileMode OperationFileMode { get; set; } = FileMode.OpenOrCreate;
        private string Tytle { get; init; }
        private string Description { get; init; }
        private string NewImageFileBasePath { get; init; }
        private bool IconWasChanged { get; init; }

        public UpdateRankingCategoryOperation(string tytle, string description, string newImageFileBasePath, bool iconWasChanged)
        {
            Tytle = tytle;
            Description = description;
            NewImageFileBasePath = newImageFileBasePath;
            IconWasChanged = iconWasChanged;
        }

        public override void Update(ISerializer serializer, IDeserializer deserializer, CatalogStore catalogStore)
        {
            OperationResult = false;

            var selectedRankingCategoryID = catalogStore.SelectedRankingCategory.ID;


            var existingRankings = new IoCollection<RankingCategory>();
            existingRankings.Deserialize(deserializer, catalogStore.RankingCategoriesFilePath);

            int indexOfSelectedRanking = existingRankings.Select((member, index) => (member, index))
                .First(x => x.member.ID.Equals(selectedRankingCategoryID)).index;


            string newIconInCatalogPath = catalogStore.SelectedRankingCategory.RankingIconPath;
            if (IconWasChanged)
            {
                File.Delete(catalogStore.SelectedRankingCategory.RankingIconPath);
                newIconInCatalogPath = Path.ChangeExtension(catalogStore.SelectedRankingCategory.RankingIconPath, Path.GetExtension(NewImageFileBasePath));
                File.Copy(NewImageFileBasePath, newIconInCatalogPath, overwrite: true);
            }

            existingRankings[indexOfSelectedRanking] = new RankingCategory()
            {
                ID = selectedRankingCategoryID,
                Description = Description,
                RankingDirrectoryPath = catalogStore.RankingCatalogPath,
                Tytle = Tytle,
                RankingIconPath = newIconInCatalogPath
            };

            OperationResult = existingRankings.Serialize(serializer,
                catalogStore.RankingCategoriesFilePath,
                mode: OperationFileMode);
        }
    }
}
