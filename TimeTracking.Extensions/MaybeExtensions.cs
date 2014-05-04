using System;

namespace TimeTracking.Extensions
{
	public static class MaybeExtensions
	{
		public static TResult Maybe<TInput, TResult>(this TInput input, Func<TInput, TResult> func)
		{
			if (Equals(input, default(TInput)))
			{
				return default(TResult);
			}

			return func(input);
		}

		public static void MaybeDo<TInput>(this TInput input, Action<TInput> action)
		{
			if (!Equals(input, default(TInput)))
			{
				action(input);
			}
		}
	}
}