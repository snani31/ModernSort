
namespace ModernSort.Services.UITheme;

internal class DeepPurpleApplicationThemeState : ApplicationThemeState
{
    public DeepPurpleApplicationThemeState()
    {
        base.Name = "DeepPurple";
        base.ThemeUri = new Uri("AppResources/ApplicationThemes/DeepPurpleTheme.xaml", UriKind.Relative);
    }

    public override void SetNextTheme(IApplicationThemeService themeService)
    {
        base.NextThemeState = new DarkestGreenApplicationThemeState();
        base.SetNextApplicationThemeToResourceDictionary();
        themeService.CurrentApplicationThemeState = new DarkestGreenApplicationThemeState();
    }
}
 