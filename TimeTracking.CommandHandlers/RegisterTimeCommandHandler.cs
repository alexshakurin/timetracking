using System;
using TimeTracking.CommandHandlers.Exceptions;
using TimeTracking.Commands;
using TimeTracking.Infrastructure;
using TimeTracking.Infrastructure.CommandHandlers;
using TimeTracking.Logging;
using TimeTracking.Model;

namespace TimeTracking.CommandHandlers
{
	public class RegisterTimeCommandHandler : ICommandHandler<RegisterTimeCommand>
	{
		private readonly IEventSourcedRepository<WorkingTime> eventSourcedRepository;

		public RegisterTimeCommandHandler(IEventSourcedRepository<WorkingTime> eventSourcedRepository)
		{
			this.eventSourcedRepository = eventSourcedRepository;
		}

		public void Handle(RegisterTimeCommand command)
		{
			try
			{
				var workingTime = eventSourcedRepository.Find(command.WorkingTimeId);

				if (workingTime == null)
				{
					workingTime = new WorkingTime(command.WorkingTimeId);
				}

				workingTime.RegisterTime(command.Date, command.Start, command.End, command.Memo);

				eventSourcedRepository.Save(workingTime, command.CommandId.ToString());
			}
			catch (Exception ex)
			{
				throw new RegisterTimeException(string.Format("Register time '{0}' failed", command), ex);
			}

			LogHelper.Debug(command.ToString());
		}
	}
}