using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace TimeTracker.Views
{
	public interface IViewWithViewModel<TViewModel> : IView
		where TViewModel : ICleanup, INotifyPropertyChanged
	{
		TViewModel ViewModel { get; }
	}
}