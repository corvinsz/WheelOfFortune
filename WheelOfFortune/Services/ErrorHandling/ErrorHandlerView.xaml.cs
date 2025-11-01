using System.Windows.Controls;

namespace WheelOfFortune.Services.ErrorHandling;

/// <summary>
/// Interaction logic for ErrorHandlerView.xaml
/// </summary>
public partial class ErrorHandlerView : UserControl
{
	public ErrorHandlerView(ErrorModel viewModel)
	{
		this.DataContext = viewModel;
		InitializeComponent();
	}
}

public record ErrorModel(string? Title, string? Message);