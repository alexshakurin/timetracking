using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using TimeTracker.Localization;
using TimeTracker.TimePublishing;
using TimeTracker.Views.ChangeTask;
using TimeTracking.ApplicationServices.Dialogs;
using TimeTracking.Commands;
using TimeTracking.Extensions;
using TimeTracking.Infrastructure;
using TimeTracking.Logging;

namespace TimeTracker
{
	public class TimeTrackingViewModel : ViewModel, ITimeTrackingViewModel
	{
		private readonly ICommandBus commandBus;
		private readonly IMessageBoxService messageBox;
		private readonly ILocalizationService localizationService;
		private string previousKey;
		private const string format = "yyyy-MM-dd";

		private ICommand changeTask;

		private IDisposable subscription;
		private IDisposable confirmationSubscription;

		private DateTimeOffset previousValue;
		private TimeSpan timeSinceLastUpdate;
		private TimeSpan totalTimeForPeriod;

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
		}

		//private void LoadTime()
		//{
		//	var loadedTime = timeService.LoadTime(DateTime.Now);
		//	if (loadedTime.HasValue)
		//	{
		//		time = loadedTime.Value;
		//	}
		//	else
		//	{
		//		time = new TimeSpan();
		//	}

		//	TotalTime = time.ToString();
		//}

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

			subscription.MaybeDo(s => s.Dispose());
			subscription = null;
			confirmationSubscription.MaybeDo(cs => cs.Dispose());
			confirmationSubscription = null;
			IsStarted = false;
		}

		private void StartTrackingTime()
		{
			LogHelper.Debug(string.Format("Time tracking started at {0}", DateTime.Now));

			previousValue = new DateTimeOffset(DateTime.Now);
			subscription = Observable.Interval(TimeSpan.FromSeconds(1))
				.Timestamp()
				.ObserveOnDispatcher()
				.Subscribe(IncreaseWorkingTime);

			confirmationSubscription = Observable.Interval(TimeSpan.FromSeconds(5))
				.Timestamp()
				.ObserveOnDispatcher()
				.Subscribe(DisplayWorkingTime);

			IsStarted = true;
		}

		private void RestartTrackingTime()
		{
			if (IsStarted)
			{
				StopTrackingTime();
			}

			StartTrackingTime();
		}
		
		private void IncreaseWorkingTime(Timestamped<long> msg)
		{
			var currentKey = DateTime.Now.Date.ToString(format);

			if (!string.IsNullOrEmpty(previousKey)
				&& !string.Equals(previousKey, currentKey, StringComparison.OrdinalIgnoreCase))
			{
				TimePublisher.PublishTimeRegistration(commandBus,
					previousKey,
					DateTime.Now.Date,
					timeSinceLastUpdate,
					Memo,
					OnTimeRegistrationErrorCallback);

				timeSinceLastUpdate = TimeSpan.FromSeconds(0);
				totalTimeForPeriod = TimeSpan.FromSeconds(0);
			}
			else
			{
				var workingTime = msg.Timestamp - previousValue;
				previousValue = msg.Timestamp;
				timeSinceLastUpdate = timeSinceLastUpdate.Add(workingTime);
				totalTimeForPeriod = totalTimeForPeriod.Add(workingTime);

				const double secondsToSave = 20;
				if (timeSinceLastUpdate.TotalSeconds > secondsToSave)
				{
					TimePublisher.PublishTimeRegistration(commandBus,
						currentKey,
						DateTime.Now.Date,
						timeSinceLastUpdate,
						Memo,
						OnTimeRegistrationErrorCallback);

					timeSinceLastUpdate = TimeSpan.FromSeconds(0);
				}
			}

			previousKey = currentKey;

			TotalTime = string.Format("{0:D2}:{1:D2}:{2:D2}", totalTimeForPeriod.Hours, totalTimeForPeriod.Minutes, totalTimeForPeriod.Seconds);
		}

		private void DisplayWorkingTime(Timestamped<long> msg)
		{
			//try
			//{
			//	var serverTime = timeService.LoadTime(DateTime.Now);

			//	if (!serverTime.HasValue)
			//	{
			//		serverTime = new TimeSpan();
			//	}

			//	ConfirmedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", serverTime.Value.Hours, serverTime.Value.Minutes, serverTime.Value.Seconds);
			//	ConfirmedTimeForeground = Brushes.Green;
			//}
			//catch (Exception)
			//{
			//	ConfirmedTimeForeground = Brushes.Red;
			//}
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
				ProjectName = changeTaskView.ViewModel.ProjectName;
				RestartTrackingTime();
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