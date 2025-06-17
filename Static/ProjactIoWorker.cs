using Microsoft.Win32;
using System.IO;
using System.IO.Pipelines;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace ModernSort.Static
{
    internal static class ProjactIoWorker
    {
        private static OpenFileDialog _fileDialog;

        static ProjactIoWorker()
        {
            _fileDialog = new OpenFileDialog();
        }

        internal static string FilePickerGetImagePathScalar()
        {
            _fileDialog.Title = "Выберите изображение допустимого формата";
            _fileDialog.Filter = "images |*.jpg;*.jpeg;*.png;*.gif;*.tif;*.jfif;";
            _fileDialog.Multiselect = false;

            var result = (_fileDialog.ShowDialog() ?? false) ? _fileDialog.FileName : String.Empty;
            return result;
        }
        internal static string[] FilePickerGetImagePaths()
        {
            _fileDialog.Title = "Выберите изображение допустимого формата";
            _fileDialog.Filter = "images |*.jpg;*.jpeg;*.png;*.gif;*.tif;*.jfif;";
            _fileDialog.Multiselect = true;

            var result = (_fileDialog.ShowDialog() ?? false) ? _fileDialog.FileNames : Array.Empty<string>();
            return result;
        }
        /// <summary>
        /// Метод возвращает значение GUID, если такого не было в указанном файле
        /// </summary>
        /// <param name="GUIDsSaveFilePath"></param>
        /// <returns></returns>
        internal static Guid GetUniqGuid(string GUIDsSaveFilePath)
        {
            FileStream fileStream;
            var guid = Guid.NewGuid();
            List<string> existingGUIDs = new List<string>();

            using (fileStream = new FileStream(GUIDsSaveFilePath, FileMode.OpenOrCreate, FileAccess.Read))
            using (var filereader = new StreamReader(fileStream))
            {

                while (filereader.Peek() >= 0)
                {
                    existingGUIDs.Add(filereader.ReadLine() ?? String.Empty);
                }
                filereader.Close();
            }


            using (fileStream = new FileStream(GUIDsSaveFilePath, FileMode.Append, FileAccess.Write))
            using (var fileWriter = new StreamWriter(fileStream))
                switch (existingGUIDs.Any(x => x == guid.ToString()))
                {
                    case true:
                        fileWriter.Close();
                        fileWriter.Dispose();
                        fileStream.Dispose();
                        return GetUniqGuid(GUIDsSaveFilePath);
                    case false:
                        fileWriter.WriteLine(guid.ToString());
                        fileWriter.Close();
                        fileWriter.Dispose();
                        fileStream.Dispose();
                        return guid;
                }


        }

        internal static void ControlRequiredFilesExistence(IEnumerable<string> dirrectoryPaths, IEnumerable<string> filePaths)
        {

            try
            {
                foreach (var dirrectoryPath in dirrectoryPaths)
                {
                    if (!Directory.Exists(dirrectoryPath))
                    {
                        Directory.CreateDirectory(dirrectoryPath);
                    }
                }

                foreach (var filePath in filePaths)
                {
                    if (!System.IO.File.Exists(filePath))
                    {
                        using (System.IO.File.Create(filePath));
                    }
                }

            }
            catch
            {
                MessageBox.Show("Can not create base dirrectory files");
            }
        }

    }
}
