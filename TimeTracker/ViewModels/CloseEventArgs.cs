using System;

namespace TimeTracker.ViewModels
{
	public class CloseEventArgs : EventArgs
	{
		public bool? DialogResult { get; private set; }

		public CloseEventArgs(bool? dialogResult)
		{
			DialogResult = dialogResult;
		}
	}
}