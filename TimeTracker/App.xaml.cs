﻿using System;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using TimeTracker.Localization;
using TimeTracker.RestApiExport;
using TimeTracker.Views.ChangeTask;
using TimeTracker.Views.ManualTime;
using TimeTracking.ApplicationServices.Dialogs;
using TimeTracking.CommandHandlers;
using TimeTracking.Commands;
using TimeTracking.EventHandlers;
using TimeTracking.Export;
using TimeTracking.Extensions;
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
			//System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");
			//System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

			LogHelper.Debug("Starting application");

			base.OnStartup(e);
			container = new UnityContainer();

			var eventDispather = new EventDispatcher();
			eventDispather.Register<WorkingTimeRegistered>();

			container.RegisterInstance(eventDispather);
			container.RegisterType<IEventHandler<WorkingTimeRegistered>, WorkingTimeRegisteredEventHandler>("readModelHandler");
			container.RegisterType<IEventHandler<WorkingTimeRegistered>, WorkingTimeRegisteredFileWriterHandler>("fileSystemHandler");

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
			container.RegisterType<IEnterManualTimeView, EnterManualTimeView>();

			ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
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