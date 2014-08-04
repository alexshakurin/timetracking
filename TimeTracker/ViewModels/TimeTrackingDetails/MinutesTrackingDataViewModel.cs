using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using TimeTracker.Localization;
using TimeTracker.Views.IntervalsManagement;
using TimeTracking.ApplicationServices.Dialogs;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class MinutesTrackingDataViewModel : ViewModel
	{
		private readonly ILocalizationService localizationService;
		private readonly TimeSpan rangeStart;
		private readonly TimeSpan rangeEnd;
		private readonly int hour;
		private ICommand manageIntervals;
		private ICommand deleteThisHour;
		private ICommand deleteWholeInterval;

		public int Minute { get; private set; }

		public int MinutesLength { get; private set; }

		public string Memo { get; private set; }

		public string Description { get; private set; }

		public ICommand DeleteThisHour
		{
			get
			{
				if (deleteThisHour == null)
				{
					deleteThisHour = new RelayCommand(ExecuteDeleteThisHour);
				}

				return deleteThisHour;
			}
		}

		public ICommand DeleteWholeInterval
		{
			get
			{
				if (deleteWholeInterval == null)
				{
					deleteWholeInterval = new RelayCommand(ExecuteDeleteWholeInterval);
				}

				return deleteWholeInterval;
			}
		}

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

		public MinutesTrackingDataViewModel(ILocalizationService localizationService,
			int hour,
			int minuteStart,
			int minutesLength,
			TimeSpan rangeStart,
			TimeSpan rangeEnd,
			string memo)
		{
			this.rangeStart = rangeStart;
			this.rangeEnd = rangeEnd;
			this.hour = hour;
			this.localizationService = localizationService;
			Minute = minuteStart;
			MinutesLength = minutesLength;

			var range = string.Format("{0} - {1}", rangeStart, rangeEnd);
			Description = string.Format("{0} ({1})", range, memo);

			Memo = memo;
		}

		private bool ConfirmDeleteInterval(TimeSpan start, TimeSpan end)
		{
			var aboutToDeleteMessage = localizationService.GetLocalizedString("AboutToDeleteIntervalMessage");
			var proceedMessage = localizationService.GetLocalizedString("ProceedDeleteIntervalMessage");

			var intervalMessage = string.Format("{0} - {1}", start, end);

			var fullMessage = string.Format("{0}{1}{2}",
				aboutToDeleteMessage + Environment.NewLine + Environment.NewLine,
				intervalMessage + Environment.NewLine + Environment.NewLine,
				proceedMessage);

			var messageBox = ServiceLocator.Current.GetInstance<IMessageBoxService>();
			var result = messageBox.Show(fullMessage,
				localizationService.GetLocalizedString("ConfirmDeleteInterval"),
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning);

			return result == MessageBoxResult.Yes;
		}

		private void ExecuteDeleteWholeInterval()
		{
			if (ConfirmDeleteInterval(rangeStart, rangeEnd))
			{
				// TODO: Remove interval
			}
		}

		private void ExecuteDeleteThisHour()
		{
			if (ConfirmDeleteInterval(new TimeSpan(hour, 0, 0), new TimeSpan(hour, 59, 59)))
			{
				// TODO: Remove interval
			}
		}

		private void ExecuteManageIntervals()
		{
			var view = ServiceLocator.Current.GetInstance<IManageIntervalsView>();
			view.ShowDialog();
		}

		
	}
}