using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using TimeTracker.Localization;
using TimeTracker.TimePublishing;
using TimeTracking.ApplicationServices.Dialogs;
using TimeTracking.ApplicationServices.Settings;
using TimeTracking.Extensions;
using TimeTracking.Extensions.Exceptions;
using TimeTracking.Infrastructure;
using TimeTracking.Logging;
using TimeTracking.ReadModel;

namespace TimeTracker
{
	public class MainViewModel : ViewModelBase
	{
		private readonly ILocalizationService localizationService;
		private readonly IMessageBoxService messageBox;
		private readonly ISettingsService settingsService;
		private readonly ICommandBus commandBus;
		private IDisposable refresh;

		private const int maxAttemptsToLoadStatistics = 5;
		private int currentAttempt;

		private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

		private ICommand startStopCommand;
		private ICommand refreshStatisticsCommand;
		private string totalTime;
		private string totalThisWeek;
		private string totalThisMonth;

		private bool isPaused;

		private ITimeTrackingViewModel timeTrackingViewModel;

		public ICommand RefreshStatisticsCommand
		{
			get
			{
				if (refreshStatisticsCommand == null)
				{
					refreshStatisticsCommand = new RelayCommand(RefreshStatistics);
				}

				return refreshStatisticsCommand;
			}
		}

		public string TotalThisMonth
		{
			get { return totalThisMonth; }
			private set
			{
				if (totalThisMonth == value)
				{
					return;
				}

				totalThisMonth = value;
				RaisePropertyChanged(() => TotalThisMonth);
			}
		}

		public string TotalThisWeek
		{
			get { return totalThisWeek; }
			private set
			{
				if (totalThisWeek == value)
				{
					return;
				}

				totalThisWeek = value;
				RaisePropertyChanged(() => TotalThisWeek);
			}
		}

		public string TotalTime
		{
			get { return totalTime; }
			private set
			{
				if (totalTime == value)
				{
					return;
				}

				totalTime = value;
				RaisePropertyChanged(() => TotalTime);
			}
		}

		public ITimeTrackingViewModel TimeTrackingViewModel
		{
			get { return timeTrackingViewModel; }
			set
			{
				if (timeTrackingViewModel == value)
				{
					return;
				}

				timeTrackingViewModel = value;
				RaisePropertyChanged(() => TimeTrackingViewModel);
			}
		}

		public string ActionHeader
		{
			get
			{
				return TimeTrackingViewModel.Maybe(vm => vm.IsStarted)
					? localizationService.GetLocalizedString("StopButton")
					: localizationService.GetLocalizedString("StartButton");
			}
		}

		public ICommand StartStopCommand
		{
			get
			{
				if (startStopCommand == null)
				{
					startStopCommand = new RelayCommand(StartOrStop, CanStartOrStop);
				}

				return startStopCommand;
			}
		}

		private bool CanStartOrStop()
		{
			return TimeTrackingViewModel != null;
		}

		public MainViewModel()
		{
			localizationService = ServiceLocator.Current.GetInstance<ILocalizationService>();
			messageBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
			settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
			commandBus = ServiceLocator.Current.GetInstance<ICommandBus>();

			SystemEvents.PowerModeChanged += PowerModeChanged;
			SystemEvents.SessionSwitch += SessionSwitch;

			StartLoading();
		}

		public override void Cleanup()
		{
			SystemEvents.PowerModeChanged -= PowerModeChanged;
			SystemEvents.SessionSwitch -= SessionSwitch;

			refresh.MaybeDo(r => r.Dispose());

			base.Cleanup();

			TimeTrackingViewModel.MaybeDo(ttvm => ttvm.Stop());
			TimeTrackingViewModel.MaybeDo(ttvm => ttvm.Cleanup());
		}

		private void StartLoading()
		{
			try
			{
				var vm = new TimeTrackingViewModel(settingsService,
					commandBus,
					messageBox,
					localizationService);

				vm.PropertyChanged += OnViewModelPropertyChanged;
				TimeTrackingViewModel = vm;

				refresh = Observable.Interval(TimeSpan.FromSeconds(5))
					.Subscribe(l => RefreshTotals());

				CommandManager.InvalidateRequerySuggested();
			}
			catch (Exception ex)
			{
				throw new UnrecoverableApplicationException(ex.Message, ex);
			}
		}

		private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			TimeTrackingViewModel vm = null;

			if (e.PropertyName.PropertyNameIn(new Expression<Func<object>>[] {() => vm.IsStarted}))
			{
				RaisePropertyChanged(() => ActionHeader);
			}
		}

