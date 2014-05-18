using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace TimeTracker.ViewModels
{
	public class ClosableViewModel : ViewModel
	{
		private ICommand okCommand;
		private ICommand cancelCommand;

		private EventHandler<CloseEventArgs> closeRequestHandler;

		public event EventHandler<CloseEventArgs> CloseRequest
		{
			add
			{
				closeRequestHandler += value;
			}
			remove
			{
				closeRequestHandler -= value;
			}
		}

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

		public override void Cleanup()
		{
			closeRequestHandler = null;
			base.Cleanup();
		}

		private void OnCloseRequest(CloseEventArgs args)
		{
			var local = closeRequestHandler;
			if (local != null)
			{
				local(this, args);
			}
		}
	}
}