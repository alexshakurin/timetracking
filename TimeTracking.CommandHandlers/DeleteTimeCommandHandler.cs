using System;
using TimeTracking.CommandHandlers.Exceptions;
using TimeTracking.Commands;
using TimeTracking.Infrastructure;
using TimeTracking.Infrastructure.CommandHandlers;
using TimeTracking.Logging;
using TimeTracking.Model;

namespace TimeTracking.CommandHandlers
{
	public class DeleteTimeCommandHandler : ICommandHandler<DeleteTimeCommand>
	{
		private readonly IEventSourcedRepository<WorkingTime> eventSourcedRepository;

		public DeleteTimeCommandHandler(IEventSourcedRepository<WorkingTime> eventSourcedRepository)
		{
			this.eventSourcedRepository = eventSourcedRepository;
		}

		public void Handle(DeleteTimeCommand command)
		{
			try
			{
				var workingTime = eventSourcedRepository.Find(command.WorkingTimeId);

				if (workingTime == null)
				{
					workingTime = new WorkingTime(command.WorkingTimeId);
				}

				workingTime.DeleteTime(command.Date, command.Start, command.End);

				eventSourcedRepository.Save(workingTime, command.CommandId.ToString());
			}
			catch (Exception ex)
			{
				throw new DeleteTimeException(string.Format("Delete time '{0}' failed", command), ex);
			}

			LogHelper.Debug(command.ToString());
		}
	}
}