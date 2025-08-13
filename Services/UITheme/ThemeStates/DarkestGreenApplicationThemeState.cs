
namespace ModernSort.Services.UITheme;

internal class DarkestGreenApplicationThemeState : ApplicationThemeState
{
    public DarkestGreenApplicationThemeState()
    {
        base.Name = "DarkestGreen";
        base.ThemeUri = new Uri("AppResources/ApplicationThemes/DarkestGreen.xaml", UriKind.Relative);
    }

    public override void SetNextTheme(IApplicationThemeService themeService)
    {
        base.NextThemeState = new PinappleApplicationThemeState();
        base.SetNextApplicationThemeToResourceDictionary();
        themeService.CurrentApplicationThemeState = new PinappleApplicationThemeState();
    }
}
