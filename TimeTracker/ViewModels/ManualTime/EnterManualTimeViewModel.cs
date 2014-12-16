using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using TimeTracker.Localization;
using TimeTracker.Messages;
using TimeTracking.Commands;

namespace TimeTracker.ViewModels.ManualTime
{
	public class EnterManualTimeViewModel : ClosableViewModel
	{
		private readonly ILocalizationService localizationService;
		private DateTime startDate;
		private DateTime startTime;
		private DateTime endDate;
		private DateTime endTime;

		private ICommand addManualTimeCommand;

		private string memo;

		public string Memo
		{
			get { return memo; }
			set
			{
				if (memo == value)
				{
					return;
				}

				memo = value;
				RaisePropertyChanged(() => Memo);
			}
		}

		public ObservableCollection<string> Errors { get; private set; }

		public ICommand AddManualTimeCommand
		{
			get
			{
				if (addManualTimeCommand == null)
				{
					addManualTimeCommand = new RelayCommand(AddManualTime, CanAddManualTime);
				}

				return addManualTimeCommand;
			}
		}

		public DateTime EndTime
		{
			get { return endTime; }
			set
			{
				if (endTime == value)
				{
					return;
				}

				endTime = value;
				RaisePropertyChanged(() => EndTime);
			}
		}

		public DateTime EndDate
		{
			get { return endDate; }
			set
			{
				if (endDate == value)
				{
					return;
				}

				endDate = value;
				RaisePropertyChanged(() => EndDate);
			}
		}

		public DateTime StartTime
		{
			get { return startTime; }
			set
			{
				if (startTime == value)
				{
					return;
				}

				startTime = value;
				RaisePropertyChanged(() => StartTime);
			}
		}

		public DateTime StartDate
		{
			get { return startDate; }
			set
			{
				if (startDate == value)
				{
					return;
				}

				startDate = value;
				RaisePropertyChanged(() => StartDate);
			}
		}

		public EnterManualTimeViewModel(ILocalizationService localizationService)
		{
			this.localizationService = localizationService;
			Errors = new ObservableCollection<string>();

			StartDate = DateTime.Now.Date;
			EndDate = DateTime.Now.Date;
			StartTime = DateTime.Now.Subtract(TimeSpan.FromHours(3));
			EndTime = DateTime.Now.Subtract(TimeSpan.FromHours(2));
		}

		private bool CanAddManualTime()
		{
			return ValidateTime();
		}

		private DateTime GetStart()
		{
			return StartDate.Date.Add(StartTime.TimeOfDay);
		}

		private DateTime GetEnd()
		{
			return EndDate.Date.Add(EndTime.TimeOfDay);
		}

		private bool ValidateTime()
		{
			Errors.Clear();

			var now = DateTime.Now;

			var resultStart = GetStart();
			var resultEnd = GetEnd();

			if (resultEnd < resultStart)
			{
				Errors.Add(localizationService.GetLocalizedString(
					"ValidateManualTime_EndGreaterThanStart"));
			}

			if (resultEnd > now)
			{
				Errors.Add(localizationService.GetLocalizedString(
					"ValidateManualTime_EndGreaterThanNow"));
			}

			if (StartDate.Date != EndDate.Date)
			{
				Errors.Add(localizationService.GetLocalizedString(
					"ValidateManualTime_StartAndEndMustBeTheSameDate"));
			}

			var difference = now - resultEnd;

			const int minMinutes = 1;

			if (difference.TotalMinutes < minMinutes)
			{
				Errors.Add(localizationService.GetLocalizedString(
					"ValidateManualTime_EndDateMustBeAtLeastOneMinute"));
			}

			return Errors.Count == 0;
		}

		private void AddManualTime()
		{
			if (CanAddManualTime())
			{
				var key = TimeTrackingViewModel.GetKey(StartDate.Date);
				var command = new RegisterTimeCommand(key.Key,
					key.Date,
					new DateTimeOffset(GetStart()),
					new DateTimeOffset(GetEnd()),
					Memo,
					"Manual time");

				MessengerInstance.Send(new ManualTimeRegistered(command));

				MessageBox.Show(localizationService.GetLocalizedString("ManualTimeAdded_Message"),
					localizationService.GetLocalizedString("ManualTimeAdded_Caption"));
			}
		}
	}
}