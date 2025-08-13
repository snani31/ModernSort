using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSort.Services.UITheme
{
    internal class ApplicationThemeFileWorker
    {
        private readonly string _themeFilePath;
        public ApplicationThemeFileWorker(string themeFilePath)
        {
            _themeFilePath = themeFilePath;
        }

        public void UpdateSelectedThemeStateFile(string newThemeStateValue)
        {
            using (var fileStream = new FileStream(_themeFilePath
                   , FileMode.Create
                   , FileAccess.Write))
            using (var fileWriter = new StreamWriter(fileStream))
                fileWriter.Write(newThemeStateValue);
        }


        public string? GetSelectedThemeStateNameFromThemeFile()
        {
            string? selectedThemeNameResult = null;
            using (var fileStream = new FileStream(_themeFilePath
                    , FileMode.OpenOrCreate, FileAccess.Read))
            using (var filereader = new StreamReader(fileStream))
            {
                selectedThemeNameResult = filereader.ReadLine();
                filereader.Close();
            }
            return selectedThemeNameResult;
        }
    }
}
