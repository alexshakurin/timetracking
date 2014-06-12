using TimeTracking.Commands;

namespace TimeTracker.Messages
{
	public class ManualTimeRegistered
	{
		public RegisterTimeCommand Command { get; private set; }

		public ManualTimeRegistered(RegisterTimeCommand command)
		{
			Command = command;
		}
	}
}