using CommunityToolkit.Mvvm.Messaging;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Windows.Threading;
using Velopack;
using Velopack.Sources;

namespace WheelOfFortune;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public static IServiceProvider AppServices { get; private set; }
	public static bool IsFirstRun { get; private set; } = false;

	[STAThread]
	private static void Main(string[] args)
	{
		// Velopack bootstrap: handles install/update hooks early
		VelopackApp.Build()
			.OnFirstRun(v =>
			{
				IsFirstRun = true;
			})
			.Run();

		// After bootstrap, continue with host + WPF startup
		MainAsync(args).GetAwaiter().GetResult();
	}

	private static async Task MainAsync(string[] args)
	{
		using IHost host = CreateHostBuilder(args).Build();
		await host.StartAsync().ConfigureAwait(true);

		AppServices = host.Services;

		var app = new App();
		app.InitializeComponent();
		app.MainWindow = host.Services.GetRequiredService<MainWindow>();
		app.MainWindow.Visibility = Visibility.Visible;

		// Check for updates automatically on startup
		_ = Task.Run(async () =>
		{
			try
			{
				const string repoUrl = "https://github.com/corvinsz/WheelOfFortune";
				UpdateManager updateManager = new(new GithubSource(repoUrl, "", true));
				if (updateManager.IsInstalled)
				{
					var updateInfo = await updateManager.CheckForUpdatesAsync();
					if (updateInfo is not null)
					{
						await updateManager.DownloadUpdatesAsync(updateInfo);
						updateManager.ApplyUpdatesAndRestart(updateInfo);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
			}
		});

		app.Run();

		await host.StopAsync().ConfigureAwait(true);
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((ctx, cfg) =>
				cfg.AddUserSecrets(typeof(App).Assembly))
			.ConfigureServices((ctx, services) =>
			{
				services.AddSingleton(_ => Current.Dispatcher);
				services.AddSingleton<ISnackbarMessageQueue>(sp =>
				{
					return new SnackbarMessageQueue(TimeSpan.FromSeconds(3), sp.GetRequiredService<Dispatcher>());
				});
				services.AddSingleton<Services.IThemeService, Services.ThemeService>();
				services.AddSingleton<Services.IDialogService, Services.DialogService>();
				services.AddSingleton<Services.ErrorHandling.IErrorHandler, Services.ErrorHandling.ErrorHandler>();

				services.AddSingleton<ReleaseNotes.VelopackUpdaterViewModel>();

				services.AddSingleton<MainWindow>();
				services.AddSingleton<ViewModels.MainViewModel>();

				// register other feature viewmodels / views...
				services.AddSingleton<ViewModels.SettingsViewModel>();
				services.AddSingleton<Dialogs.SettingsDialog>();

				services.AddSingleton<WeakReferenceMessenger>();
				services.AddSingleton<IMessenger>(prov => prov.GetRequiredService<WeakReferenceMessenger>());
			});
}