using Microsoft.Win32;
using System.IO;
using System.IO.Pipelines;
using static System.Net.WebRequestMethods;

namespace ModernSort.Static
{
    internal static class ProjactIoWorker
    {
        private static OpenFileDialog _fileDialog;
        /// <summary>
        /// Константа содержит имя директории, в которой будут соджержаться все ресурсы пользователя проекта
        /// А именно - директории каждой созданной категории ранжира и их медиа ресурсов, вместе с json 
        /// файлами категорий ранжира, критериев фильтрации, фильтрами и медиа объектами
        /// </summary>
        internal const string USER_RESOURCES_DIRECTORY_NAME = "UserResources";
        /// <summary>
        /// Константа содержит имя любой иконки для категории ранжира, за исключением формата изображения
        /// </summary>
        internal const string RANKING_CATEGORY_ICON_TYTLE = "Ranking_Icon";
        /// <summary>
        /// Константа содержит имя файла, хранящего состояние всех категорий ранжира проекта в формате json
        /// </summary>
        internal const string RANKING_CATEGORIES_JSON = "RankingCategories.json";
        internal const string PROJACT_GUIDS_FILE = "ProjactGUIDSFile.txt";
        private static readonly string _currentExecutableFileDirectoryPath;
        internal static string UserResourcesDirrectoryPath 
        {
            get 
            {
                if (!Directory.Exists(_currentExecutableFileDirectoryPath + $@"\{USER_RESOURCES_DIRECTORY_NAME}"))
                {
                    Directory.CreateDirectory(_currentExecutableFileDirectoryPath + $@"\{USER_RESOURCES_DIRECTORY_NAME}");
                }
                return _currentExecutableFileDirectoryPath + $@"\{USER_RESOURCES_DIRECTORY_NAME}";
            }
        }
        static ProjactIoWorker()
        {
            _fileDialog = new OpenFileDialog();
            _currentExecutableFileDirectoryPath = 
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        internal static string FilePickerGetImage()
        {
            _fileDialog.Title = "Выберите изображение допустимого формата";
            _fileDialog.Filter = "Изображения | *.jpg";
            _fileDialog.Multiselect = false;
            if (_fileDialog.ShowDialog() ?? false)
            {
                return _fileDialog.FileName;
            }
            else 
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// Метод возвращает значение GUID, если такого не было в указанном файле
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static Guid GetUniqGuid(string path)
        {
            FileStream file;
            var guid = Guid.NewGuid();
            List<string> existingGUIDs = new List<string>();

            using (file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {

                using (var filereader = new StreamReader(file))
                {

                    while (filereader.Peek() >= 0)
                    {
                        existingGUIDs.Add(filereader.ReadLine() ?? String.Empty);
                    }
                    filereader.Close();
                }
            }

            using (file = new FileStream(path,FileMode.Append,FileAccess.Write))
            {
                using (var fileWriter = new StreamWriter(file))
                {
                    switch (existingGUIDs.Any(x => x == guid.ToString()))
                    {
                        case true:
                            fileWriter.Close();
                            fileWriter.Dispose();
                            file.Dispose(); 
                            return GetUniqGuid(path);
                            break;
                        case false:
                            fileWriter.WriteLine(guid.ToString());
                            fileWriter.Close();
                            fileWriter.Dispose();
                            file.Dispose();
                            return guid;
                            break;
                    }
                }
            }
        }

    }
}
