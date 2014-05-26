using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using TimeTracker.Localization;
using TimeTracker.RestApiExport;
using TimeTracker.Views.ChangeTask;
using TimeTracking.ApplicationServices.Dialogs;
using TimeTracking.CommandHandlers;
using TimeTracking.Commands;
using TimeTracking.EventHandlers;
using TimeTracking.Export;
using TimeTracking.Infrastructure;
using TimeTracking.Infrastructure.CommandHandlers;
using TimeTracking.Infrastructure.Impl;
using TimeTracking.Infrastructure.Serialization;
using TimeTracking.LocalStorage;
using TimeTracking.Logging;
using TimeTracking.Model.Events;
using TimeTracking.ReadModel;

namespace TimeTracker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		private IUnityContainer container;

		static App()
		{
			DispatcherHelper.Initialize();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			//System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");
			//System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

			LogHelper.Debug("Starting application");

			base.OnStartup(e);
			container = new UnityContainer();

			var eventDispather = new EventDispatcher();
			eventDispather.Register<WorkingTimeRegistered>();
			container.RegisterInstance(eventDispather);
			container.RegisterType<IEventHandler<WorkingTimeRegistered>, WorkingTimeRegisteredEventHandler>();
			container.RegisterType<ILocalizationService, LocalizationService>();
			container.RegisterType<IMessageBoxService, MessageBoxService>();
			container.RegisterType<ITimeTrackingViewModel, TimeTrackingViewModel>();
			container.RegisterType<IEventBus, EventBus>();
			container.RegisterType<ICommandBus, SynchronousCommandBus>();
			container.RegisterType<MainViewModel>();
			//container.RegisterType<ITimeService, FileSystemTimeService>();
			var service = new RestApiTimeService("localhost:9000", "2");
			container.RegisterInstance<ITimeService>(service);
			container.RegisterInstance<ITextSerializer>(new JsonTextSerializer());

			container.RegisterType<ICommandHandler<RegisterTimeCommand>, RegisterTimeCommandHandler>();
			container.RegisterType<EventStoreDbContext>(new TransientLifetimeManager(),
				new InjectionConstructor("EventStore"));
			container.RegisterType(typeof(IEventSourcedRepository<>),
				typeof(SqlEventSourcedRepository<>),
				new ContainerControlledLifetimeManager());
			container.RegisterType<ReadModelRepository>();

			container.RegisterType<IChangeTaskView, ChangeTaskView>();

			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
		}
	}
}