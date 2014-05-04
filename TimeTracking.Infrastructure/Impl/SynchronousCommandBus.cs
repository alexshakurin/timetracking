using Microsoft.Practices.ServiceLocation;
using TimeTracking.Commands;
using TimeTracking.Infrastructure.CommandHandlers;

namespace TimeTracking.Infrastructure.Impl
{
	public class SynchronousCommandBus : ICommandBus
	{
		public void Send<TCommand>(TCommand command) where TCommand : IDomainCommand
		{
			var handler = ServiceLocator.Current.GetInstance<ICommandHandler<TCommand>>();

			handler.Handle(command);
			//var handlers = ServiceLocator.Current.GetAllInstances<ICommandHandler<TCommand>>();

			//foreach (var handler in handlers)
			//{
			//	handler.Handle(command);
			//}
		}
	}
}