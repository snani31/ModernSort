using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace ModernSort.Static
{
    internal static class ProjactIoWorker
    {
        static ProjactIoWorker()
        {

        }

        internal static string FilePickerGetImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Выберите изображение допустимого формата";
            fileDialog.Filter = "Изображения | *.jpg";
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() ?? false)
            {
                return fileDialog.FileName;
            }
            else 
            {
                return String.Empty;
            }
        }
    }
}
