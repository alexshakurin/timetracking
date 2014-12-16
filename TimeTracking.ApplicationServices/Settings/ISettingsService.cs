namespace TimeTracking.ApplicationServices.Settings
{
	public interface ISettingsService
	{
		string GetLatestMemo();

		void SetLatestMemo(string memo);

		void DeleteSettingsFile(string settingsFile);
	}
}