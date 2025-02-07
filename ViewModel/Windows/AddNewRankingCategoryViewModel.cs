using ModernSort.Commands;
using ModernSort.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.ViewModel.Windows
{
    internal class AddNewRankingCategoryViewModel: ViewModelBase
    {
        private string _selectedImagePath;
        public string SelectedImagePath 
        {
            get
            {
                return _selectedImagePath;
            }
            private set 
            {
                _selectedImagePath = value;
                OnPropertyChenged(nameof(SelectedImagePath));
            } 
        }
        public Action BackToPastWindow {  get; init; }
        public ICommand SelectImageFile { get; init; }
        public AddNewRankingCategoryViewModel() 
        {
            SelectImageFile = new OpenNewWindowCommand(SelectImage);
        }

        private void SelectImage()
        {
            SelectedImagePath = ProjactIoWorker.FilePickerGetImage();
        }
    }
}
