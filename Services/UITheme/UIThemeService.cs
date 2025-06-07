using ModernSort.Enums;
using System;
using System.IO;
using System.IO.Pipes;
using System.Windows;

namespace ModernSort.Services.UITheme
{
    public class UIThemeService
    {
        private UIThemes SelectedTheme { get; set; } = UIThemes.DeepPurple;

        public IDictionary<UIThemes, Uri> ThemeMapping { get; private set; }

        public UIThemeService()
        {
            ThemeMapping = new Dictionary<UIThemes, Uri>();
            

            string selectedThemeStr;
            using (var fileStream = new FileStream("C:\\Users\\vipar\\OneDrive\\Рабочий стол\\DataRanking\\Solution\\ModernSort\\bin\\Debug\\net9.0-windows\\UserResources\\SelectedUITheme.txt"
                , FileMode.OpenOrCreate, FileAccess.Read))
            using (var filereader = new StreamReader(fileStream))
            {
                selectedThemeStr = filereader.ReadLine() ?? "Pinapple";
                filereader.Close();
            }

            SelectedTheme = UIThemes.DeepPurple;
            object? assd;
            if (Enum.TryParse(typeof(UIThemes), selectedThemeStr,true,out assd))
            {
                SelectedTheme = (UIThemes)assd;
            }
        }

        public void ThemeRegister(UIThemes theme, Uri themeUri)
        {
            if (ThemeMapping.ContainsKey(theme))
            {
                throw new ArgumentException($"Key {theme} Was already mapped to the {ThemeMapping[theme]}");
            }
            ThemeMapping.Add(theme, themeUri);
        }

        public void ChangeTheme()
        {
            var firstTheme = Enum.GetValues(typeof(UIThemes)).Cast<UIThemes>().First();
            var lastTheme = Enum.GetValues(typeof(UIThemes)).Cast<UIThemes>().Last();

            Uri uri = SelectedTheme.Equals(lastTheme) ? ThemeMapping[SelectedTheme = firstTheme] : ThemeMapping[++SelectedTheme];

            ResourceDictionary theme = new ResourceDictionary()
            {
                Source = uri,
            };
            App.Current.Resources.MergedDictionaries[0] = theme;

            using (var fileStream = new FileStream("C:\\Users\\vipar\\OneDrive\\Рабочий стол\\DataRanking\\Solution\\ModernSort\\bin\\Debug\\net9.0-windows\\UserResources\\SelectedUITheme.txt"
                , FileMode.Create
                , FileAccess.Write))
            using (var fileWriter = new StreamWriter(fileStream))
                fileWriter.Write(Enum.GetName<UIThemes>(SelectedTheme));

        }

        public void SetApplicationThemeToSelected()
        {
            Uri uri = (ThemeMapping.ContainsKey(SelectedTheme)) ? ThemeMapping[SelectedTheme] : ThemeMapping[UIThemes.DeepPurple];

            ResourceDictionary theme = new ResourceDictionary()
            {
                Source = uri,
            };

            App.Current.Resources.MergedDictionaries[0] = theme;
        }
    }
}
