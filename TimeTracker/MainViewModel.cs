using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

namespace TimeTracker
{
	public class MainViewModel : ViewModelBase
	{
		private ICommand startStopCommand;

		public TimeTrackingViewModel TimeTrackingViewModel { get; private set; }

		public ICommand StartStopCommand
		{
			get
			{
				if (startStopCommand == null)
				{
					startStopCommand = new RelayCommand(StartOrStop);
				}

				return startStopCommand;
			}
		}
		
		public MainViewModel()
		{
			TimeTrackingViewModel = ServiceLocator.Current.GetInstance<TimeTrackingViewModel>();
		}

		private void StartOrStop()
		{
			TimeTrackingViewModel.StartOrStop();
		}
	}
}