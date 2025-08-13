namespace ModernSort.Services.UITheme;

internal class UnassignedApplicationThemeState : ApplicationThemeState
{
    public UnassignedApplicationThemeState(IApplicationThemeState nextApplicationThemeState)
    {
        base.NextThemeState = nextApplicationThemeState;
    }

    public override void SetNextTheme(IApplicationThemeService themeService)
    {
        base.SetNextApplicationThemeToResourceDictionary();
        themeService.CurrentApplicationThemeState = base.NextThemeState;
    }
}
