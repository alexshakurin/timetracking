﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TimeTracking.Extensions;

namespace TimeTracking.Core
{
	internal class TimeTrackingBus : IDisposable
	{
		private readonly BlockingCollection<TimePeriodProcessingUnit> collection;
		private readonly Task processingTask;
		private TimeSpan totalTime;
		private Action<TimeSpan> timeChange;
		private Action<TimeTrackingKey, DateTimeOffset, DateTimeOffset, string> save;

		public TimeTrackingBus(TimeSpan totalTime,
			Action<TimeSpan> timeChange,
			Action<TimeTrackingKey, DateTimeOffset, DateTimeOffset, string> save)
		{
			this.totalTime = totalTime;
			collection = new BlockingCollection<TimePeriodProcessingUnit>();
			processingTask = Task.Run(new Action(ProcessCollection));
			this.timeChange = timeChange;
			this.save = save;
		}

		public void Dispose()
		{
			collection.CompleteAdding();
			processingTask.Wait();
			timeChange = null;
			save = null;
		}

		public void EnqueuePeriodData(TimeTrackingKey key, DateTimeOffset currentTime)
		{
			collection.Add(new TimePeriodProcessingUnit(UnitType.TimeElapsed,
				new TimePeriodData(key, currentTime)));
		}

		public void EnqueueStart(TimeTrackingKey key, DateTimeOffset startTime)
		{
			collection.Add(new TimePeriodProcessingUnit(UnitType.Start,
				new TimePeriodData(key, startTime)));
		}

		public void EnqueueStop(TimeTrackingKey key, DateTimeOffset endTime)
		{
			collection.Add(new TimePeriodProcessingUnit(UnitType.Stop,
				new TimePeriodData(key, endTime)));
		}

		private void ProcessCollection()
		{
			TrackingData currentData = null;
			foreach (var nextUnit in collection.GetConsumingEnumerable())
			{
				var localCurrendData = currentData;
				var localNextUnit = nextUnit;
				currentData = ProcessTimeData(localCurrendData, localNextUnit);

				totalTime = totalTime.Add(currentData.Maybe(cd => cd.Elapsed, TimeSpan.Zero));
				timeChange.MaybeDo(tc => tc(totalTime));
			}
		}

		private TrackingData ProcessTimeData(TrackingData currentData, TimePeriodProcessingUnit unit)
		{
			TrackingData newData;
			if (unit.UnitType == UnitType.Start)
			{
				newData = new TrackingData(unit.Data.Key,
					unit.Data.CurrentTime,
					unit.Data.CurrentTime,
					unit.Data.CurrentTime);
			}
			else if (unit.UnitType == UnitType.Stop)
			{
				newData = null;
				if (currentData.Key.Equals(unit.Data.Key))
				{
					SaveData(currentData.Tick(unit.Data.Key, unit.Data.CurrentTime),
						"Saving due to stop request");
				}
				else
				{
					SaveData(currentData, "Saving due to stop request");
					// TODO: Save new data ????
				}
			}
			else if (unit.UnitType == UnitType.TimeElapsed)
			{
				if (currentData.Key.Equals(unit.Data.Key))
				{
					var combined = currentData.Tick(unit.Data.Key, unit.Data.CurrentTime);

#if DEBUG
					const double secondsToSave = 5;
#else
					const double secondsToSave = 20;
#endif

					if (combined.ElapsedSinceStart.TotalSeconds > secondsToSave)
					{
						SaveData(combined, "Saving due to time elapsed");
						newData = new TrackingData(combined.Key,
							combined.CurrentDate,
							combined.CurrentDate,
							combined.CurrentDate);
					}
					else
					{
						newData = combined;
					}
				}
				else
				{
					SaveData(currentData, "Saving due to day change");
					newData = new TrackingData(unit.Data.Key,
						unit.Data.CurrentTime,
						unit.Data.CurrentTime,
						unit.Data.CurrentTime);
				}
			}
			else
			{
				throw new InvalidOperationException(string.Format("Unit type {0} is unknown",
					Enum.GetName(typeof(UnitType), unit.UnitType)));
			}

			return newData;
		}

		private void SaveData(TrackingData trackingData, string message)
		{
			if (trackingData.ElapsedSinceStart.TotalSeconds > 0)
			{
				save.MaybeDo(s => s(trackingData.Key,
					trackingData.Start,
					trackingData.CurrentDate,
					message));
			}
		}

		private class TimePeriodData
		{
			public TimeTrackingKey Key { get; private set; }

			public DateTimeOffset CurrentTime { get; set; }

			public TimePeriodData(TimeTrackingKey key, DateTimeOffset currentTime)
			{
				Key = key;
				CurrentTime = currentTime;
			}
		}

		private class TimePeriodProcessingUnit
		{
			public UnitType UnitType { get; private set; }

			public TimePeriodData Data { get; private set; }

			public TimePeriodProcessingUnit(UnitType unitType, TimePeriodData data)
			{
				UnitType = unitType;
				Data = data;
			}
		}

		private enum UnitType
		{
			Start,
			TimeElapsed,
			Stop
		}
	}
}