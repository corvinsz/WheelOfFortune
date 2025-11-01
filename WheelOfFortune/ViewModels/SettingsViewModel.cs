using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using WheelOfFortune.ReleaseNotes;
using WheelOfFortune.Services;

namespace WheelOfFortune.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
	private readonly ISnackbarMessageQueue _snackbarMessageQueue;
	private readonly IThemeService _themeService;

	public SettingsViewModel(ISnackbarMessageQueue snackbarMessageService,
							 IThemeService themeService,
							 VelopackUpdaterViewModel velopackUpdater)
	{
		_snackbarMessageQueue = snackbarMessageService;
		_themeService = themeService;
		VelopackUpdater = velopackUpdater;
		SelectedTheme = ThemeOptions.First();
	}

	public BaseTheme[] ThemeOptions { get; } = Enum.GetValues<BaseTheme>();
	public VelopackUpdaterViewModel VelopackUpdater { get; }

	[ObservableProperty]
	private BaseTheme _selectedTheme;

	partial void OnSelectedThemeChanged(BaseTheme value)
	{
		_themeService.SetTheme(value);
	}
}