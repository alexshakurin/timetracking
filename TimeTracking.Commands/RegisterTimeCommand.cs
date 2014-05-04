using System;

namespace TimeTracking.Commands
{
	public class RegisterTimeCommand : IDomainCommand
	{
		public Guid WorkingTimeId { get; private set; }

		public Guid CommandId { get; private set; }

		public DateTime Date { get; private set; }

		public int Minutes { get; private set; }

		public string Memo { get; private set; }
		
		public RegisterTimeCommand(Guid workingTimeId, DateTime date, int minutes, string memo)
		{
			WorkingTimeId = workingTimeId;
			CommandId = Guid.NewGuid();
			Date = date;
			Minutes = minutes;
			Memo = memo;
		}
	}
}