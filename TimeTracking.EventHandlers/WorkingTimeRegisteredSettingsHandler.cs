using TimeTracking.ApplicationServices.Settings;
using TimeTracking.Model.Events;

namespace TimeTracking.EventHandlers
{
	public class WorkingTimeRegisteredSettingsHandler : IEventHandler<WorkingTimeRegistered>
	{
		private readonly ISettingsService settingsService;

		public WorkingTimeRegisteredSettingsHandler(ISettingsService settingsService)
		{
			this.settingsService = settingsService;
		}

		public void Handle(WorkingTimeRegistered @event)
		{
			settingsService.SetLatestMemo(@event.Memo);
		}
	}
}