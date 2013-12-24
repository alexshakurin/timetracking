using System;
using System.IO;
using TimeTracking.Export;

namespace TimeTracker.FileSystemExport
{
	public class FileSystemTimeService : ITimeService
	{
		public void SaveTime(TimeSpan currentTime, DateTime date)
		{
			var fileName = date.ToString("yyyy_MM_dd") + ".txt";
			File.WriteAllText(fileName, currentTime.ToString());
		}

		public TimeSpan? LoadTime(DateTime date)
		{
			var fileName = date.ToString("yyyy_MM_dd") + ".txt";
			TimeSpan? result = null;

			if (File.Exists(fileName))
			{
				TimeSpan span;
				var text = File.ReadAllText(fileName);
				if (TimeSpan.TryParse(text, out span))
				{
					result = new TimeSpan(span.Hours, span.Minutes, span.Seconds);
				}
			}

			return result;
		}
	}
}