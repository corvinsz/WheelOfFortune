using System.Windows.Media;

namespace WheelOfFortune.Models
{
    public class WheelSlice
    {
        public string Label { get; set; } = string.Empty;
        public Brush Fill { get; set; } = Brushes.LightGray;
    }
}
