using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.Resources;

namespace WheelOfFortune.Localization
{
	public partial class LocalizationManager : ObservableObject
	{
		public static LocalizationManager Instance { get; } = new();

		private readonly ResourceManager _resourceManager = Strings.ResourceManager;

		[ObservableProperty]
		private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

		partial void OnCurrentCultureChanged(CultureInfo value)
		{
			Strings.Culture = value;
			OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Item[]"));
		}

		public string this[string key] =>
			_resourceManager.GetString(key, _currentCulture) ?? $"!{key}!";
	}
}
