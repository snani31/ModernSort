using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ModernSort.Converters
{
    internal class PathToBitmapImageConverter : IValueConverter
    {
        private FileStream BitmapConverterFileStream {  get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool fileNotFound = true;
            if (value is string imagePath 
                and not null 
                &&  !(fileNotFound = !File.Exists(imagePath)))
            {
                using (BitmapConverterFileStream = File.OpenRead(imagePath))
                {
                    var resultBitmapImage = new BitmapImage();
                    resultBitmapImage.BeginInit();
                    resultBitmapImage.StreamSource = BitmapConverterFileStream;
                    resultBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    resultBitmapImage.EndInit();
                    return resultBitmapImage;
                }
            }
            else
            {
                string ConvertProblemMessage = fileNotFound ? "not found": "incorrect format or null";
                throw new FileNotFoundException(message: $"File {nameof(imagePath)} was {ConvertProblemMessage}");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
