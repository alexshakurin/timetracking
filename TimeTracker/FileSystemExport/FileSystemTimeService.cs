using System;
using System.IO;
using TimeTracking.Export;

namespace TimeTracker.FileSystemExport
{
	public class FileSystemTimeService : ITimeService
	{
		private static readonly object syncRoot = new object();

		public void SaveTime(TimeSpan currentTime, DateTime date)
		{
			lock (syncRoot)
			{
				var fileName = date.ToString("yyyyMMdd") + ".txt";
				File.WriteAllText(fileName, currentTime.ToString());
			}
		}

		public TimeSpan? LoadTime(DateTime date)
		{
			lock (syncRoot)
			{
				var fileName = date.ToString("yyyyMMdd") + ".txt";
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
}