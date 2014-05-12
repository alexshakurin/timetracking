using System;
using TimeTracker.ViewModels.ChangeTask;
using TimeTracking.Extensions;

namespace TimeTracker.Views.ChangeTask
{
	/// <summary>
	/// Interaction logic for ChangeTaskView.xaml
	/// </summary>
	public partial class ChangeTaskView : IChangeTaskView
	{
		public ChangeTaskViewModel ViewModel { get; private set; }

		public ChangeTaskView(ChangeTaskViewModel viewModel)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException();
			}

			DataContext = viewModel;
			ViewModel = viewModel;
			InitializeComponent();
		}

		internal ChangeTaskView() : this(new ChangeTaskViewModel { Memo = "Lorem ipsum"})
		{
		}

		protected override void OnClosed(System.EventArgs e)
		{
			ViewModel.MaybeDo(vm => vm.Cleanup());
			base.OnClosed(e);
		}
	}
}