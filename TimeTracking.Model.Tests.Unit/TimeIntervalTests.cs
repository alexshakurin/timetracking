using System;
using NUnit.Framework;

namespace TimeTracking.Model.Tests.Unit
{
	[TestFixture]
	public class TimeIntervalTests
	{
		[Test]
		public void TestIntersect_FirstBeforeSecond_NotIntersect()
		{
			var start1 = new DateTime(2010, 1, 1, 10, 0, 0, 0);
			var end1 = start1.AddMinutes(10);

			var interval1 = new TimeInterval(new DateTimeOffset(start1), new DateTimeOffset(end1));
			var interval2 = new TimeInterval(new DateTimeOffset(end1.AddMinutes(30)), new DateTimeOffset(end1.AddMinutes(50)));

			Assert.IsFalse(interval1.Intersects(interval2));
			Assert.IsFalse(interval2.Intersects(interval1));
		}

		[Test]
		public void TestIntersect_FirstAfterSecond_NotIntersect()
		{
			var start1 = new DateTime(2010, 1, 1, 10, 0, 0, 0);
			var end1 = start1.AddMinutes(10);
			var start2 = end1.AddMinutes(10);
			var end2 = start2.AddMinutes(10);

			var interval1 = new TimeInterval(new DateTimeOffset(start1), new DateTimeOffset(end1));
			var interval2 = new TimeInterval(new DateTimeOffset(start2), new DateTimeOffset(end2));

			Assert.IsFalse(interval1.Intersects(interval2));
			Assert.IsFalse(interval2.Intersects(interval1));
		}

		[Test]
		public void TestIntersect_FirstInMiddleOfSecond_Intersect()
		{
			var start1 = new DateTime(2010, 1, 1, 10, 0, 0, 0);
			var end1 = start1.AddMinutes(50);

			var interval1 = new TimeInterval(new DateTimeOffset(start1), new DateTimeOffset(end1));
			var interval2 = new TimeInterval(new DateTimeOffset(start1.AddMinutes(10)), new DateTimeOffset(start1.AddMinutes(20)));

			Assert.IsTrue(interval1.Intersects(interval2));
			Assert.IsTrue(interval2.Intersects(interval1));
		}

		[Test]
		public void TestIntersect_StartsInMiddleEndsLater_Intersect()
		{
			var start1 = new DateTime(2010, 1, 1, 10, 0, 0, 0);
			var end1 = start1.AddMinutes(50);

			var interval1 = new TimeInterval(new DateTimeOffset(start1), new DateTimeOffset(end1));
			var interval2 = new TimeInterval(new DateTimeOffset(start1.AddMinutes(10)), new DateTimeOffset(end1.AddMinutes(20)));

			Assert.IsTrue(interval1.Intersects(interval2));
			Assert.IsTrue(interval2.Intersects(interval1));
		}

		[Test]
		public void TestIntersect_StartsJustAfterEnd_NotIntersect()
		{
			var start1 = new DateTime(2010, 1, 1, 10, 0, 0, 0);
			var end1 = start1.AddMinutes(50);
			var start2 = end1;
			var end2 = start2.AddMinutes(10);

			var interval1 = new TimeInterval(new DateTimeOffset(start1), new DateTimeOffset(end1));
			var interval2 = new TimeInterval(new DateTimeOffset(start2), new DateTimeOffset(end2));

			Assert.IsFalse(interval1.Intersects(interval2));
			Assert.IsFalse(interval2.Intersects(interval1));
		}

		[Test]
		public void TestIntersect_Self_Intersect()
		{
			var start1 = new DateTime(2010, 1, 1, 10, 0, 0, 0);
			var end1 = start1.AddMinutes(50);

			var interval1 = new TimeInterval(new DateTimeOffset(start1), new DateTimeOffset(end1));

			Assert.IsTrue(interval1.Intersects(interval1));
		}
	}
}
