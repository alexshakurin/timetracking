using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using TimeTracker.FileSystemExport;
using TimeTracker.RestApiExport;
using TimeTracking.Export;
using TimeTracking.Infrastructure;
using TimeTracking.Infrastructure.Impl;

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
			container.RegisterType<IEventBus, EventBus>();
			container.RegisterType<MainViewModel>();
			//container.RegisterType<ITimeService, FileSystemTimeService>();
			var service = new RestApiTimeService("localhost:9000", "2");
			container.RegisterInstance<ITimeService>(service);

			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
		}
	}
}