﻿using System;
using System.Reactive.Linq;
using TimeTracking.Commands;
using TimeTracking.Extensions;

namespace TimeTracking.Core
{
	public class TimeTrackingCore : ITimeTrackingCore
	{
		private readonly object syncRoot = new object();

		private readonly TimeTrackingBus trackingBus;

		private Action trackingStarted;
		private Action trackingStopped;


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
			this.timeChanged = timeChanged;
			this.timeSave = timeSave;
			this.keyProvider = keyProvider;
			this.trackingStarted = trackingStarted;
			this.trackingStopped = trackingStopped;

			trackingBus = new TimeTrackingBus(baseTime,
				OnTotalTimeChange,
				SaveTimeAndResetNoLock);
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
						trackingBus.EnqueueStop(GetCurrentKey(),
							new DateTimeOffset(DateTime.Now).ToLocalTime());
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

				var startTime = new DateTimeOffset(DateTime.Now).ToLocalTime();

				var key = GetCurrentKey();
				if (key != null && !string.IsNullOrEmpty(key.Key))
				{
					trackingBus.EnqueueStart(key, startTime);

					subscription = Observable.Interval(TimeSpan.FromSeconds(1))
						.Timestamp()
						.Select(t => new { Key = GetCurrentKey(), t.Timestamp})
						.Where(t => t.Key != null && !string.IsNullOrEmpty(t.Key.Key))
						.Subscribe(t => IncreaseWorkingTime(t.Key, t.Timestamp));

					isStarted = true;
					OnTimeTrackingStarted();
				}
				
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

		private void IncreaseWorkingTime(TimeTrackingKey key, DateTimeOffset currentTime)
		{
			if (key == null || string.IsNullOrEmpty(key.Key))
			{
				return;
			}

			trackingBus.EnqueuePeriodData(key, currentTime);
		}

		private void SaveTimeAndResetNoLock(TimeTrackingKey key,
			DateTimeOffset start,
			DateTimeOffset end,
			string reason)
		{
			var timeSinceLastSave = end - start;
			if (timeSinceLastSave.TotalMilliseconds > 0)
			{
				var command = new RegisterTimeCommand(key.Key,
					key.Date,
					start,
					end,
					currentMemo,
					reason);
				timeSave.MaybeDo(ts => ts(command));
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