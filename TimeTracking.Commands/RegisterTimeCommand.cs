using System;

namespace TimeTracking.Commands
{
	public class RegisterTimeCommand : IDomainCommand
	{
		private readonly string reason;

		public string WorkingTimeId { get; private set; }

		public Guid CommandId { get; private set; }

		public DateTime Date { get; private set; }

		public DateTimeOffset Start { get; private set; }
		public DateTimeOffset End { get; private set; }

		public string Memo { get; private set; }

		public RegisterTimeCommand(string workingTimeId,
			DateTime date,
			DateTimeOffset start,
			DateTimeOffset end,
			string memo,
			string reason = null)
		{
			WorkingTimeId = workingTimeId;
			CommandId = Guid.NewGuid();
			Date = date;
			Start = start;
			End = end;
			Memo = memo;
			this.reason = reason;
		}

		public override string ToString()
		{
			return string.Format("Id '{0}': {1} for '{2}' with memo '{3}'{4}",
				WorkingTimeId,
				string.Format("'{0} - {1}'", Start.TimeOfDay, End.TimeOfDay),
				Date.ToShortDateString(),
				Memo,
				string.IsNullOrEmpty(reason)
					? null
					: string.Format(" as a result of '{0}'", reason));
		}
	}
}