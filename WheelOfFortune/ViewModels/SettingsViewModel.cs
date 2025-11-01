using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using WheelOfFortune.Models;
using WheelOfFortune.ReleaseNotes;
using WheelOfFortune.Services;

namespace WheelOfFortune.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
	private readonly ISnackbarMessageQueue _snackbarMessageQueue;
	private readonly IThemeService _themeService;
	public List<Language> Languages { get; } =
	[
		new Language("English", "en", "pack://application:,,,/Assets/Flags/en.png"),
		new Language("Deutsch", "de", "pack://application:,,,/Assets/Flags/de.png"),
	];

	public SettingsViewModel(ISnackbarMessageQueue snackbarMessageService,
							 IThemeService themeService,
							 VelopackUpdaterViewModel velopackUpdater)
	{
		_snackbarMessageQueue = snackbarMessageService;
		_themeService = themeService;
		VelopackUpdater = velopackUpdater;
		SelectedTheme = ThemeOptions.First();
		SelectedLanguage = Languages.First();
	}

	public BaseTheme[] ThemeOptions { get; } = Enum.GetValues<BaseTheme>();
	public VelopackUpdaterViewModel VelopackUpdater { get; }

	[ObservableProperty]
	private BaseTheme _selectedTheme;

	[ObservableProperty]
	private Language _selectedLanguage;

	partial void OnSelectedLanguageChanged(Language value)
	{
		Localization.LocalizationManager.Instance.CurrentCulture = new System.Globalization.CultureInfo(value.Name);
	}

	partial void OnSelectedThemeChanged(BaseTheme value)
	{
		_themeService.SetTheme(value);
	}
}