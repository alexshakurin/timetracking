using System;

namespace TimeTracking.Core
{
	public interface ITimeTrackingCore : IDisposable
	{
		void Start();

		void Stop();

		void ChangeMemo(string memo);
	}
}