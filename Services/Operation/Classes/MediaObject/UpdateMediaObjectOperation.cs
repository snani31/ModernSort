using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Stores.Catalog;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Services.Operations
{
    internal class UpdateMediaObjectOperation : UpdateOperation<MediaObject>
    {

        #region ViewModel Data
        private Guid SelectedMediaObjectGuid { get; init; }
        private string Tytle { get; init; }
        private string Description { get; init; }
        private IEnumerable<string> BeforeEditFilePaths { get; init; }
        private IEnumerable<string> SelectedFilesPaths { get; }
        private IEnumerable<string> NewAddedFilesPaths { get; }
        private bool RemoveNewAddedFilesAfterCreating { get; }
        public IEnumerable<Filter> SelectedFilters { get; }
        #endregion

        #region Catalog Data
        private string SelectedCategoryMediaFilesDirectoryPath { get; set; }
        #endregion

        public UpdateMediaObjectOperation(string tytle, string description, Guid selectedMediaObjectGuid,
            IEnumerable<string> beforeEditFilePaths, IEnumerable<string> selectedFilesPaths, 
            IEnumerable<Filter> selectedFilters)
        {
            this.Tytle = tytle;
            this.Description = description;
            this.BeforeEditFilePaths = beforeEditFilePaths;
            SelectedFilesPaths = selectedFilesPaths;
            SelectedMediaObjectGuid = selectedMediaObjectGuid;
            SelectedFilters = selectedFilters;
        }

        public UpdateMediaObjectOperation(string tytle, string description, Guid selectedMediaObjectGuid,
            IEnumerable<string> beforeEditFilePaths, IEnumerable<string> selectedFilesPaths,
            IEnumerable<Filter> selectedFilters, IEnumerable<string> newAddedFilesPaths = null, 
            bool removeNewAddedFilesAfterCreating = false) 
            : this(tytle, description, selectedMediaObjectGuid, beforeEditFilePaths, selectedFilesPaths, selectedFilters)
        {
            NewAddedFilesPaths = newAddedFilesPaths;
            RemoveNewAddedFilesAfterCreating = removeNewAddedFilesAfterCreating;
        }

        public override void Update()
        {
            List<string> existingFileNames = new List<string>(
                            Directory.GetFiles(SelectedCategoryMediaFilesDirectoryPath)
                            .Select(x => Path.GetFileNameWithoutExtension(x)));

            List<string> newFilesFinalNames = new List<string>();

            CopyFilesToDirrectory(new Queue<string>(SelectedFilesPaths),
              existingFileNames, ref newFilesFinalNames);


            #region ExistingMediaObjectsWork


            int indexOfSelectedMediaObject = base.EntityesCollection.Select((member, index) => (member, index))
                .First(x => x.member.ID.Equals(SelectedMediaObjectGuid)).index;

            EntityesCollection[indexOfSelectedMediaObject] = new MediaObject()
            {
                ID = SelectedMediaObjectGuid,
                Description = this.Description,
                Tytle = this.Tytle,
                Paths = newFilesFinalNames,
                MatchFilters = SelectedFilters
            };
            #endregion
            foreach (var filePath in BeforeEditFilePaths)
            {
                File.Delete(filePath);
            }

            if (RemoveNewAddedFilesAfterCreating && NewAddedFilesPaths.Count() > 0)
            {
                DeleteSelectedMediaObjectsFiles();
            }
        }

        private void CopyFilesToDirrectory(Queue<string> copiedFiles, List<string> existingFileNames,
               ref List<string> newFilesFinalNames)
        {
            if (copiedFiles.Count < 1) return;
            string currentFullFilePath = copiedFiles.Dequeue();

            var fileName = new
            {
                FileExtention = Path.GetExtension(currentFullFilePath),
                FileName = Path.GetFileNameWithoutExtension(currentFullFilePath),
            };
            /*
             цикл работает до тех пор, пока в указанной директории существует файл с таким же именем, как 
            следующий в очереди
             */
            string name = fileName.FileName;
            while (existingFileNames.Any(x => x == name))
            {
                name = Path.GetRandomFileName();
            }

            File.Copy(currentFullFilePath,
                    SelectedCategoryMediaFilesDirectoryPath + @$"\{name}{fileName.FileExtention}");
            existingFileNames.Add(name);
            newFilesFinalNames.Add(name + fileName.FileExtention);

            CopyFilesToDirrectory(copiedFiles, existingFileNames, ref newFilesFinalNames);
        }

        public override void SetCatalogData(CatalogStore catalogStore)
        {
            base.FilePath = catalogStore.MediaObjectsFilePath;
            SelectedCategoryMediaFilesDirectoryPath = catalogStore.MediaFilesCatalogPath;
        }

        /// <summary>
        /// Метод позволяет удалить все выбранные пользователем для медиа-объекта файлы в изначальной папке
        /// </summary>
        void DeleteSelectedMediaObjectsFiles()
        {
            foreach (var filePath in NewAddedFilesPaths)
            {
                File.Delete(filePath);
            }
        }
    }
}
