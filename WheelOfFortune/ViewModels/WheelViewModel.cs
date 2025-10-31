using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using WheelOfFortune.Models;

namespace WheelOfFortune.ViewModels;

public partial class WheelViewModel : ObservableObject
{
	public ObservableCollection<WheelSlice> Slices { get; } = [];

	[ObservableProperty]
	private string sliceText = string.Empty;

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
}
