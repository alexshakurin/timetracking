using System;
using System.Net;
using TimeTracking.Export;

namespace TimeTracker.RestApiExport
{
	public class RestApiTimeService : ITimeService
	{
		private static readonly object syncRoot = new object();

		private readonly string serverUrl;
		private readonly string userId;
		private TimeSpan previousTime;

		public RestApiTimeService(string serverUrl, string userId)
		{
			this.serverUrl = serverUrl;
			this.userId = userId;
			previousTime = TimeSpan.MinValue;
		}

		public void SaveTime(TimeSpan currentTime, DateTime date)
		{
			lock (syncRoot)
			{
				var span = currentTime.Subtract((previousTime == TimeSpan.MinValue
					? currentTime
					: previousTime));

				var totalMinutes = ((int) span.TotalMinutes);

				if (totalMinutes > 0)
				{
					var client = new WebClient();
					client.UploadString(GetPostAddress(), totalMinutes.ToString());
					previousTime = currentTime;
				}

				if (previousTime == TimeSpan.MinValue)
				{
					previousTime = currentTime;
				}
			}
		}

		public TimeSpan? LoadTime(DateTime date)
		{
			lock (syncRoot)
			{
				TimeSpan? result;
				var client = new WebClient();
				var time = client.DownloadString(GetReadAddress());
				TimeSpan timeFromServer;
				if (TimeSpan.TryParse(time, out timeFromServer))
				{
					result = timeFromServer;
				}
				else
				{
					result = null;
				}

				return result;
			}
		}

		private string GetPostAddress()
		{
			return string.Format("http://{0}/timetracking/api/tracktime/{1}",
				serverUrl,
				userId);
		}

		private string GetReadAddress()
		{
			return string.Format("http://{0}/timetracking/api/gettime/{1}/today",
				serverUrl,
				userId);
		}
	}
}