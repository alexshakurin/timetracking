using TimeTracking.Commands;

namespace TimeTracking.Infrastructure.CommandHandlers
{
	public interface ICommandHandler<TCommand> where TCommand : IDomainCommand
	{
		void Handle(TCommand command);
	}
}