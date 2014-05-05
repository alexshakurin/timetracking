using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media;
using TimeTracking.Commands;
using TimeTracking.Extensions;
using TimeTracking.Infrastructure;
using TimeTracking.Logging;

namespace TimeTracker
{
	public class TimeTrackingViewModel : ViewModel, ITimeTrackingViewModel
	{
		private readonly ICommandBus commandBus;
		private readonly Guid workingTimeId;

		private IDisposable subscription;
		private IDisposable confirmationSubscription;

		private DateTimeOffset previousValue;
		private TimeSpan time;

		private string totalTime;
		private bool isStarted;

		private string confirmedTime;

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

		public TimeTrackingViewModel(ICommandBus commandBus)
		{
			this.commandBus = commandBus;
			workingTimeId = Guid.NewGuid();
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
		
		private void IncreaseWorkingTime(Timestamped<long> msg)
		{
			var workingTime = msg.Timestamp - previousValue;
			previousValue = msg.Timestamp;
			time = time.Add(workingTime);

			var totalMinutes = (int) workingTime.TotalMinutes;

			if (totalMinutes > 0)
			{
				commandBus.Send(new RegisterTimeCommand(workingTimeId,
					DateTime.Now.Date,
					totalMinutes,
					"Test"));
			}

			//timeService.SaveTime(time, DateTime.Now);

			TotalTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
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
	}
}