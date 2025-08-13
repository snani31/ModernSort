using System.Windows;

namespace ModernSort.Services.UITheme;

internal abstract class ApplicationThemeState : IApplicationThemeState
{
    protected IApplicationThemeState NextThemeState { get; set; }

    public string Name { get; init; }
    public Uri ThemeUri { get; init; }

    public abstract void SetNextTheme(IApplicationThemeService themeService);

    protected virtual void SetNextApplicationThemeToResourceDictionary()
    {
        ResourceDictionary theme = new ResourceDictionary()
        {
            Source = NextThemeState.ThemeUri,
        };
        App.Current.Resources.MergedDictionaries[0] = theme;
    }

}
