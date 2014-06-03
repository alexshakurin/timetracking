using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using TimeTracker.Localization;
using TimeTracker.TimePublishing;
using TimeTracker.Views.ChangeTask;
using TimeTracking.ApplicationServices.Dialogs;
using TimeTracking.Commands;
using TimeTracking.Core;
using TimeTracking.Extensions;
using TimeTracking.Infrastructure;
using TimeTracking.Logging;
using TimeTracking.ReadModel;

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

		private bool isStarted;

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

		public TimeTrackingViewModel(string memo,
			ICommandBus commandBus,
			IMessageBoxService messageBox,
			ILocalizationService localizationService)
		{
			this.commandBus = commandBus;
			this.messageBox = messageBox;
			this.localizationService = localizationService;
			Memo = memo;
			core = new TimeTrackingCore(memo,
				GetCurrentKey,
				OnTimeSaved,
				OnTrackingStarted,
				OnTrackingStopped);
		}

		public static TimeTrackingKey ToTimeTrackingKey(DateTime dateTime)
		{
			var date = dateTime.ToString(format);

			return new TimeTrackingKey(date, dateTime);
		}

		public static TimeTrackingKey GetCurrentKey()
		{
			var date = DateTime.Now.Date;
			return new TimeTrackingKey(date.ToString(format), date);
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

		public override void Cleanup()
		{
			base.Cleanup();
			StopTrackingTime();
			core.Dispose();
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
				RaisePropertyChanged(() => IsStarted);
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