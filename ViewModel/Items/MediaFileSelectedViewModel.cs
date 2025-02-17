using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernSort.ViewModel.Items
{
    internal class MediaFileSelectedViewModel : ViewModelBase
    {

        public string MediaImagePath { get; set; }
        public ICommand RemoveMediafile { get; }

        public MediaFileSelectedViewModel(string mediaImagePath,ICommand removeMediafile)
        {
            MediaImagePath = mediaImagePath;
            RemoveMediafile = removeMediafile;
        }
    }
}
