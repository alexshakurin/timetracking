using TimeTracking.ApplicationServices.Settings;

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
	}
}