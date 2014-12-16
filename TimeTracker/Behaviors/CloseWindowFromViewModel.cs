using System.Windows;
using System.Windows.Interactivity;
using TimeTracker.ViewModels;

namespace TimeTracker.Behaviors
{
	public class CloseWindowFromViewModel : Behavior<Window>
	{
		public ClosableViewModel ViewModel
		{
			get { return (ClosableViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register("ViewModel",
				typeof(ClosableViewModel),
				typeof(CloseWindowFromViewModel));

		protected override void OnAttached()
		{
			var viewModel = ViewModel ?? AssociatedObject.DataContext as ClosableViewModel;

			if (viewModel != null)
			{
				viewModel.CloseRequest += OnViewModelRequestClose;
			}
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			var viewModel = ViewModel ?? AssociatedObject.DataContext as ClosableViewModel;

			if (viewModel != null)
			{
				viewModel.CloseRequest -= OnViewModelRequestClose;
			}
		}

		private void OnViewModelRequestClose(object sender, CloseEventArgs e)
		{
			if (e.DialogResult != null)
			{
				AssociatedObject.DialogResult = e.DialogResult;
			}
			else
			{
				AssociatedObject.Close();
			}
		}
	}
}