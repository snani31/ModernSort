using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Static;
using ModernSort.ViewModel.Items;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    class CreateMediaObjectViewModel : ViewModelValidateble, IDialogRequestClose
    {
        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        private readonly ISerializer _serializer;
        private ObservableCollection<MediaFileSelectedViewModel> _selectedFiles;

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

        [MaxLength(30, ErrorMessage = $"Descriptyon can not be bigger then 30 symbols")]
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

        public ActionCommand CloseDialogCommand { get; }
        public ActionCommand SelectMultimediaFiles { get; }
        public ActionCommand CreateMediaObjectCommand { get; }

        public CreateMediaObjectViewModel()
        {
            SelectedFiles = new ObservableCollection<MediaFileSelectedViewModel>();
            SelectedFiles.CollectionChanged += SelectedMediaCollectionChanged;

            CloseDialogCommand = new ActionCommand(
                () => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));

            SelectMultimediaFiles = new ActionCommand(
                () => 
                {
                    SelectedFiles.Add(new MediaFileSelectedViewModel(ProjactIoWorker.FilePickerGetImage(),
                new ParameterCommand(RemoveMediafileFromList)));
                    Validate(nameof(SelectedFiles), SelectedFiles);
                }
                );

            CreateMediaObjectCommand = new ActionCommand(
                CreateMediaObjact,
                base.CanExecuteByValidation);
            base.PostValidationChange += CreateMediaObjectCommand.OnCanExecuteChanged;
        }

        public CreateMediaObjectViewModel(ISerializer serializer,RankingCategory selectedRankingCategory): this()
        {
            _serializer = serializer;
        }

        private void CreateMediaObjact()
        {

        }

        private void RemoveMediafileFromList(object? parametr)
        {
            if (parametr is MediaFileSelectedViewModel mediafile and not null)
            {
                SelectedFiles.Remove(mediafile);
            }
        }

        private void SelectedMediaCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Validate(nameof(SelectedFiles), SelectedFiles);
        }

    }
}
