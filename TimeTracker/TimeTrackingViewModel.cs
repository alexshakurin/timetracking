using System;
using System.Reactive;
using System.Reactive.Linq;
using TimeTracking.Export;

namespace TimeTracker
{
	public class TimeTrackingViewModel : ViewModel
	{
		private readonly ITimeService timeService;

		private IDisposable subscription;

		private DateTimeOffset previousValue;
		private TimeSpan time;

		private string totalTime;

		private bool isStarted;

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
				RaisePropertyChanged("TotalTime");
			}
		}

		public TimeTrackingViewModel(ITimeService timeService)
		{
			this.timeService = timeService;
			LoadTime();
		}

		private void LoadTime()
		{
			var loadedTime = timeService.LoadTime(DateTime.Now);
			if (loadedTime.HasValue)
			{
				time = loadedTime.Value;
			}
			else
			{
				time = new TimeSpan();
			}

			TotalTime = time.ToString();
		}

		public override void Cleanup()
		{
			base.Cleanup();
			StopTrackingTime();
		}
		
		public void StartOrStop()
		{
			if (isStarted)
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
			subscription.Dispose();
			subscription = null;
			isStarted = false;
		}

		private void StartTrackingTime()
		{
			previousValue = new DateTimeOffset(DateTime.Now);
			subscription = Observable.Interval(TimeSpan.FromSeconds(1))
				.Timestamp()
				.ObserveOnDispatcher()
				.Subscribe(IncreaseWorkingTime);
			isStarted = true;
		}
		
		private void IncreaseWorkingTime(Timestamped<long> msg)
		{
			var workingTime = msg.Timestamp - previousValue;
			previousValue = msg.Timestamp;
			time = time.Add(workingTime);

			timeService.SaveTime(time, DateTime.Now);

			TotalTime = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
		}
	}
}