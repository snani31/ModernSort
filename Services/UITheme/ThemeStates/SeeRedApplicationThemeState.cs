namespace ModernSort.Services.UITheme;

internal class SeeRedApplicationThemeState: ApplicationThemeState
{
    public SeeRedApplicationThemeState()
    {
        base.Name = "SeeRed";
        base.ThemeUri = new Uri("AppResources/ApplicationThemes/SeeRed.xaml", UriKind.Relative);
    }

    public override void SetNextTheme(IApplicationThemeService themeService)
    {
        base.NextThemeState = new DeepPurpleApplicationThemeState();
        base.SetNextApplicationThemeToResourceDictionary();
        themeService.CurrentApplicationThemeState = new DeepPurpleApplicationThemeState();
    }
}
