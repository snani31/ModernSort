using ModernSort.Commands;
using ModernSort.Services;
using ModernSort.Static;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ModernSort.ViewModel.Pages
{
    internal class SelectedMediaObjectPageViewModel : ViewModelBase
    {
        private OutputContentService ContentService { get; init; }
        public ICommand CopyPhotoToClipboard { get; set; }


        private string _selectedImagePath;

        public string SelectedImagePath
        {
            get
            {
                return
                _selectedImagePath;
            }
            set
            {
                _selectedImagePath = value;
                OnPropertyChenged(nameof(SelectedImagePath));
            }
        }

        private int _selectedPathIndex;
        public int SelectedPathIndex 
        {  
            get
            {
                return _selectedPathIndex;
            }
            set 
            {
                if (value < 0 || value > MediaFiles.Count - 1)
                {
                    OnPropertyChenged(nameof(SelectedPathIndex));
                    return;
                }
                _selectedPathIndex = value;
                OnPropertyChenged(nameof(SelectedPathIndex));
            }
        }

        public ICommand ScrollPresenterLeft { get; init; }
        public ICommand ScrollPresenterRight { get; init; }

        public ObservableCollection<string> MediaFiles
        {
            get
            {
                ObservableCollection<string> result = new ObservableCollection<string>(
                    ContentService.MediaObjectContentService.GetFilesFullPathsOfSelectedMediaObject());
                return result;
            } 
        }

        public SelectedMediaObjectPageViewModel(OutputContentService contentService)
        {
            ScrollPresenterRight = new RelayCommand( 
                (e) =>
                {
                    SelectedPathIndex++;
                }
                );

            ScrollPresenterLeft = new RelayCommand(
                (e) =>
                {
                    SelectedPathIndex--;
                }
                );

            CopyPhotoToClipboard = new RelayCommand(
                (e) =>
                {
                    ProjactIoWorker.CopyImageToClipboard(SelectedImagePath);
                }
                );
            ContentService = contentService;
            SelectedPathIndex = 0;
            SelectedImagePath = ContentService.MediaObjectContentService.GetFilesFullPathsOfSelectedMediaObject().First();
        }
    }
}
