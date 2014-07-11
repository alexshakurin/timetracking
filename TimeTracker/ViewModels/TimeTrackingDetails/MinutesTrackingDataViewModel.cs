using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class MinutesTrackingDataViewModel : ViewModel
	{
		private ICommand manageIntervals;

		public int Minute { get; private set; }

		public int MinutesLength { get; private set; }

		public string Memo { get; private set; }

		public string Description { get; private set; }

		public ICommand ManageIntervals
		{
			get
			{
				if (manageIntervals == null)
				{
					manageIntervals = new RelayCommand(ExecuteManageIntervals);
				}

				return manageIntervals;
			}
		}

		private void ExecuteManageIntervals()
		{
			//MessageBox.Show("Done");
		}

		public MinutesTrackingDataViewModel(int minuteStart,
			int minutesLength,
			TimeSpan rangeStart,
			TimeSpan rangeEnd,
			string memo)
		{
			Minute = minuteStart;
			MinutesLength = minutesLength;

			var range = string.Format("{0} - {1}", rangeStart, rangeEnd);
			Description = string.Format("{0} ({1})", range, memo);

			Memo = memo;
		}
	}
}