namespace TimeTracker
{
	public interface ITimeTrackingViewModel
	{
		string Memo { get; set; }

		bool IsStarted { get; }

		void Stop();

		void Start();

		void StartOrStop();
	}
}