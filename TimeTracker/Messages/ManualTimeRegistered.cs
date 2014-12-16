using GalaSoft.MvvmLight.Messaging;
using TimeTracking.Commands;

namespace TimeTracker.Messages
{
	public class ManualTimeRegistered : MessageBase
	{
		public RegisterTimeCommand Command { get; private set; }

		public ManualTimeRegistered(RegisterTimeCommand command)
		{
			Command = command;
		}
	}
}