using System;
using GalaSoft.MvvmLight;
using TimeTracking.Extensions;

namespace TimeTracker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			(DataContext as ICleanup).MaybeDo(c => c.Cleanup());
		}
	}
}