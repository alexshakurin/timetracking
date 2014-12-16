using System;
using Microsoft.Practices.ServiceLocation;
using TimeTracker.ViewModels;
using TimeTracker.ViewModels.ManualTime;
using TimeTracking.Extensions;

namespace TimeTracker.Views.ManualTime
{
	/// <summary>
	/// Interaction logic for EnterManualTimeView.xaml
	/// </summary>
	public partial class EnterManualTimeView : IEnterManualTimeView
	{
		public EnterManualTimeViewModel ViewModel { get; private set; }

		internal EnterManualTimeView()
			: this(ServiceLocator.Current.GetInstance<EnterManualTimeViewModel>())
		{
		}

		public EnterManualTimeView(EnterManualTimeViewModel viewModel)
		{
			ViewModel = viewModel;
			DataContext = ViewModel;
			ViewModel.CloseRequest += ViewModelOnCloseRequest;

			InitializeComponent();
		}

		private void ViewModelOnCloseRequest(object sender, CloseEventArgs e)
		{
			if (e.DialogResult != null)
			{
				DialogResult = e.DialogResult;
			}
			else
			{
				Close();
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			ViewModel.MaybeDo(vm => vm.CloseRequest -= ViewModelOnCloseRequest);
			ViewModel.MaybeDo(vm => vm.Cleanup());
			base.OnClosed(e);
		}
	}
}