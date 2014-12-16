using Microsoft.Practices.ServiceLocation;

namespace TimeTracker
{
	public class ViewModelLocator
	{
		private MainViewModel main;

		public MainViewModel Main
		{
			get
			{
				if (main == null)
				{
					main = ServiceLocator.Current.GetInstance<MainViewModel>();
				}

				return main;
			}
		}
	}
}