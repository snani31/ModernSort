using ModernSort.Static;
using ModernSort.Stores.Catalog;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using System.IO;

namespace ModernSort.Services.Operations
{
    internal class CreateMediaObjectOperation : CreateOperation
    {
        #region Catalog Data
        private string GUIDFilePath { get; set; } 

        private string MediaFilesCatalogPath { get; set; }
        #endregion

        #region VModel Data
        private IEnumerable<string> MediaObjectSelectedFiles { get; init; }
        private IEnumerable<Filter> SelectedMatchFilters { get; init; }
        private string Tytle { get; init; }

        private string Description { get; init; }
        #endregion

        public CreateMediaObjectOperation(IEnumerable<string> mediaObjectSelectedFiles, string tytle, string descryption, IEnumerable<Filter> selectedMatchFilters)
        {
            base.OperationResult = false;
            MediaObjectSelectedFiles = mediaObjectSelectedFiles;
            Tytle = tytle;
            Description = descryption;
            SelectedMatchFilters = selectedMatchFilters;
        }

        public override void Create(ISerializer serializer)
        {

            Guid id = ProjactIoWorker.GetUniqGuid(GUIDFilePath);

            List<string> existingFileNames = new List<string>(
                Directory.GetFiles(MediaFilesCatalogPath)
                .Select(x => Path.GetFileNameWithoutExtension(x)));

            List<string> newFilesFinalNames = new List<string>();

            CopyFilesToDirrectory(new Queue<string>(MediaObjectSelectedFiles),
               existingFileNames, ref newFilesFinalNames);

            RankingEntityes.Ranking_Entityes.MediaObjacts.MediaObject newMediaObjact = new RankingEntityes.Ranking_Entityes.MediaObjacts.MediaObject()
            {
                Description = this.Description,
                Tytle = this.Tytle,
                ID = id,
                Paths = newFilesFinalNames,
                MatchFilters = SelectedMatchFilters
            };

            base.OperationResult = newMediaObjact.Serialize(serializer,
                 base.FilePath,
                base.OperationFileMode);
        }
        /// <summary>
        ///  Метод позволяет проверять наличие файлов с тем же именем, что и следующий файл на копирование из очереди
        ///  , чтобы избежать конфликта имён файлов внутри конечного каталога</summary>
        /// <param name="copiedFilesPaths">Список полных путей к копируемым файлам</param>
        /// <param name="existingFileNames">Список имён уже существующих в заданной директории файлов</param>
        /// <param name="newFilesFinalNames">Итоговый список имён копируемых файлов, не создающих конфликтов имён в заданной директории</param>
        void CopyFilesToDirrectory(Queue<string> copiedFilesPaths, List<string> existingFileNames,
                ref List<string> newFilesFinalNames)
        {
            if (copiedFilesPaths.Count < 1) return;
            string currentFullFilePath = copiedFilesPaths.Dequeue();

            var primarFileName = new
            {
                FileExtention = Path.GetExtension(currentFullFilePath),
                FileName = Path.GetFileNameWithoutExtension(currentFullFilePath),
            };
            /*
             цикл работает до тех пор, пока в указанной директории существует файл с таким же именем, как 
            следующий в очереди
             */
            string dynamicFileName = primarFileName.FileName;
            while (existingFileNames.Any(x => x == dynamicFileName))
            {
                dynamicFileName = Path.GetRandomFileName();
            }

            File.Copy(currentFullFilePath,
                     MediaFilesCatalogPath + @$"\{dynamicFileName}{primarFileName.FileExtention}");
            existingFileNames.Add(dynamicFileName);
            newFilesFinalNames.Add(dynamicFileName + primarFileName.FileExtention);

            CopyFilesToDirrectory(copiedFilesPaths, existingFileNames, ref newFilesFinalNames);
        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.MediaObjectsFilePath;

            MediaFilesCatalogPath = catalogStore.MediaFilesCatalogPath;

            GUIDFilePath = catalogStore.GUIDsFilePath;
        }
    }
}
