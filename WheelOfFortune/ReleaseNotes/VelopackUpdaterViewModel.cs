using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Velopack;
using Velopack.Sources;
using WheelOfFortune.Services.ErrorHandling;

namespace WheelOfFortune.ReleaseNotes;

public partial class VelopackUpdaterViewModel : ObservableObject
{
	private readonly IErrorHandler _errorHandler;
	private readonly ISnackbarMessageQueue _snackbarMessageQueue;
	private readonly UpdateManager _um;
	private UpdateInfo? _update;

	public VelopackUpdaterViewModel(IErrorHandler errorHandler,
									ISnackbarMessageQueue snackbarMessageQueue)
	{
		_errorHandler = errorHandler;
		_snackbarMessageQueue = snackbarMessageQueue;

		const string repoUrl = "https://github.com/corvinsz/WheelOfFortune";
		_um = new UpdateManager(new GithubSource(repoUrl, "", true));
		CurrentAppVersion = _um.CurrentVersion?.ToString() ?? "n.a.";
	}

	public bool IsUpdateAvailable => _update is not null;
	public bool IsInstalled => _um.IsInstalled;

	[ObservableProperty]
	private int _downloadProgress = 0;

	[ObservableProperty]
	private string _currentAppVersion;

	[RelayCommand]
	private async Task CheckForUpdate(bool showMessages)
	{
		if (!IsInstalled)
		{
			if (showMessages)
			{
				_snackbarMessageQueue.Enqueue(Localization.LocalizationManager.Instance["Snackbar_AppIsNotInstalled"]);
			}
			return;
		}

		try
		{
			_update = await _um.CheckForUpdatesAsync().ConfigureAwait(true);
			OnPropertyChanged(nameof(IsUpdateAvailable));

			if (showMessages)
			{
				if (IsUpdateAvailable)
				{
					string msg = $"{Localization.LocalizationManager.Instance["Snackbar_UpdateAvailable"]}: {_update?.TargetFullRelease.Version}";
					_snackbarMessageQueue.Enqueue(msg);
				}
				else
				{
					_snackbarMessageQueue.Enqueue(Localization.LocalizationManager.Instance["Snackbar_NoUpdatesAvailable"]);
				}
			}

		}
		catch (Exception ex)
		{
			if (IsUpdateAvailable)
			{
				_errorHandler.HandleError(ex);
			}
			// TODO: Log error
			//App.Log.LogError(ex, "Error checking for updates");
			//EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error);
		}
	}

	[RelayCommand]
	private async Task ApplyUpdateAndRestart(bool showMessages)
	{
		if (_update is null)
		{
			return;
		}

		try
		{
			await _um.DownloadUpdatesAsync(_update, (progress) => DownloadProgress = progress).ConfigureAwait(true);
			_um.ApplyUpdatesAndRestart(_update);
		}
		catch (Exception ex)
		{
			if (showMessages)
			{
				_errorHandler.HandleError(ex);
			}
			// TODO: Log error
			// App.Log.LogError(ex, "Error downloading updates");
		}
	}

	[RelayCommand]
	private async Task CheckAndApplyUpdate()
	{
		await CheckForUpdate(false).ConfigureAwait(true);
		if (IsUpdateAvailable)
		{
			await ApplyUpdateAndRestart(false).ConfigureAwait(true);
		}
	}
}