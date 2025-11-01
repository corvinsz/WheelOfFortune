using System.Globalization;

namespace WheelOfFortune.Models;

public class Language
{
	public Language(string friendlyName, string name, string imagePath)
	{
		FriendlyName = friendlyName;
		Name = name;
		CultureInfo = new CultureInfo(name);
		ImagePath = imagePath;
	}

	public string FriendlyName { get; }
	public string Name { get; }
	public CultureInfo CultureInfo { get; }
	public string ImagePath { get; }
}
