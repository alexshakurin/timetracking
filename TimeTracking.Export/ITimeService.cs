using System;

namespace TimeTracking.Export
{
    public interface ITimeService
    {
	    void SaveTime(TimeSpan currentTime, DateTime date);

	    TimeSpan? LoadTime(DateTime date);
    }
}