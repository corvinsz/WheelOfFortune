using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WheelOfFortune.ViewModels;

namespace WheelOfFortune;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly WheelViewModel _vm = new();

	private Canvas? _wheelLayer;
	private RotateTransform? _wheelRotateTransform;
	private double _currentAngle = 0.0;
	private bool _isSpinning = false;

	public MainWindow()
	{
		InitializeComponent();
		DataContext = _vm;
		SlicesList.ItemsSource = _vm.Slices;
		SlicesTextBox.TextChanged += SlicesTextBox_TextChanged;

		if (_vm.Slices is INotifyCollectionChanged obs)
		{
			obs.CollectionChanged += Slices_CollectionChanged;
		}
	}

	private void SlicesTextBox_TextChanged(object? sender, TextChangedEventArgs e)
	{
		_vm.SliceText = SlicesTextBox.Text;
	}

	private void Slices_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		DrawWheel();
	}

	private void WheelCanvas_Loaded(object sender, RoutedEventArgs e)
	{
		DrawWheel();
	}

	private void WheelCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		DrawWheel();
	}

	private void DrawWheel()
	{
		WheelCanvas.Children.Clear();
		_wheelLayer = new Canvas();

		var slices = _vm.Slices.ToList();
		if (slices.Count == 0)
		{
			// no slices to draw
			return;
		}

		double w = WheelCanvas.ActualWidth;
		double h = WheelCanvas.ActualHeight;
		double size = Math.Min(w, h) * 0.95;
		double centerX = w / 2;
		double centerY = h / 2;
		double radius = size / 2;

		_wheelLayer.Width = w;
		_wheelLayer.Height = h;

		_wheelRotateTransform = new RotateTransform(_currentAngle, centerX, centerY);
		_wheelLayer.RenderTransform = _wheelRotateTransform;

		double startAngle = -90;
		double anglePer = 360.0 / slices.Count;

		for (int i = 0; i < slices.Count; i++)
		{
			var slice = slices[i];
			double sweep = anglePer;
			double endAngle = startAngle + sweep;

			var path = CreatePieSlice(centerX, centerY, radius, startAngle, endAngle, slice.Fill);
			_wheelLayer.Children.Add(path);

			// Add label
			double midAngle = (startAngle + endAngle) / 2;
			double rad = midAngle * Math.PI / 180.0;
			double labelRadius = radius * 0.6;
			var text = new TextBlock
			{
				Text = slice.Label,
				Foreground = Brushes.White,
				FontWeight = FontWeights.SemiBold
			};
			var transform = new TranslateTransform(centerX + Math.Cos(rad) * labelRadius - 20, centerY + Math.Sin(rad) * labelRadius - 10);
			text.RenderTransform = transform;
			_wheelLayer.Children.Add(text);

			startAngle = endAngle;
		}

		// Add center circle on wheel layer
		var centerEllipse = new Ellipse
		{
			Width = radius * 0.3,
			Height = radius * 0.3,
			Fill = Brushes.White,
			Stroke = Brushes.Gray,
			StrokeThickness = 1
		};
		Canvas.SetLeft(centerEllipse, centerX - centerEllipse.Width / 2);
		Canvas.SetTop(centerEllipse, centerY - centerEllipse.Height / 2);
		_wheelLayer.Children.Add(centerEllipse);

		// Add wheel layer to canvas
		WheelCanvas.Children.Add(_wheelLayer);

		// Draw a pointer at the top center (outside the wheel) to indicate the selected slice
		var pointer = new Polygon
		{
			Fill = Brushes.Red,
			Stroke = Brushes.Black,
			StrokeThickness = 1,
			Points =
			[
				new Point(centerX - 10, centerY - radius - 4),
				new Point(centerX + 10, centerY - radius - 4),
				new Point(centerX, centerY - radius + 10)
			]
		};
		WheelCanvas.Children.Add(pointer);
	}

	private static Path CreatePieSlice(double cx, double cy, double r, double startAngle, double endAngle, Brush fill)
	{
		// Convert to radians
		double startRad = startAngle * Math.PI / 180.0;
		double endRad = endAngle * Math.PI / 180.0;

		var startPoint = new Point(cx + r * Math.Cos(startRad), cy + r * Math.Sin(startRad));
		var endPoint = new Point(cx + r * Math.Cos(endRad), cy + r * Math.Sin(endRad));

		bool largeArc = (endAngle - startAngle) > 180;

		var figure = new PathFigure { StartPoint = new Point(cx, cy) };
		figure.Segments.Add(new LineSegment(startPoint, true));
		var arc = new ArcSegment(endPoint, new Size(r, r), 0, largeArc, SweepDirection.Clockwise, true);
		figure.Segments.Add(arc);
		figure.Segments.Add(new LineSegment(new Point(cx, cy), true));

		var geo = new PathGeometry();
		geo.Figures.Add(figure);

		var path = new Path
		{
			Data = geo,
			Fill = fill,
			Stroke = Brushes.White,
			StrokeThickness = 1
		};

		return path;
	}

	private static double NormalizeAngle(double angle)
	{
		double a = angle % 360.0;
		if (a < 0) a += 360.0;
		return a;
	}

	private void SpinButton_Click(object sender, RoutedEventArgs e)
	{
		if (_isSpinning) return;
		if (_vm.Slices.Count == 0) return;

		if (_wheelLayer is null || _wheelRotateTransform is null)
		{
			DrawWheel();
		}

		// pick a random slice to land on
		int selectedIndex = Random.Shared.Next(0, _vm.Slices.Count);

		// compute target delta so that the selected slice mid-angle aligns with the pointer at -90
		double anglePer = 360.0 / _vm.Slices.Count;
		double midAngle = -90 + (selectedIndex + 0.5) * anglePer; // current mid angle for slice
		double targetDelta = NormalizeAngle(-90 - midAngle); // amount to rotate to bring midAngle to -90

		// pick random full rotations
		int fullRotations = Random.Shared.Next(3, 7);
		double totalRotation = fullRotations * 360.0 + targetDelta;

		double from = _currentAngle;
		double to = _currentAngle + totalRotation;

		var animation = new DoubleAnimation(from, to, new Duration(TimeSpan.FromSeconds(4 + Random.Shared.NextDouble() * 1.5)))
		{
			EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
		};

		animation.Completed += (s, a) =>
		{
			_currentAngle = NormalizeAngle(to);
			if (_wheelRotateTransform is not null)
			{
				_wheelRotateTransform.Angle = _currentAngle;
			}

			_isSpinning = false;

			// show result
			var slice = _vm.Slices[selectedIndex];
			MessageBox.Show($"Result: {slice.Label}", "Wheel Result", MessageBoxButton.OK, MessageBoxImage.Information);
		};

		// disable the button while spinning
		if (sender is Button btn)
		{
			btn.IsEnabled = false;
			animation.Completed += (s, a) => btn.IsEnabled = true;
		}

		_isSpinning = true;

		// start animation on the rotate transform
		_wheelRotateTransform?.BeginAnimation(RotateTransform.AngleProperty, animation);
	}
}