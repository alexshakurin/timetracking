using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using TimeTracking.Extensions;
using TimeTracking.Logging;

namespace TimeTracker
{
	public class MainViewModel : ViewModelBase
	{
		private ICommand startStopCommand;

		private bool isPaused;

		public ITimeTrackingViewModel TimeTrackingViewModel { get; private set; }

		public ICommand StartStopCommand
		{
			get
			{
				if (startStopCommand == null)
				{
					startStopCommand = new RelayCommand(StartOrStop);
				}

				return startStopCommand;
			}
		}
		
		public MainViewModel()
		{
			TimeTrackingViewModel = ServiceLocator.Current.GetInstance<ITimeTrackingViewModel>();

			SystemEvents.PowerModeChanged += PowerModeChanged;
			SystemEvents.SessionSwitch += SessionSwitch;
		}

		public override void Cleanup()
		{
			SystemEvents.PowerModeChanged -= PowerModeChanged;
			SystemEvents.SessionSwitch -= SessionSwitch;

			base.Cleanup();

			TimeTrackingViewModel.MaybeDo(ttvm => ttvm.Stop());
		}

		private void SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason.In(new[] {SessionSwitchReason.SessionLock, SessionSwitchReason.SessionLogoff})
				&& TimeTrackingViewModel.IsStarted)
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
			if (e.Mode == PowerModes.Suspend && TimeTrackingViewModel.IsStarted)
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
			TimeTrackingViewModel.Start();
			isPaused = false;
		}

		private void PauseTracking()
		{
			TimeTrackingViewModel.Stop();
			isPaused = true;
		}

		private void StartOrStop()
		{
			LogHelper.Debug(string.Format("Change tracking state by user request at {0}", DateTime.Now));

			TimeTrackingViewModel.StartOrStop();
		}
	}
}