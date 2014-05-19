using System;
using System.Reactive.Linq;
using TimeTracking.Commands;
using TimeTracking.Extensions;

namespace TimeTracking.Core
{
	public class TimeTrackingCore : ITimeTrackingCore
	{
		private DateTimeOffset previousValue;

		private Action trackingStarted;
		private Action trackingStopped;

		private TimeTrackingKey previousKey;

		private TimeSpan totalTime;
		private TimeSpan timeSinceLastSave;

		private Func<TimeTrackingKey> keyProvider;
		private Action<TimeSpan> timeChanged;
		private Action<RegisterTimeCommand> timeSave;

		private IDisposable subscription;
		private string currentMemo;

		private bool isStarted;

		public TimeTrackingCore(TimeSpan baseTime,
			Func<TimeTrackingKey> keyProvider,
			Action<TimeSpan> timeChanged,
			Action<RegisterTimeCommand> timeSave,
			Action trackingStarted,
			Action trackingStopped)
		{
			totalTime = baseTime;
			this.timeChanged = timeChanged;
			this.timeSave = timeSave;
			this.keyProvider = keyProvider;
			this.trackingStarted = trackingStarted;
			this.trackingStopped = trackingStopped;
		}

		public void Stop()
		{
			subscription.MaybeDo(s => s.Dispose());
			subscription = null;

			if (isStarted)
			{
				var key = GetCurrentKey();
				if (key != null && !string.IsNullOrEmpty(key.Key))
				{
					OnTimeSave(new RegisterTimeCommand(key.Key,
						key.Date,
						timeSinceLastSave,
						currentMemo));
				}

				OnTimeTrackingStopped();
				isStarted = false;
			}
		}

		public void Start()
		{
			previousValue = new DateTimeOffset(DateTime.Now);

			subscription = Observable.Interval(TimeSpan.FromSeconds(1))
				.Timestamp()
				.Select(t => new TrackingData(GetCurrentKey(), t.Timestamp))
				.Subscribe(IncreaseWorkingTime);

			isStarted = true;
			OnTimeTrackingStarted();
		}

		public void ChangeMemo(string memo)
		{
			Stop();
			currentMemo = memo;
			Start();
		}

		public void Dispose()
		{
			subscription.Dispose();

			keyProvider = null;
			timeChanged = null;
			timeSave = null;
			trackingStarted = null;
			trackingStopped = null;
		}

		private void IncreaseWorkingTime(TrackingData data)
		{
			var currentKey = data.Key;

			if (string.IsNullOrEmpty(currentKey.Key))
			{
				return;
			}

			if (previousKey != null
				&& !string.IsNullOrEmpty(previousKey.Key)
				&& !string.Equals(previousKey.Key, currentKey.Key, StringComparison.OrdinalIgnoreCase))
			{
				OnTimeSave(new RegisterTimeCommand(previousKey.Key,
					previousKey.Date,
					timeSinceLastSave,
					currentMemo));

				timeSinceLastSave = TimeSpan.FromSeconds(0);
				totalTime = TimeSpan.FromSeconds(0);
			}
			else
			{
				var workingTime = data.Timestamp - previousValue;
				previousValue = data.Timestamp;
				timeSinceLastSave = timeSinceLastSave.Add(workingTime);
				totalTime = totalTime.Add(workingTime);

				const double secondsToSave = 20;
				if (timeSinceLastSave.TotalSeconds > secondsToSave)
				{
					OnTimeSave(new RegisterTimeCommand(currentKey.Key,
						currentKey.Date,
						timeSinceLastSave,
						currentMemo));

					timeSinceLastSave = TimeSpan.FromSeconds(0);
				}
			}

			previousKey = currentKey;

			OnTotalTimeChange(totalTime);
		}

		private void OnTimeSave(RegisterTimeCommand command)
		{
			timeSave.MaybeDo(ts => ts(command));
		}

		private TimeTrackingKey GetCurrentKey()
		{
			return keyProvider.Maybe(x => x());
		}

		private void OnTotalTimeChange(TimeSpan currentTotalTime)
		{
			timeChanged.MaybeDo(t => t(currentTotalTime));
		}

		private void OnTimeTrackingStarted()
		{
			trackingStarted.MaybeDo(ts => ts());
		}

		private void OnTimeTrackingStopped()
		{
			trackingStopped.MaybeDo(ts => ts());
		}
	}
}