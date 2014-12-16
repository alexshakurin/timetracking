using System.Windows;

namespace TimeTracking.ApplicationServices.Dialogs
{
	public class MessageBoxService : IMessageBoxService
	{
		public MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image)
		{
			return MessageBox.Show(message, caption, button, image);
		}

		public MessageBoxResult ShowOkError(string message, string caption)
		{
			return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}