namespace ModernSort.Services.UITheme;

internal interface IApplicationThemeService
{
    public IApplicationThemeState CurrentApplicationThemeState { get; set; }
    void SetSelectedApplicationTheme();

    void SwitchApplicationthemeState();
}
