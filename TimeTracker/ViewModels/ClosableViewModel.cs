using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace TimeTracker.ViewModels
{
	public class ClosableViewModel : ViewModel
	{
		private ICommand okCommand;
		private ICommand cancelCommand;

		public event EventHandler<CloseEventArgs> CloseRequest;

		public ICommand OkCommand
		{
			get
			{
				if (okCommand == null)
				{
					okCommand = new RelayCommand(ExecuteOk, CanExecuteOk);
				}

				return okCommand;
			}
		}

		public ICommand CancelCommand
		{
			get
			{
				if (cancelCommand == null)
				{
					cancelCommand = new RelayCommand(ExecuteCancel);
				}

				return cancelCommand;
			}
		}

		protected virtual void ExecuteOk()
		{
			OnCloseRequest(new CloseEventArgs(true));
		}

		protected virtual bool CanExecuteOk()
		{
			return true;
		}

		protected virtual void ExecuteCancel()
		{
			OnCloseRequest(new CloseEventArgs(false));
		}

		private void OnCloseRequest(CloseEventArgs args)
		{
			var local = CloseRequest;
			if (local != null)
			{
				local(this, args);
			}
		}
	}
}