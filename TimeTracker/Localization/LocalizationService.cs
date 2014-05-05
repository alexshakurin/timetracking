namespace TimeTracker.Localization
{
	public class LocalizationService : ILocalizationService
	{
		public string GetLocalizedString(string key)
		{
			return Properties.Resources.ResourceManager.GetString(key);
		}
	}
}