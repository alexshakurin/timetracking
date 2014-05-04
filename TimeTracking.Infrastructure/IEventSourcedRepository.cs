﻿using System;
using TimeTracking.Model;

namespace TimeTracking.Infrastructure
{
	public interface IEventSourcedRepository<T> where T : IEventSourced
	{
		/// <summary>
		/// Tries to retrieve the event sourced entity.
		/// </summary>
		/// <param name="id">The id of the entity</param>
		/// <returns>The hydrated entity, or null if it does not exist.</returns>
		T Find(Guid id);

		/// <summary>
		/// Saves the event sourced entity.
		/// </summary>
		/// <param name="eventSourced">The entity.</param>
		/// <param name="correlationId">A correlation id to use when publishing events.</param>
		void Save(T eventSourced, string correlationId);
	}
}