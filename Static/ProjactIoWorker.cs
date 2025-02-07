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
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Выберите изображение допустимого формата";
            ofd.Filter = "Изображения | *.jpg";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() ?? false)
            {
                return ofd.FileName;
            }
            else 
            {
                throw new Exception("Не удалось выбрать файл");
            }
        }
    }
}
