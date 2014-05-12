using System.Windows.Input;

namespace TimeTracker
{
	public interface ITimeTrackingViewModel
	{
		ICommand ChangeTask { get; }

		string Memo { get; }

		string ProjectName { get; }

		bool IsStarted { get; }

		void Stop();

		void Start();

		void StartOrStop();
	}
}