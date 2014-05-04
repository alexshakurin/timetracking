using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using TimeTracking.Logging;

namespace TimeTracker
{
	public class MainViewModel : ViewModelBase
	{
		private ICommand startStopCommand;

		public TimeTrackingViewModel TimeTrackingViewModel { get; private set; }

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
			TimeTrackingViewModel = ServiceLocator.Current.GetInstance<TimeTrackingViewModel>();

			SystemEvents.PowerModeChanged += PowerModeChanged;
			SystemEvents.SessionSwitch += SessionSwitch;
		}

		public override void Cleanup()
		{
			SystemEvents.PowerModeChanged -= PowerModeChanged;
			SystemEvents.SessionSwitch -= SessionSwitch;

			base.Cleanup();
		}

		private void SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason == SessionSwitchReason.SessionLock || e.Reason == SessionSwitchReason.SessionLogoff)
			{
				LogHelper.Debug(string.Format("Stopping time tracking at {0} by user logoff/lock", DateTime.Now));

				TimeTrackingViewModel.Stop();
			}
			else if(e.Reason == SessionSwitchReason.SessionUnlock)
			{
				LogHelper.Debug(string.Format("Resuming time tracking at {0} by session unlock", DateTime.Now));

				TimeTrackingViewModel.Start();
			}
		}

		private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			if (e.Mode == PowerModes.Suspend)
			{
				LogHelper.Debug(string.Format("Stopping time tracking at {0} by system suspend", DateTime.Now));

				TimeTrackingViewModel.Stop();
			}
			else if(e.Mode == PowerModes.Resume)
			{
				LogHelper.Debug(string.Format("Resuming time tracking at {0} by system resume", DateTime.Now));

				TimeTrackingViewModel.Start();
			}
		}

		private void StartOrStop()
		{
			LogHelper.Debug(string.Format("Change tracking state by user request at {0}", DateTime.Now));

			TimeTrackingViewModel.StartOrStop();
		}
	}
}