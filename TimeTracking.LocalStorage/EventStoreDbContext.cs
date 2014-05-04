using System.Data.Entity;

namespace TimeTracking.LocalStorage
{
	public class EventStoreDbContext : DbContext
	{
		//public const string SchemaName = "Events";

		public EventStoreDbContext(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<StoredEvent>().HasKey(x => new {x.AggregateId, x.AggregateType, x.Version})
				.ToTable("storedevents");
		}
	}
}