using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using TimeTracker.Localization;
using TimeTracking.ApplicationServices.Dialogs;
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
		private readonly ICommandBus commandBus;

		private ICommand startStopCommand;

		private bool isPaused;

		private ITimeTrackingViewModel timeTrackingViewModel;

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
			commandBus = ServiceLocator.Current.GetInstance<ICommandBus>();

			SystemEvents.PowerModeChanged += PowerModeChanged;
			SystemEvents.SessionSwitch += SessionSwitch;

			StartLoading();
		}

		public override void Cleanup()
		{
			SystemEvents.PowerModeChanged -= PowerModeChanged;
			SystemEvents.SessionSwitch -= SessionSwitch;

			base.Cleanup();

			TimeTrackingViewModel.MaybeDo(ttvm => ttvm.Stop());
			TimeTrackingViewModel.MaybeDo(ttvm => ttvm.Cleanup());
		}

		private async void StartLoading()
		{
			try
			{
				var stats = await Task.Run(() =>
				{
					var currentKey = TimeTracker.TimeTrackingViewModel.GetCurrentKey();
					var repository = ServiceLocator.Current.GetInstance<ReadModelRepository>();

					var statistics = repository.GetStatisticsForDay(currentKey.Key);

					return new
						{
							Duration = statistics.Maybe(s => TimeSpan.FromSeconds(s.Seconds), TimeSpan.Zero),
							LatestMemo = statistics.Maybe(s => s.LatestMemo)
						};
				});

				var vm = new TimeTrackingViewModel(stats.Duration,
					stats.LatestMemo,
					commandBus,
					messageBox,
					localizationService);

				vm.PropertyChanged += OnViewModelPropertyChanged;
				TimeTrackingViewModel = vm;

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
	}
}