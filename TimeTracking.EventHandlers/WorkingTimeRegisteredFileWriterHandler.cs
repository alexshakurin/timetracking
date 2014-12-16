using System;
using System.IO;
using TimeTracking.Extensions;
using TimeTracking.Logging;
using TimeTracking.Model.Events;

namespace TimeTracking.EventHandlers
{
	public class WorkingTimeRegisteredFileWriterHandler : IEventHandler<WorkingTimeRegistered>
	{
		private static readonly object fileSyncRoot = new object();

		public void Handle(WorkingTimeRegistered @event)
		{
			lock (fileSyncRoot)
			{
				try
				{
					const string backupDirName = "backup";
					if (!Directory.Exists(backupDirName))
					{
						Directory.CreateDirectory(backupDirName);
					}

					var fileName = string.Format("backup\\{0}.txt", @event.Date.ToString("yyyy_MM_dd"));

					using (var fileStream = new FileStream(fileName, FileMode.Append))
					using (var streamWriter = new StreamWriter(fileStream))
					{
						streamWriter.WriteLine("{0} - {1}", @event.Start.TimeOfDay, @event.End.TimeOfDay);
					}
				}
				catch (Exception ex)
				{
					LogHelper.Error(string.Format("Error writing event {0} to file. Details: {1}",
						@event,
						ex));

					if (ex.IsFatal())
					{
						throw;
					}
				}
			}
		}
	}
}