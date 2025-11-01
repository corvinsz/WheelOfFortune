using MaterialDesignThemes.Wpf;

namespace WheelOfFortune.Services;

public interface IThemeService
{
	void SetTheme(BaseTheme theme);
}

public class ThemeService : IThemeService
{
	public void SetTheme(BaseTheme newTheme)
	{
		var paletteHelper = new PaletteHelper();
		var theme = paletteHelper.GetTheme();
		theme.SetBaseTheme(newTheme);
		paletteHelper.SetTheme(theme);
	}
}