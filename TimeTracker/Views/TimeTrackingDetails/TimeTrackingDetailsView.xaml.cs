using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls;
using TimeTracker.ViewModels;
using TimeTracker.ViewModels.TimeTrackingDetails;
using TimeTracking.Extensions;

namespace TimeTracker.Views.TimeTrackingDetails
{
	/// <summary>
	/// Interaction logic for TimeTrackingDetailsView.xaml
	/// </summary>
	public partial class TimeTrackingDetailsView : ITimeTrackingDetailsView
	{
		public TimeTrackingDetailsView(ITimeTrackingDetailsViewModel viewModel)
		{
			DataContext = viewModel;
			InitializeComponent();
		}

		internal TimeTrackingDetailsView()
			: this(new TimeTrackingDetailsViewModel())
		{
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			(DataContext as ICleanup).MaybeDo(c => c.Cleanup());
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
