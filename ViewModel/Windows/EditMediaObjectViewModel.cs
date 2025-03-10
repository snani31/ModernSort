using ModernSort.Commands;
using ModernSort.Enums;
using ModernSort.Services.Dialog;
using ModernSort.Static;
using ModernSort.ViewModel.Items;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class EditMediaObjectViewModel : ViewModelValidateble, IDialogRequestClose
    {
        private FunktionPageEnum _pagePresenter;
        public FunktionPageEnum PagePresenter
        {
            get { return _pagePresenter; }
            private set
            {
                _pagePresenter = value;
                OnPropertyChenged(nameof(PagePresenter));
            }
        }
        private ISerializer Serializer { get; init; }
        private IDeserializer Deserializer { get; init; }

        private List<string> BeforeEditFilePaths { get; init; }

        private RankingCategory _selectedRankingCategory;

        private RankingCategory SelectedRankingCategory
        {
            get { return _selectedRankingCategory; }
            init { _selectedRankingCategory = value; }
        }

        private MediaObject _selectedMediaObject;

        private MediaObject SelectedMediaObject
        {
            get { return _selectedMediaObject; }
            init { _selectedMediaObject = value; }
        }
        private string _mediaObjectDescryption;

        [MaxLength(100, ErrorMessage = "Max Lenght for Descryption is 100 symbols")]
        public string MediaObjectDescryption
        {
            get { return _mediaObjectDescryption; }
            set
            {
                _mediaObjectDescryption = value;
                OnPropertyChenged(nameof(MediaObjectDescryption));
                Validate(nameof(MediaObjectDescryption), value);
            }
        }

        private string _mediaObjectTytle;

        [MaxLength(30, ErrorMessage = "Max Lenght for Tytle is 30 symbols")]
        public string MediaObjectTytle
        {
            get { return _mediaObjectTytle; }
            set
            {
                _mediaObjectTytle = value;
                OnPropertyChenged(nameof(MediaObjectTytle));
                Validate(nameof(MediaObjectTytle), value);
            }
        }

        private ObservableCollection<string> _selectedFilePaths;

        [MinLength(1, ErrorMessage = "You need to select just 1 file at least")]
        public ObservableCollection<string> SelectedFilePaths
        {
            get { return _selectedFilePaths; }
            set
            {
                _selectedFilePaths = value;
                Validate(nameof(SelectedFilePaths), value);
            }
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public ICommand CloseDialog { get; init; }
        public ICommand DeleteMediaObjact { get; init; }
        public ICommand SelectMultimediaFiles { get; init; }
        public ICommand RemoveFileFromList {  get; init; }
        public RelayCommand EditMediaObject { get; init; }
        private string SelectedCategoryMediaFilesDirectoryPath { 
            get 
            {
                return SelectedRankingCategory.RankingDirrectoryPath + "\\" + "Media";
            } 
        }

        public EditMediaObjectViewModel()
        {
            CloseDialog = new RelayCommand(
                (p) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                });

            DeleteMediaObjact = new RelayCommand(DeleteMediaObjactMethod);

            SelectMultimediaFiles = new RelayCommand(
               (p) =>
               {
                   string[] newFilePaths = ProjactIoWorker.FilePickerGetImagePaths();
                   if (newFilePaths is not null && newFilePaths != Array.Empty<string>())
                   {
                       foreach (string filePath in newFilePaths)
                       {
                           SelectedFilePaths.Add(filePath);
                       }
                   }
               }
               );
            RemoveFileFromList = new RelayCommand(RemoveFileFromListMethod);

            EditMediaObject = new RelayCommand(EditMediaObjact, CanExecuteByValidation);
            base.PostValidationChange += EditMediaObject.OnCanExecuteChanged;


        }

        internal EditMediaObjectViewModel(RankingCategory rankingCategory, MediaObject mediaObject, ISerializer serializer, IDeserializer deserializer) : this()
        {
            Serializer = serializer;
            Deserializer = deserializer;
            SelectedMediaObject = mediaObject;
            SelectedRankingCategory = rankingCategory;

            MediaObjectTytle = SelectedMediaObject.Tytle;
            MediaObjectDescryption = SelectedMediaObject.Description;
            PagePresenter = FunktionPageEnum.FilesPresent;

            BeforeEditFilePaths = SelectedMediaObject.Paths.Select(x => @$"{SelectedRankingCategory.RankingDirrectoryPath}\Media\{x}").ToList();
            SelectedFilePaths = new ObservableCollection<string>(BeforeEditFilePaths);

            SelectedFilePaths.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                Validate(nameof(SelectedFilePaths), SelectedFilePaths);
            };

        }

        private void DeleteMediaObjactMethod(object? parameter)
        {
            string jsonFilePath = SelectedRankingCategory.RankingDirrectoryPath + "\\"
                + ProjactIoWorker.MEDIA_OBJECTS_JSON;


            var existingMediaObjects = new IoCollection<MediaObject>();
            existingMediaObjects.Deserialize(Deserializer, jsonFilePath);

            existingMediaObjects = new IoCollection<MediaObject>
                (existingMediaObjects.Where(x => !x.ID.Equals(SelectedMediaObject.ID)));

            if (existingMediaObjects.Serialize(Serializer, jsonFilePath,
                mode: FileMode.OpenOrCreate))
            {
                foreach (var filePath in BeforeEditFilePaths)
                {
                    File.Delete(filePath);
                }
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
            Validate(nameof(SelectedFilePaths), SelectedFilePaths);
        }

        private void RemoveFileFromListMethod(object? parameter)
        {
            if (parameter is string mediafile and not null)
            {
                SelectedFilePaths.Remove(mediafile);
            }
        }

        private void EditMediaObjact(object? parameter)
        {
            try
            {

                #region existingFilesWork
                List<string> existingFileNames = new List<string>(
                            Directory.GetFiles(SelectedCategoryMediaFilesDirectoryPath)
                            .Select(x => Path.GetFileNameWithoutExtension(x)));

                List<string> newFilesFinalNames = new List<string>();

                CopyFilesToDirrectory(new Queue<string>(SelectedFilePaths),
                  existingFileNames, ref newFilesFinalNames);
                #endregion


                #region ExistingMediaObjectsWork
                var existingMediaObjects = new IoCollection<MediaObject>();
                existingMediaObjects.Deserialize(Deserializer,
                    SelectedRankingCategory.RankingDirrectoryPath + @"\MediaObjacts.json");

                int indexOfSelectedMediaObject = existingMediaObjects.Select((member, index) => (member, index))
                    .First(x => x.member.ID.Equals(SelectedMediaObject.ID)).index;

                existingMediaObjects[indexOfSelectedMediaObject] = new MediaObject()
                {
                    ID = SelectedMediaObject.ID,
                    Description = MediaObjectDescryption,
                    Tytle = MediaObjectTytle,
                    Paths = newFilesFinalNames
                }; 
                #endregion



                if (existingMediaObjects.Serialize(Serializer,
                    SelectedRankingCategory.RankingDirrectoryPath + @"\MediaObjacts.json",
                    mode: FileMode.OpenOrCreate))
                {
                    foreach (var filePath in BeforeEditFilePaths)
                    {
                        File.Delete(filePath);
                    }
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /// Метод позволяет проверять наличие файлов с тем же именем, что и следующий файл на копирование из очереди
            /// , чтобы избежать конфликта имён файлов внутри конечного каталога
            void CopyFilesToDirrectory(Queue<string> copiedFiles, List<string> existingFileNames,
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

        }
    }
}
