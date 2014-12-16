using System;
using System.IO;
using TimeTracking.ApplicationServices.Settings;
using TimeTracking.Extensions;
using TimeTracking.Logging;

namespace TimeTracker.Settings
{
	public class SettingsService : ISettingsService
	{
		private static readonly object syncRoot = new object();

		public string GetLatestMemo()
		{
			lock (syncRoot)
			{
				return Properties.Settings.Default["LatestTask"] as string;
			}
		}

		public void SetLatestMemo(string memo)
		{
			lock (syncRoot)
			{
				Properties.Settings.Default["LatestTask"] = memo;
				Properties.Settings.Default.Save();
			}
		}

		public void DeleteSettingsFile(string file)
		{
			lock (syncRoot)
			{
				const int maxAttempts = 3;
				var success = false;
				var currentAttempt = 0;
				while (currentAttempt < maxAttempts && !success)
				{
					try
					{
						if (!string.IsNullOrEmpty(file))
						{
							File.Delete(file);
							Properties.Settings.Default.Reload();
							Properties.Settings.Default.Save();
							success = true;
						}
					}
					catch (Exception ex)
					{
						if (!ex.IsFatal())
						{
							LogHelper.Error(string.Format("Unable to delete settings file {0}. Reason: {1}",
								file,
								ex));
						}
						else
						{
							throw;
						}
					}
					finally
					{
						currentAttempt++;
					}
				}
			}
		}
	}
}