using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TimeTracker.Controls
{
	/// <summary>
	/// Interaction logic for DayTimeTrackingDetails.xaml
	/// </summary>
	public partial class DayTimeTrackingDetails
	{
		private const int minuteColumnWidth = 25;

		public DayTimeTrackingDetails()
		{
			InitializeComponent();

			AddMinuteColumnsToGrid(minutesColumnGrid);

			foreach (var minute in Enumerable.Range(0, 60))
			{
				var textBlock = new TextBlock
					{
						Text = TimeSpan.FromMinutes(minute).ToString("mm"),
						Width = minuteColumnWidth,
						Margin = new Thickness(3)
					};
				Grid.SetColumn(textBlock, minute);

				minutesColumnGrid.Children.Add(textBlock);
			}
		}

		private static void AddMinuteColumnsToGrid(Grid grid)
		{
			grid.ColumnDefinitions.Clear();

			foreach (var minute in Enumerable.Range(0, 60))
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition
					{
						Width = new GridLength(minuteColumnWidth, GridUnitType.Pixel),
						SharedSizeGroup = "minute" + minute
					});
			}
		}

		private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
		{
			var targetGrid = sender as Grid;

			if (targetGrid != null)
			{
				AddMinuteColumnsToGrid(targetGrid);
			}
		}
	}
}
