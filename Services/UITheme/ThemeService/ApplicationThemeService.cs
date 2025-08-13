namespace ModernSort.Services.UITheme;

internal class ApplicationThemeService : IApplicationThemeService
{
    public IApplicationThemeState CurrentApplicationThemeState { get; set; }
    private IEnumerable<IApplicationThemeState> _applicationThemeStates = new List<ApplicationThemeState>();
    private readonly ApplicationThemeFileWorker _applicationThemeFileWorker;

    public ApplicationThemeService(string selectedThemeFilePath,
        IEnumerable<IApplicationThemeState> applicationThemeStates)
    {

        _applicationThemeFileWorker = new ApplicationThemeFileWorker(selectedThemeFilePath);
        _applicationThemeStates = applicationThemeStates;
    }

    public void SetSelectedApplicationTheme()
    {
        string selectedThemeName = _applicationThemeFileWorker.GetSelectedThemeStateNameFromThemeFile() ?? String.Empty;

        CurrentApplicationThemeState = new UnassignedApplicationThemeState(
            _applicationThemeStates.FirstOrDefault(x => x.Name.Equals(selectedThemeName)) ?? new DeepPurpleApplicationThemeState());

        CurrentApplicationThemeState.SetNextTheme(this);

    }

    public void SwitchApplicationthemeState()
    {
        CurrentApplicationThemeState.SetNextTheme(this);
        _applicationThemeFileWorker.UpdateSelectedThemeStateFile(CurrentApplicationThemeState.Name);
    }
}
