using GalaSoft.MvvmLight;

namespace TimeTracker
{
	public abstract class ViewModel : ViewModelBase
	{
		public override void Cleanup()
		{
			base.Cleanup();
			MessengerInstance.Unregister(this);
		}
	}
}