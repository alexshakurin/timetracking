using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using TimeTracker.FileSystemExport;
using TimeTracking.Export;

namespace TimeTracker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			var container = new UnityContainer();
			container.RegisterType<MainViewModel>();
			container.RegisterType<ITimeService, FileSystemTimeService>();

			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
		}
	}
}