﻿using System;

namespace TimeTracking.Model.Events
{
	public class WorkingTimeRegistered : VersionedEvent
	{
		public DateTime Date { get; private set; }

		public int Minutes { get; private set; }

		public string Memo { get; private set; }

		public WorkingTimeRegistered(DateTime date, int minutes, string memo)
		{
			if (minutes == 0)
			{
				throw new ArgumentException("Minutes cannot be zero", "minutes");
			}

			Date = date.Date;
			Minutes = minutes;
			Memo = memo;
		}
	}
}