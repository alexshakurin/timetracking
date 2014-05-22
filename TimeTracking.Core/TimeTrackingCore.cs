using System;
using System.Reactive.Linq;
using TimeTracking.Commands;
using TimeTracking.Extensions;

namespace TimeTracking.Core
{
	public class TimeTrackingCore : ITimeTrackingCore
	{
		private readonly object syncRoot = new object();

		private TimeSpan totalTime;
		private TimeSpan timeSinceLastSave;

		private Action trackingStarted;
		private Action trackingStopped;

		private TimeTrackingKey previousKey;

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

			lock (syncRoot)
			{
				if (isStarted)
				{
					var key = GetCurrentKey();
					if (key != null && !string.IsNullOrEmpty(key.Key))
					{
						SaveTimeAndResetNoLock(key, "Saving by stop request");
					}

					isStarted = false;
					OnTimeTrackingStopped();
				}
			}
		}

		public void Start()
		{
			lock (syncRoot)
			{
				if (isStarted)
				{
					return;
				}

				var previousValue = new DateTimeOffset(DateTime.Now).ToLocalTime();

				subscription = Observable.Interval(TimeSpan.FromSeconds(1))
					.TimeInterval()
					.Select(t => new TrackingData(GetCurrentKey(), previousValue, t.Interval))
					.Subscribe(IncreaseWorkingTime);

				isStarted = true;
				OnTimeTrackingStarted();
			}
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

			lock (syncRoot)
			{
				if (previousKey != null
				&& !string.IsNullOrEmpty(previousKey.Key)
				&& !string.Equals(previousKey.Key, currentKey.Key, StringComparison.OrdinalIgnoreCase))
				{
					SaveTimeAndResetNoLock(previousKey, "Saving due to day end");

					totalTime = TimeSpan.FromSeconds(0);
				}
				else
				{
					timeSinceLastSave = timeSinceLastSave.Add(data.Interval);
					totalTime = totalTime.Add(data.Interval);

#if DEBUG
					const double secondsToSave = 5;
#else
					const double secondsToSave = 20;
#endif
					if (timeSinceLastSave.TotalSeconds > secondsToSave)
					{
						SaveTimeAndResetNoLock(currentKey, "Saving due to time elapsed");
					}
				}

				previousKey = currentKey;

				OnTotalTimeChange(totalTime);
			}
		}

		private void SaveTimeAndResetNoLock(TimeTrackingKey key, string reason)
		{
			if (timeSinceLastSave.TotalMilliseconds > 0)
			{
				var command = new RegisterTimeCommand(key.Key,
					key.Date,
					timeSinceLastSave,
					currentMemo,
					reason);
				timeSave.MaybeDo(ts => ts(command));
				timeSinceLastSave = TimeSpan.Zero;
			}
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