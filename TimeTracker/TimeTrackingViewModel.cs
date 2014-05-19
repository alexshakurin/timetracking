using System;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro;
using Microsoft.Practices.ServiceLocation;
using TimeTracker.Localization;
using TimeTracker.TimePublishing;
using TimeTracker.Views.ChangeTask;
using TimeTracking.ApplicationServices.Dialogs;
using TimeTracking.Commands;
using TimeTracking.Core;
using TimeTracking.Infrastructure;
using TimeTracking.Logging;

namespace TimeTracker
{
	public class TimeTrackingViewModel : ViewModel, ITimeTrackingViewModel
	{
		private readonly ICommandBus commandBus;
		private readonly IMessageBoxService messageBox;
		private readonly ILocalizationService localizationService;
		private readonly ITimeTrackingCore core;
		private const string format = "yyyy-MM-dd";

		private ICommand changeTask;

		private string totalTime;
		private bool isStarted;

		private string confirmedTime;

		private string memo;

		public ICommand ChangeTask
		{
			get
			{
				if (changeTask == null)
				{
					changeTask = new RelayCommand(ExecuteChangeTask, CanExecuteChangeTask);
				}

				return changeTask;
			}
		}

		private string projectName;

		public string ProjectName
		{
			get { return projectName; }
			private set
			{
				if (projectName == value)
				{
					return;
				}

				projectName = value;
				RaisePropertyChanged(() => ProjectName);
			}
		}

		public string Memo
		{
			get { return memo; }
			set
			{
				if (memo == value)
				{
					return;
				}

				memo = value;
				RaisePropertyChanged(() => Memo);
			}
		}

		private Brush confirmedTimeForeground;

		public Brush ConfirmedTimeForeground
		{
			get { return confirmedTimeForeground; }
			set
			{
				if (confirmedTimeForeground == value)
				{
					return;
				}

				confirmedTimeForeground = value;
				RaisePropertyChanged(() => ConfirmedTimeForeground);
			}
		}

		public string ConfirmedTime
		{
			get { return confirmedTime; }
			set
			{
				if (confirmedTime == value)
				{
					return;
				}

				confirmedTime = value;
				RaisePropertyChanged(() => ConfirmedTime);
			}
		}

		public string TotalTime
		{
			get { return totalTime; }
			set
			{
				if (totalTime == value)
				{
					return;
				}

				totalTime = value;
				RaisePropertyChanged(() => TotalTime);
			}
		}

		public bool IsStarted
		{
			get { return isStarted; }
			private set
			{
				if (isStarted == value)
				{
					return;
				}

				isStarted = value;
				RaisePropertyChanged(() => IsStarted);
			}
		}

		public TimeTrackingViewModel(ICommandBus commandBus,
			IMessageBoxService messageBox,
			ILocalizationService localizationService)
		{
			this.commandBus = commandBus;
			this.messageBox = messageBox;
			this.localizationService = localizationService;
			core = new TimeTrackingCore(TimeSpan.Zero,
				() =>
				{
					var date = DateTime.Now.Date;
					return new TimeTrackingKey(date.ToString(format), date);
				},
				OnTimeChanged,
				OnTimeSaved,
				OnTrackingStarted,
				OnTrackingStopped);
		}

		private void OnTrackingStopped()
		{
			IsStarted = false;
		}

		private void OnTrackingStarted()
		{
			IsStarted = true;
		}

		private void OnTimeSaved(RegisterTimeCommand command)
		{
			TimePublisher.PublishTimeRegistration(commandBus,
				command,
				OnTimeRegistrationErrorCallback);
		}

		private void OnTimeChanged(TimeSpan totalTimeForPeriod)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
				TotalTime = string.Format("{0:D2}:{1:D2}:{2:D2}", totalTimeForPeriod.Hours, totalTimeForPeriod.Minutes, totalTimeForPeriod.Seconds)));
		}

		public override void Cleanup()
		{
			base.Cleanup();
			StopTrackingTime();
		}

		public void Stop()
		{
			if (IsStarted)
			{
				StopTrackingTime();
			}
		}

		public void Start()
		{
			StartTrackingTime();
		}

		public void StartOrStop()
		{
			if (IsStarted)
			{
				StopTrackingTime();
			}
			else
			{
				StartTrackingTime();
			}
		}

		private void StopTrackingTime()
		{
			LogHelper.Debug(string.Format("Time tracking stopped at {0}", DateTime.Now));

			core.Stop();
			IsStarted = false;
		}

		private void StartTrackingTime()
		{
			LogHelper.Debug(string.Format("Time tracking started at {0}", DateTime.Now));

			core.Start();

			IsStarted = true;
		}

		private bool CanExecuteChangeTask()
		{
			return true;
		}

		private void ExecuteChangeTask()
		{
			var changeTaskView = ServiceLocator.Current.GetInstance<IChangeTaskView>();

			changeTaskView.ViewModel.SetDefaultValues(Memo, ProjectName);

			var result = changeTaskView.ShowDialog();

			if (result.HasValue && result.Value)
			{
				Memo = changeTaskView.ViewModel.Memo;
				core.ChangeMemo(Memo);
				ProjectName = changeTaskView.ViewModel.ProjectName;
			}
		}

		private void OnTimeRegistrationErrorCallback(Exception error)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => OnTimeRegistrationError(error)));
		}

		private void OnTimeRegistrationError(Exception error)
		{
			LogHelper.Error(error.ToString());
			var caption = localizationService.GetLocalizedString("TimeRegistrationError_Caption");
			var message = localizationService.GetLocalizedString("TimeRegistrationError_Message");

			messageBox.ShowOkError(message, caption);
		}
	}
}