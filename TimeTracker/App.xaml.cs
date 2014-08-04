using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using TimeTracker.Localization;
using TimeTracker.Messages;
using TimeTracker.RestApiExport;
using TimeTracker.Settings;
using TimeTracker.ViewModels.TimeTrackingDetails;
using TimeTracker.Views.ChangeTask;
using TimeTracker.Views.IntervalsManagement;
using TimeTracker.Views.ManualTime;
using TimeTracker.Views.TimeTrackingDetails;
using TimeTracking.ApplicationServices.Dialogs;
using TimeTracking.ApplicationServices.Settings;
using TimeTracking.CommandHandlers;
using TimeTracking.Commands;
using TimeTracking.EventHandlers;
using TimeTracking.Export;
using TimeTracking.Extensions;
using TimeTracking.Extensions.Exceptions;
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

		public App()
		{
			DispatcherUnhandledException += OnDispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			if (!CheckSettings(new SettingsService()))
			{
				try
				{
					var exeFile = Process.GetCurrentProcess().MainModule.FileName;
					Process.Start(exeFile);
					Environment.Exit(-1);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
					throw new UnrecoverableApplicationException(null, ex);
				}
			}

			LogHelper.Debug("Starting application");

			base.OnStartup(e);
			container = new UnityContainer();

			var eventDispather = new EventDispatcher();
			eventDispather.Register<WorkingTimeRegistered>();

			container.RegisterInstance(eventDispather);
			container.RegisterType<IEventHandler<WorkingTimeRegistered>, WorkingTimeRegisteredEventHandler>("readModelHandler");
			container.RegisterType<IEventHandler<WorkingTimeRegistered>, WorkingTimeRegisteredFileWriterHandler>("fileSystemHandler");
			container.RegisterType<IEventHandler<WorkingTimeRegistered>, WorkingTimeRegisteredSettingsHandler>("settingsHandler");
			container.RegisterType<IEventHandler<WorkingTimeRegistered>, WorkingTimeRegisteredIntervalHandler>("intervalHandler");

			container.RegisterType<ISettingsService, SettingsService>();
			container.RegisterType<ILocalizationService, LocalizationService>();
			container.RegisterType<IMessageBoxService, MessageBoxService>();
			container.RegisterType<ITimeTrackingViewModel, TimeTrackingViewModel>();
			container.RegisterType<ITimeTrackingDetailsViewModel, TimeTrackingDetailsViewModel>();
			container.RegisterType<IEventBus, EventBus>();
			container.RegisterType<ICommandBus, SynchronousCommandBus>();
			container.RegisterType<MainViewModel>();

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

			container.RegisterType<IManageIntervalsView, ManageIntervalsView>();
			container.RegisterType<ITimeTrackingDetailsView, TimeTrackingDetailsView>();
			container.RegisterType<IChangeTaskView, ChangeTaskView>();
			container.RegisterType<IEnterManualTimeView, EnterManualTimeView>();

			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

			Messenger.Default.Register<OpenTimeTrackingDetailsViewMessage>(this,
				OnOpenTimeTrackingDetailsMessageReceived);
		}

		private void OnOpenTimeTrackingDetailsMessageReceived(OpenTimeTrackingDetailsViewMessage msg)
		{
			ITimeTrackingDetailsView existingWindow = null;

			foreach (Window window in Current.Windows)
			{
				if (window is ITimeTrackingDetailsView)
				{
					existingWindow = window as ITimeTrackingDetailsView;
					break;
				}
			}

			if (existingWindow == null)
			{
				existingWindow = ServiceLocator.Current.GetInstance<ITimeTrackingDetailsView>();
				existingWindow.Show();
			}

			existingWindow.Activate();
		}

		private bool CheckSettings(ISettingsService settingsService)
		{
			var success = true;

			try
			{
				settingsService.GetLatestMemo();
			}
			catch (ConfigurationErrorsException ex)
			{
				success = false;
				var fileName = (ex.InnerException as ConfigurationErrorsException)
					.Maybe(e => e.Filename);

				settingsService.DeleteSettingsFile(fileName);
				LogHelper.Error(string.Format("Error loading latest memo: {0}", ex));
			}

			return success;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			LogHelper.Error(e.Exception.ToString());

			var localizationService = LocalizationService.Default;

			var isRecoverable = !e.Exception.IsFatal();

			var lastInnerException = GetTheMostInnerException(e.Exception);

			var exceptionKey = lastInnerException.GetType().ToString();

			var errorText = localizationService.GetLocalizedString(exceptionKey + "_message");

			if (string.IsNullOrEmpty(errorText))
			{
				errorText = localizationService.GetLocalizedString("GenericErrorText")
					?? "Application has encountered unexpected error and will be shut down";
			}

			var errorCaption = localizationService.GetLocalizedString("GenericErrorCaption")
				?? "Error";

			MessageBox.Show(errorText,
				errorCaption,
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			e.Handled = true;
			if (!isRecoverable)
			{
				Shutdown();
			}
		}

		private Exception GetTheMostInnerException(Exception exception)
		{
			var inner = exception;

			while (inner.Maybe(e => e.InnerException) != null)
			{
				inner = inner.InnerException;
			}

			return inner;
		}

		private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			LogHelper.Error(e.ExceptionObject.ToString());
		}

	}
}