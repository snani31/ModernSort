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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    class CreateMediaObjectViewModel : ViewModelValidateble, IDialogRequestClose
    {
        private readonly ISerializer _serializer;
        private readonly RankingCategory _selectedCategory;
        private ObservableCollection<MediaFileSelectedViewModel> _selectedFiles;
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        [MinLength(1,ErrorMessage = "You need to select just 1 file at least")]
        public ObservableCollection<MediaFileSelectedViewModel> SelectedFiles 
        {
            get { return _selectedFiles; }
            private set 
            { _selectedFiles = value;
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

        public ICommand CloseDialogCommand { get; }
        public ICommand SelectMultimediaFiles { get; }
        public RelayCommand CreateMediaObjectCommand { get; }

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
                    SelectedFiles.Add(new MediaFileSelectedViewModel(ProjactIoWorker.FilePickerGetImage(),
                new RelayCommand(RemoveMediafileFromList)));
                    Validate(nameof(SelectedFiles), SelectedFiles);
                }
                );

            CreateMediaObjectCommand = new RelayCommand(
                CreateMediaObjact,
                base.CanExecuteByValidation);
            base.PostValidationChange += CreateMediaObjectCommand.OnCanExecuteChanged;
        }

        public CreateMediaObjectViewModel(ISerializer serializer,RankingCategory selectedRankingCategory): this()
        {
            _serializer = serializer;
            _selectedCategory = selectedRankingCategory;
        }

        private void CreateMediaObjact(object? parameter)
        {
            try
            {
                Guid id = ProjactIoWorker.GetUniqGuid(ProjactIoWorker.UserResourcesDirrectoryPath
                    + @$"\{ProjactIoWorker.PROJACT_GUIDS_FILE}");

                foreach(var file in SelectedFiles)
                {
                    File.Copy(file.MediaImagePath,
                        _selectedCategory.RankingDirrectoryPath + @"\Media" + @$"\{Path.GetFileName(file.MediaImagePath)}");
                }

                MediaObject newMediaObjact = new MediaObject()
                {
                    Description = this.Descriptyon,
                    Tytle = this.Tytle,
                    ID = id,
                    Paths = new List<string>(SelectedFiles.Select(x => Path.GetFileName(x.MediaImagePath)))
                };
                //Если процесс сериализации нового медиа объекта прошел успешно - закрыть текущее окно с параметром true
                if (newMediaObjact.Serialize(_serializer, _selectedCategory.RankingDirrectoryPath + @"\MediaObjacts.json"))
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveMediafileFromList(object? parametr)
        {
            if (parametr is MediaFileSelectedViewModel mediafile and not null)
            {
                SelectedFiles.Remove(mediafile);
            }
        }
    }
}
