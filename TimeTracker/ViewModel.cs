using GalaSoft.MvvmLight;

namespace TimeTracker
{
	public class ViewModel : ViewModelBase
	{
		public override void Cleanup()
		{
			base.Cleanup();
			MessengerInstance.Unregister(this);
		}
	}
}