using System.Windows.Controls;
using WheelOfFortune.ViewModels;

namespace WheelOfFortune.Dialogs;

/// <summary>
/// Interaction logic for SettingsDialog.xaml
/// </summary>
public partial class SettingsDialog : UserControl
{
	public SettingsDialog(SettingsViewModel viewModel)
	{
		DataContext = viewModel;
		InitializeComponent();
	}
}
