namespace ModernSort.Services.UITheme;

internal interface IApplicationThemeState
{
    public string Name { get; protected init; }
    public Uri ThemeUri { get; protected init; }
    void SetNextTheme(IApplicationThemeService themeService);
}
