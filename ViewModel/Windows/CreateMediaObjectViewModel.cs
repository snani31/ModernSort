using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Static;
using ModernSort.ViewModel.Items;
using RankingEntityes.Filters;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    class CreateMediaObjectViewModel : ViewModelValidateble, IDialogRequestClose
    {

        private OutputContentService ContentService  { get; init; }
        private OperationService OperationService { get; init; }

        private ObservableCollection<Filter> _selectedFilters;
        public ObservableCollection<Filter> SelectedMatchFilters
        {
            get { return _selectedFilters; }
            private set
            {
                _selectedFilters = value;
            }
        }

        private ObservableCollection<FilterCriterion> _existingFilterCriterions;
        public ObservableCollection<FilterCriterion> ExistingFilterCriterions
        {
            get { return _existingFilterCriterions; }
            private set
            {
                _existingFilterCriterions = value;
            }
        }

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

        public bool DeleteSelectedMediaObjectsFiles { get; set; } = false;

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

        public ICommand AddFilterCommand  { get; }
        public ICommand RemoveFilterCommand { get; }
        public ICommand CloseDialogCommand { get; }
        public ICommand SelectMultimediaFiles { get; }
        public RelayCommand CreateMediaObjectCommand { get; }

        public CreateMediaObjectViewModel()
        {
            AddFilterCommand = new RelayCommand(AddToSelectedFilters);
            RemoveFilterCommand = new RelayCommand(RemoveFromSelectedFilters);

            SelectedMatchFilters = new ObservableCollection<Filter>();

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

        public CreateMediaObjectViewModel(OperationService operationService, OutputContentService contentService) 
            : this()
        {
            OperationService = operationService;

            ContentService = contentService;

            ExistingFilterCriterions = new ObservableCollection<FilterCriterion>(ContentService.FilterCriterionContentService.GetUnloadedFilterCriterions());
        }

        private void CreateMediaObjactMethod(object? parameter)
        {
            IEnumerable<string> selectedFileNames = SelectedFiles.Select(x => x.MediaImagePath);
            IOperation createMediaObjectOperation = new CreateMediaObjectOperation(selectedFileNames,Tytle,Descriptyon,SelectedMatchFilters
                ,removeSelectedFilesAfterCreating: DeleteSelectedMediaObjectsFiles); 

            bool MediaObjectCreationWasSuccesfullyCompleted =
                OperationService.InvokeOperation<RankingEntityes.Ranking_Entityes.MediaObjacts.MediaObject>(createMediaObjectOperation);

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


        private void AddToSelectedFilters(object? parameter)
        {
            if (parameter is Filter filter)
            {
                SelectedMatchFilters.Add(filter);
            }
        }

        private void RemoveFromSelectedFilters(object? parameter)
        {
            if (parameter is Filter filter)
            {
                SelectedMatchFilters.Remove(filter);
            }
        }
    }
}
