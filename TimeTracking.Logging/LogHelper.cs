using System;
using System.Deployment.Application;
using log4net;

namespace TimeTracking.Logging
{
	public static class LogHelper
	{
		private static readonly ILog logger;

		static LogHelper()
		{
			var version = "1.0.0.0 (debug)";

			if (ApplicationDeployment.IsNetworkDeployed)
			{
				var deployment = ApplicationDeployment.CurrentDeployment;

				if (deployment != null && deployment.CurrentVersion != null)
				{
					version = deployment.CurrentVersion.ToString();
				}
			}

			log4net.Config.XmlConfigurator.Configure();
			GlobalContext.Properties["VersionContext"] = version;

			logger = LogManager.GetLogger(typeof(LogHelper));
		}

		public static void Debug(string debug)
		{
			logger.Debug(debug);
		}

		public static void Error(string error)
		{
			logger.Error(error);
		}
	}
}