using Microsoft.Win32;
using System.IO;

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
            _currentExecutableFileDirectoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
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

    }
}
