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
        /// <summary>
        /// Константа содержит имя файла, хранящего значения всех ранее присвоенных сущностям значений GUID
        /// </summary>
        internal const string PROJACT_GUIDS_FILE = "ProjactGUIDSFile.txt";
        /// <summary>
        /// Константа содержит имя Json файла, состояние всех существующих медиа-объектов выбранной кагерории
        /// </summary>
        internal const string MEDIA_OBJECTS_JSON = "MediaObjacts.json";

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

    }
}
