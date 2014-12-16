using System;
using TimeTracker.ViewModels;
using TimeTracker.ViewModels.IntervalsManagement;
using TimeTracking.Extensions;

namespace TimeTracker.Views.IntervalsManagement
{
	/// <summary>
	/// Interaction logic for ManageIntervalsView.xaml
	/// </summary>
	public partial class ManageIntervalsView : IManageIntervalsView
	{
		public ManageIntervalsViewModel ViewModel { get; private set; }

		public ManageIntervalsView(ManageIntervalsViewModel viewModel)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException();
			}

			DataContext = viewModel;
			ViewModel = viewModel;
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

		internal ManageIntervalsView()
			: this(new ManageIntervalsViewModel())
		{
		}

		protected override void OnClosed(EventArgs e)
		{
			ViewModel.MaybeDo(vm => vm.CloseRequest -= ViewModelOnCloseRequest);
			ViewModel.MaybeDo(vm => vm.Cleanup());
			base.OnClosed(e);
		}
	}
}
