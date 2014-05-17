using System.Windows;

namespace TimeTracking.ApplicationServices.Dialogs
{
	public interface IMessageBoxService
	{
		MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image);

		MessageBoxResult ShowOkError(string message, string caption);
	}
}