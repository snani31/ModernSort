namespace ModernSort.Services.UITheme;

internal class PinappleApplicationThemeState : ApplicationThemeState
{
    public PinappleApplicationThemeState()
    {
        base.Name = "Pinapple";
        base.ThemeUri = new Uri("AppResources/ApplicationThemes/PinappleTheme.xaml", UriKind.Relative);
    }

    public override void SetNextTheme(IApplicationThemeService themeService)
    {
        base.NextThemeState = new SeeRedApplicationThemeState();
        base.SetNextApplicationThemeToResourceDictionary();
        themeService.CurrentApplicationThemeState = new SeeRedApplicationThemeState();
    }
}
