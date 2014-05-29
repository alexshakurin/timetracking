namespace TimeTracker.Localization
{
	public class LocalizationService : ILocalizationService
	{
		public static ILocalizationService Default
		{
			get
			{
				return new LocalizationService();
			}
		}

		public string GetLocalizedString(string key)
		{
			return Properties.Resources.ResourceManager.GetString(key);
		}
	}
}