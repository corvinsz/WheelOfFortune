using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using WheelOfFortune.Models;

namespace WheelOfFortune.ViewModels;

    public class WheelViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WheelSlice> Slices { get; } = new ObservableCollection<WheelSlice>();

        private string _sliceText = string.Empty;
        public string SliceText
        {
            get => _sliceText;
            set
            {
                if (_sliceText == value) return;
                _sliceText = value;
                OnPropertyChanged();
                UpdateSlicesFromText();
            }
        }

        private readonly Brush[] _palette = new Brush[]
        {
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
        };

        private void UpdateSlicesFromText()
        {
            Slices.Clear();
            if (string.IsNullOrWhiteSpace(SliceText)) return;
            var lines = SliceText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
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
            OnPropertyChanged(nameof(Slices));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
