using System;

namespace TimeTracking.Commands
{
	public class RegisterTimeCommand : IDomainCommand
	{
		private readonly string reason;

		public string WorkingTimeId { get; private set; }

		public Guid CommandId { get; private set; }

		public DateTime Date { get; private set; }

		public TimeSpan Time { get; private set; }

		public string Memo { get; private set; }

		public RegisterTimeCommand(string workingTimeId,
			DateTime date,
			TimeSpan time,
			string memo,
			string reason = null)
		{
			WorkingTimeId = workingTimeId;
			CommandId = Guid.NewGuid();
			Date = date;
			Time = time;
			Memo = memo;
			this.reason = reason;
		}

		public override string ToString()
		{
			return string.Format("Id '{0}': {1} for '{2}' with memo '{3}'{4}",
				WorkingTimeId,
				Time,
				Date.ToShortDateString(),
				Memo,
				string.IsNullOrEmpty(reason)
					? null
					: string.Format(" as a result of '{0}'", reason));
		}
	}
}