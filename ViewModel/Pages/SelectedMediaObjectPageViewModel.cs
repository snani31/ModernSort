using ModernSort.Commands;
using ModernSort.Services;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernSort.ViewModel.Pages
{
    internal class SelectedMediaObjectPageViewModel : ViewModelBase
    {
        private OutputContentService ContentService { get; init; }

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
            ContentService = contentService;
        }
    }
}
