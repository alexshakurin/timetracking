
using TimeTracking.Commands;

namespace TimeTracking.Infrastructure
{
	public interface ICommandBus
	{
		void Send<TCommand>(TCommand command) where TCommand : IDomainCommand;
	}
}
