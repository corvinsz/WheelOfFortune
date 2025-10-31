using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using WheelOfFortune.Models;

namespace WheelOfFortune.ViewModels;

public partial class WheelViewModel : ObservableObject
{
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
			var dialog = new Dialogs.MessageBoxDialog("Overwrite Slices?", "Importing from clipboard will overwrite your current slices. Do you want to continue?", System.Windows.MessageBoxButton.YesNo);
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
}
