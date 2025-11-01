using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WheelOfFortune.Models;
using WheelOfFortune.Services;

namespace WheelOfFortune.ViewModels;

public partial class MainViewModel : ObservableObject
{
	private readonly IDialogService _dialogService;
	public ISnackbarMessageQueue SnackbarMessageQueue { get; }
	public MainViewModel(IDialogService dialogService, ISnackbarMessageQueue snackbarMessageQueue)
	{
		SelectedLanguage = Languages.First();
		_dialogService = dialogService;
		SnackbarMessageQueue = snackbarMessageQueue;
	}

	public List<string> Languages { get; } =
	[
		"en",
		"de",
	];

	[ObservableProperty]
	private string _selectedLanguage;

	partial void OnSelectedLanguageChanged(string value)
	{
		Localization.LocalizationManager.Instance.CurrentCulture = new System.Globalization.CultureInfo(value);
	}

	public ObservableCollection<HistoryEntry> History { get; } = [];
	public ObservableCollection<WheelSlice> Slices { get; } = [];

	[ObservableProperty]
	private string _sliceText = string.Empty;

	private readonly Brush[] _palette =
	[
		Brushes.Coral,
		Brushes.MediumSeaGreen,
		Brushes.SteelBlue,
		Brushes.Goldenrod,
		Brushes.MediumPurple,
		Brushes.Tomato,
		Brushes.Teal,
		Brushes.Plum,
		Brushes.SandyBrown,
		Brushes.OliveDrab
	];


	partial void OnSliceTextChanged(string value)
	{
		UpdateSlicesFromText();
	}

	private void UpdateSlicesFromText()
	{
		Slices.Clear();
		if (string.IsNullOrWhiteSpace(SliceText))
		{
			return;
		}

		var lines = SliceText.Split(['\r', '\n'], System.StringSplitOptions.RemoveEmptyEntries);
		int i = 0;
		foreach (var line in lines)
		{
			var slice = new WheelSlice
			{
				Label = line.Trim(),
				Fill = _palette[i % _palette.Length]
			};
			Slices.Add(slice);
			i++;
		}
	}

	[RelayCommand]
	private async Task ImportFromClipboard()
	{
		if (Slices.Count > 0)
		{
			var dialog = new Dialogs.MessageBoxDialog(Localization.LocalizationManager.Instance["OverwriteSlicesDialog_HeaderTextBlock_Text"],
													  Localization.LocalizationManager.Instance["OverwriteSlicesDialog_ContentTextBlock_Text"],
													  System.Windows.MessageBoxButton.YesNo);
			MessageBoxResult? shouldImport = (MessageBoxResult?)await DialogHost.Show(dialog);

			if (shouldImport != MessageBoxResult.Yes)
			{
				return;
			}
		}

		SliceText = Clipboard.GetText();
	}

	[RelayCommand]
	private void ShuffleSilces()
	{
		// TODO: implement
	}

	[RelayCommand]
	private async Task OpenSettings()
	{
		var view = App.AppServices.GetRequiredService<Dialogs.SettingsDialog>();
		await _dialogService.Show(view);
	}
}
