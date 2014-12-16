using System;

namespace TimeTracking.Commands
{
	public class DeleteTimeCommand : IDomainCommand
	{
		public string WorkingTimeId { get; private set; }

		public Guid CommandId { get; private set; }

		public DateTime Date { get; private set; }

		public DateTimeOffset Start { get; private set; }
		public DateTimeOffset End { get; private set; }

		public DeleteTimeCommand(string workingTimeId,
			DateTime date,
			DateTimeOffset start,
			DateTimeOffset end)
		{
			WorkingTimeId = workingTimeId;
			CommandId = Guid.NewGuid();
			Date = date;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			return string.Format("Removing from '{0}' to '{1}' for day '{2}'",
				Start.TimeOfDay,
				End.TimeOfDay,
				Date.ToShortDateString());
		}
	}
}