using System;

namespace TimeTracking.Commands
{
	public class RegisterTimeCommand : IDomainCommand
	{
		public string WorkingTimeId { get; private set; }

		public Guid CommandId { get; private set; }

		public DateTime Date { get; private set; }

		public TimeSpan Time { get; private set; }

		public string Memo { get; private set; }
		
		public RegisterTimeCommand(string workingTimeId, DateTime date, TimeSpan time, string memo)
		{
			WorkingTimeId = workingTimeId;
			CommandId = Guid.NewGuid();
			Date = date;
			Time = time;
			Memo = memo;
		}

		public override string ToString()
		{
			return string.Format("Id '{0}': {1} for '{2}' with memo '{3}'",
				WorkingTimeId,
				Time,
				Date.ToShortDateString(),
				Memo);
		}
	}
}