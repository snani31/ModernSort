using ModernSort.Enums;
using ModernSort.Stores.Catalog;
using System.IO;
using System.Windows;

namespace ModernSort.Services.UITheme
{
    internal class UIThemeService
    {
        private UIThemes SelectedTheme { get; set; }
        private UIThemes DefoultTheme { get; init; }
        private string SelectedThemeFilePath {  get; init; }
        public IDictionary<UIThemes, Uri> ThemeMapping { get; private set; }

        public UIThemeService(CatalogStore catalogStore, UIThemes defoultTheme,Uri defoultThemeUri)
        {
            ThemeMapping = new Dictionary<UIThemes, Uri>();
            SelectedThemeFilePath = catalogStore.SelectedUIThemeFilePath;
            DefoultTheme = defoultTheme;
            ThemeRegister(defoultTheme,defoultThemeUri);

            GetLastSelectedTheme();
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

            using (var fileStream = new FileStream(SelectedThemeFilePath
                , FileMode.Create
                , FileAccess.Write))
            using (var fileWriter = new StreamWriter(fileStream))
                fileWriter.Write(Enum.GetName<UIThemes>(SelectedTheme));

        }

        private void GetLastSelectedTheme()
        {
            string selectedThemeStr;

            using (var fileStream = new FileStream(SelectedThemeFilePath
                , FileMode.OpenOrCreate, FileAccess.Read))
            using (var filereader = new StreamReader(fileStream))
            {
                selectedThemeStr = filereader.ReadLine();
                filereader.Close();
            }

            object? stringResultOfSelectedTheme;
            if (Enum.TryParse(typeof(UIThemes), selectedThemeStr, true, out stringResultOfSelectedTheme))
            {
                SelectedTheme = (UIThemes)stringResultOfSelectedTheme;
            }
            else
            {
                SelectedTheme = DefoultTheme;
            }
        }

        public void SetApplicationThemeToSelected()
        {
            Uri uri = ThemeMapping.ContainsKey(SelectedTheme) ? ThemeMapping[SelectedTheme] : ThemeMapping[DefoultTheme];

            ResourceDictionary theme = new ResourceDictionary()
            {
                Source = uri,
            };
            App.Current.Resources.MergedDictionaries[0] = theme;
        }
    }
}
