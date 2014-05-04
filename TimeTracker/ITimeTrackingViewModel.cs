namespace TimeTracker
{
	public interface ITimeTrackingViewModel
	{
		bool IsStarted { get; }

		void Stop();

		void Start();

		void StartOrStop();
	}
}