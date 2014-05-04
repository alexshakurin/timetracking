using TimeTracking.Commands;
using TimeTracking.Infrastructure;
using TimeTracking.Infrastructure.CommandHandlers;
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
			var workingTime = eventSourcedRepository.Find(command.WorkingTimeId);

			if (workingTime == null)
			{
				workingTime = new WorkingTime(command.WorkingTimeId);
			}

			workingTime.RegisterTime(command.Date, command.Minutes, command.Memo);

			eventSourcedRepository.Save(workingTime, command.CommandId.ToString());
		}
	}
}