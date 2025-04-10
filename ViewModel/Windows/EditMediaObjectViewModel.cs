using ModernSort.Commands;
using ModernSort.Enums;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Services.Operatios;
using ModernSort.Static;
using ModernSort.ViewModel.Items;
using ModernSort.ViewModel.Items.FiltrationItems;
using RankingEntityes.Filters;
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
        private List<string> BeforeEditFilePaths { get; init; }

        private List<Filter> SelectedFilters { get; init; }

        public OperationService OperationService { get; }
        public OutputContentService ContentService { get; }

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

        public ObservableCollection<FilterCriterionItemViewModel> FilterCriterions { get; init; }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public ICommand CloseDialog { get; init; }
        public ICommand DeleteMediaObjact { get; init; }
        public ICommand SelectMultimediaFiles { get; init; }
        public ICommand RemoveFileFromList {  get; init; }
        public RelayCommand EditMediaObject { get; init; }
        public ICommand ShowMediaFilesPage { get; init; }
        public ICommand ShowFiltersPage { get; init; }
        public ICommand RemoveFilterFromSelection { get; init; }
        public ICommand AddFilterToSelection { get; init; }

        public EditMediaObjectViewModel()
        {
            CloseDialog = new RelayCommand(
                (p) =>
                {
                    CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false));
                });

            ShowFiltersPage = new RelayCommand(
                (p) =>
                {
                    PagePresenter = FunktionPageEnum.FiltersPresent;
                });

            RemoveFilterFromSelection = new RelayCommand(RemoveFilterFromSelectionCommandMethod);

            AddFilterToSelection = new RelayCommand(AddFilterToSelectionCommandMethod);

            ShowMediaFilesPage = new RelayCommand(
                (p) =>   
                {
                    PagePresenter = FunktionPageEnum.FilesPresent;
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

        internal EditMediaObjectViewModel(OperationService operationService,OutputContentService contentService) : this()
        {
            OperationService = operationService;
            ContentService = contentService;

            MediaObjectTytle = contentService.MediaObjectContentService.SelectedMediaObject.Tytle;
            MediaObjectDescryption = contentService.MediaObjectContentService.SelectedMediaObject.Description;
            PagePresenter = FunktionPageEnum.FilesPresent;

            BeforeEditFilePaths = new List<string>(ContentService.MediaObjectContentService.GetFilesFullPathsOfSelectedMediaObject());
            SelectedFilePaths = new ObservableCollection<string>(BeforeEditFilePaths);

            SelectedFilePaths.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                Validate(nameof(SelectedFilePaths), SelectedFilePaths);
            };


            var allExistingFilterCriterionsItemsVM = ContentService.FilterCriterionContentService
                .GetUnloadedFilterCriterions()
                .Select(x => new FilterCriterionItemViewModel(x));

            FilterCriterions = new ObservableCollection<FilterCriterionItemViewModel>(allExistingFilterCriterionsItemsVM);

            foreach (var filterItemTmp in FilterCriterions.SelectMany(filterItem => filterItem.FilterItems).ToArray())
            {
                filterItemTmp.SelectFilterIfContains(ContentService.MediaObjectContentService.SelectedMediaObject.MatchFilters);
            }

            SelectedFilters = new List<Filter>(ContentService.MediaObjectContentService.SelectedMediaObject.MatchFilters);

        }

        private void DeleteMediaObjactMethod(object? parameter)
        {
            IOperation operation = new RamoveMediaObjectOperation(BeforeEditFilePaths,ContentService.MediaObjectContentService.SelectedMediaObject.ID);

            bool MediaObjectRemoveWasSuccsesfullyCompleted = OperationService.InvokeOperation<MediaObject>(operation);

            if (MediaObjectRemoveWasSuccsesfullyCompleted)
            {
                ContentService.MediaObjectContentService.DropSelectionOfMEdiaObject();
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }

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
            IOperation operation = new UpdateMediaObjectOperation(tytle:MediaObjectTytle,
                description: MediaObjectDescryption,
                ContentService.MediaObjectContentService.SelectedMediaObject.ID,
                BeforeEditFilePaths,
                SelectedFilePaths,
                SelectedFilters);

            bool MediaObjectUpdateWasSuccsesfullyCompleted = OperationService.InvokeOperation<MediaObject>(operation);

            if (MediaObjectUpdateWasSuccsesfullyCompleted)
            {
                ContentService.MediaObjectContentService.DropSelectionOfMEdiaObject();
                CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true));
            }
        }

        private void AddFilterToSelectionCommandMethod(object? parameter)
        {
            if (parameter is ConditionFilterItemViewModel filterItem)
            {
                SelectedFilters.Add(filterItem.Filter);
            }
        }

        private void RemoveFilterFromSelectionCommandMethod(object? parameter)
        {
            if (parameter is ConditionFilterItemViewModel filterItem)
            {
                SelectedFilters.Remove(filterItem.Filter);
            }
        }
    }
}