		private void SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason.In(new[] {SessionSwitchReason.SessionLock, SessionSwitchReason.SessionLogoff})
				&& TimeTrackingViewModel.Maybe(t => t.IsStarted))
			{
				LogHelper.Debug(string.Format("Pausing time tracking at {0} by {1}",
					DateTime.Now,
					Enum.GetName(typeof(SessionSwitchReason), e.Reason)));

				PauseTracking();
			}
			else if (e.Reason == SessionSwitchReason.SessionUnlock && isPaused)
			{
				LogHelper.Debug(string.Format("Resuming time tracking at {0} by session unlock", DateTime.Now));

				ResumeTracking();
			}
		}

		private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			if (e.Mode == PowerModes.Suspend && TimeTrackingViewModel.Maybe(t => t.IsStarted))
			{
				LogHelper.Debug(string.Format("Pausing time tracking at {0} by system suspend", DateTime.Now));

				PauseTracking();
			}
			else if (e.Mode == PowerModes.Resume && isPaused)
			{
				LogHelper.Debug(string.Format("Resuming time tracking at {0} by system resume", DateTime.Now));

				ResumeTracking();
			}
		}

		private void ResumeTracking()
		{
			TimeTrackingViewModel.MaybeDo(t =>
				{
					t.Start();
					isPaused = false;
				});
		}

		private void PauseTracking()
		{
			TimeTrackingViewModel.MaybeDo(t =>
				{
					t.Stop();
					isPaused = true;
				});
		}

		private void StartOrStop()
		{
			LogHelper.Debug(string.Format("Change tracking state by user request at {0}", DateTime.Now));

			TimeTrackingViewModel.MaybeDo(t =>
				{
					t.StartOrStop();
					RaisePropertyChanged(() => ActionHeader);
				});
		}

		private void SetTotalTime(TimeSpan totalTimeForPeriod)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
			{
				TotalTime = string.Format("{0:D2}:{1:D2}",
					(int)(totalTimeForPeriod.TotalSeconds / 3600),
					totalTimeForPeriod.Minutes);
			}));
		}

		private void SetTotalForCurrentWeek(TimeSpan totalTimeForPeriod)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
			{
				TotalThisWeek = string.Format("{0:D2}:{1:D2}",
					(int)(totalTimeForPeriod.TotalSeconds / 3600),
					totalTimeForPeriod.Minutes);
			}));
		}

		private void SetTotalForCurrentMonth(TimeSpan totalTimeForPeriod)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
			{
				TotalThisMonth = string.Format("{0:D2}:{1:D2}",
					(int)(totalTimeForPeriod.TotalSeconds / 3600),
					totalTimeForPeriod.Minutes);
			}));
		}

		private TimeSpan ReadTotalForToday()
		{
			var repository = ServiceLocator.Current.GetInstance<ReadModelRepository>();
			return repository.GetStatisticsForDay(TimeTracker.TimeTrackingViewModel.GetCurrentKey().Key)
				.Maybe(s => TimeSpan.FromSeconds(s.Seconds), TimeSpan.Zero);
		}

		private TimeSpan ReadTotalForCurrentMonth()
		{
			var currentDate = DateTime.Now.Date;
			var firstDay = 1;
			var lastday = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

			var dates = Enumerable.Range(firstDay, lastday)
				.Select(day => TimeTracker.TimeTrackingViewModel.ToTimeTrackingKey(
					new DateTime(currentDate.Year, currentDate.Month, day)))
				.Select(key => key.Key)
				.ToList()
				.AsReadOnly();

			var repository = ServiceLocator.Current.GetInstance<ReadModelRepository>();
			return repository.GetTotalForPeriods(dates);
		}

		private TimeSpan ReadTotalForCurrentWeek()
		{
			var currentDate = DateTime.Now.Date;
			var firstDayOfWeek = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.FirstDayOfWeek;

			while (currentDate.DayOfWeek != firstDayOfWeek)
			{
				currentDate = currentDate.Subtract(TimeSpan.FromDays(1));
			}

			var firstDayOfCurrentWeek = currentDate;

			const int firstDay = 0;
			const int daysCount = 7;

			var dates = Enumerable.Range(firstDay, daysCount)
				.Select(day => TimeTracker.TimeTrackingViewModel.ToTimeTrackingKey(
					firstDayOfCurrentWeek.AddDays(day)))
				.Select(key => key.Key)
				.ToList()
				.AsReadOnly();

			var repository = ServiceLocator.Current.GetInstance<ReadModelRepository>();
			return repository.GetTotalForPeriods(dates);
		}

		private async void RefreshTotals()
		{
			try
			{
				await semaphoreSlim.WaitAsync();
				var totalForToday = Task.Run(() => ReadTotalForToday());
				var totalForWeek = Task.Run(() => ReadTotalForCurrentWeek());
				var totalForMonth = Task.Run(() => ReadTotalForCurrentMonth());

				await Task.WhenAll(new Task[] {totalForToday, totalForWeek, totalForMonth});

				SetTotalTime(totalForToday.Result);
				SetTotalForCurrentWeek(totalForWeek.Result);
				SetTotalForCurrentMonth(totalForMonth.Result);

				currentAttempt = 0;
			}
			catch (Exception ex)
			{
				if (ex.IsFatal() || currentAttempt > maxAttemptsToLoadStatistics)
				{
					throw;
				}

				currentAttempt++;
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		private void RefreshStatistics()
		{
			TimePublisher.Refresh(OnError);
		}

		private void OnError(Exception error)
		{
			TimeTracker.TimeTrackingViewModel.OnTimeRegistrationError(error,
				localizationService,
				messageBox);
		}
	}
}