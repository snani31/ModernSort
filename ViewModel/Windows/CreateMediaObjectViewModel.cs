using ModernSort.Commands;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
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
        private OperationService OperationService { get; init; }

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
                CreateMediaObjactMethod,
                base.CanExecuteByValidation);
            base.PostValidationChange += CreateMediaObjectCommand.OnCanExecuteChanged;
        }

        public CreateMediaObjectViewModel(OperationService operationService) 
            : this()
        {
            OperationService = operationService;
        }

        private void CreateMediaObjactMethod(object? parameter)
        {
            IEnumerable<string> selectedFileNames = SelectedFiles.Select(x => x.MediaImagePath);
            IOperation createMediaObjectOperation = new CreateMediaObjectOperation(selectedFileNames,Tytle,Descriptyon); 

            bool MediaObjectCreationWasSuccesfullyCompleted = 
                OperationService.InvokeOperation<MediaObject>(createMediaObjectOperation);

            if (MediaObjectCreationWasSuccesfullyCompleted)
            {
                CloseRequested?.Invoke(this,new DialogCloseRequestedEventArgs(true));
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
