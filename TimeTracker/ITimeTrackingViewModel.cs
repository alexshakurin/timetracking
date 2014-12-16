using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace TimeTracker
{
	public interface ITimeTrackingViewModel : ICleanup
	{
		ICommand ChangeTask { get; }

		ICommand EnterManualTimeCommand { get; }

		string Memo { get; }

		string ProjectName { get; }

		bool IsStarted { get; }

		void Stop();

		void Start();

		void StartOrStop();
	}
}