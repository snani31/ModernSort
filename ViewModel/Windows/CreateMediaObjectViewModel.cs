using ModernSort.Commands;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    class CreateMediaObjectViewModel : ViewModelValidateble, IDialogRequestClose
    {
        private ObservableCollection<MediaFileSelectedViewModel> _selectedFiles;
        [MinLength(1, ErrorMessage = "You need to select just 1 file at least")]
        public ObservableCollection<MediaFileSelectedViewModel> SelectedFiles
        {
            get { return _selectedFiles; }
            private set
            {
                _selectedFiles = value;
                Validate(nameof(SelectedFiles), value);
            }
        }

        private string _title;

        [MaxLength(20, ErrorMessage = $"Tytle can not be bigger then 20 symbols")]
        public string Tytle
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Validate(nameof(Tytle), value);
            }
        }

        private string _descriptyon;

        [MaxLength(100, ErrorMessage = $"Descriptyon can not be bigger then 100 symbols")]
        public string Descriptyon
        {
            get
            {
                return _descriptyon;
            }
            set
            {
                _descriptyon = value;
                Validate(nameof(Descriptyon), value);
            }
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand CloseDialogCommand { get; }
        public ICommand SelectMultimediaFiles { get; }
        public RelayCommand CreateMediaObjectCommand { get; }

        private ISerializer Serializer { get; init; }
        private RankingCategory SelectedCategory { get; init; }
        private string SelectedCategoryMediaFilesDirectoryPath { 
            get 
            {
                return SelectedCategory.RankingDirrectoryPath + @"\Media";
            }
        }

        public CreateMediaObjectViewModel()
        {
            SelectedFiles = new ObservableCollection<MediaFileSelectedViewModel>();
            SelectedFiles.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                Validate(nameof(SelectedFiles), SelectedFiles);
            };

            CloseDialogCommand = new RelayCommand(
                (p) => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));

            SelectMultimediaFiles = new RelayCommand(
                (p) =>
                {
                    string[] newFilePaths = ProjactIoWorker.FilePickerGetImagePaths();
                    if (newFilePaths is not null && newFilePaths != Array.Empty<string>())
                    {
                        foreach (string filePath in newFilePaths)
                        {
                            SelectedFiles.Add(new MediaFileSelectedViewModel(filePath,
                new RelayCommand(RemoveMediafileFromList)));
                        }
                    }
                }
                );

            CreateMediaObjectCommand = new RelayCommand(
                CreateMediaObjact,
                base.CanExecuteByValidation);
            base.PostValidationChange += CreateMediaObjectCommand.OnCanExecuteChanged;
        }

        public CreateMediaObjectViewModel(ISerializer serializer, RankingCategory selectedRankingCategory) : this()
        {
            Serializer = serializer;
            SelectedCategory = selectedRankingCategory;
        }

        private void CreateMediaObjact(object? parameter)
        {
            try
            {
                Guid id = ProjactIoWorker.GetUniqGuid(ProjactIoWorker.UserResourcesDirrectoryPath
                    + @$"\{ProjactIoWorker.PROJACT_GUIDS_FILE}");

                List<string> existingFileNames = new List<string>(
                    Directory.GetFiles(SelectedCategoryMediaFilesDirectoryPath)
                    .Select(x => Path.GetFileNameWithoutExtension(x)));
                List<string> newFilesFinalNames = new List<string>();

                CopyFilesToDirrectory(new Queue<MediaFileSelectedViewModel>(SelectedFiles),
                   existingFileNames,ref newFilesFinalNames);

                MediaObject newMediaObjact = new MediaObject()
                {
                    Description = this.Descriptyon,
                    Tytle = this.Tytle,
                    ID = id,
                    Paths = newFilesFinalNames
                };
                //Если процесс сериализации нового медиа объекта прошел успешно - закрыть текущее окно с параметром true
                if (newMediaObjact.Serialize(Serializer,
                    SelectedCategory.RankingDirrectoryPath + @"\MediaObjacts.json",
                    FileMode.Append))
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /// Метод позволяет проверять наличие файлов с тем же именем, что и следующий файл на копирование из очереди
            /// , чтобы избежать конфликта имён файлов внутри конечного каталога
            void CopyFilesToDirrectory(Queue<MediaFileSelectedViewModel> copiedFiles,List<string> existingFileNames,
                ref List<string> newFilesFinalNames)
            {
                if (copiedFiles.Count <1) return;
                string currentFullFilePath = copiedFiles.Dequeue().MediaImagePath;

                var fileName = new
                    {  FileExtention = Path.GetExtension(currentFullFilePath),
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

                CopyFilesToDirrectory(copiedFiles,existingFileNames,ref newFilesFinalNames);
            }

        }

        private void RemoveMediafileFromList(object? parameter)
        {
            if (parameter is MediaFileSelectedViewModel mediafile and not null)
            {
                SelectedFiles.Remove(mediafile);
            }
        }
    }
}
