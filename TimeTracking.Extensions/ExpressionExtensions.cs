using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TimeTracking.Extensions
{
	public static class ExpressionExtensions
	{
		public static bool PropertyNameIn(this string propertyName,
			IEnumerable<Expression<Func<object>>> collection)
		{
			var propertyNameFound = false;

			foreach (var expression in collection)
			{
				var memberExpression = (expression.Body as MemberExpression)
					.Maybe(me => me.Member)
					.Maybe(m => m.Name);

				var unaryExpression = (expression.Body as UnaryExpression)
					.Maybe(ue => ue.Operand as MemberExpression)
					.Maybe(me => me.Member)
					.Maybe(m => m.Name);

				if (string.IsNullOrEmpty(memberExpression) && string.IsNullOrEmpty(unaryExpression))
				{
					throw new NotSupportedException(string.Format(
						"Expression of type '{0}' is not yet supported",
						expression.Body.Maybe(b => b.GetType())));
				}

				propertyNameFound = propertyName.In(new[] {unaryExpression, memberExpression}.Where(c => !string.IsNullOrEmpty(c)));

				if (propertyNameFound)
				{
					break;
				}
			}

			return propertyNameFound;
		}

	}
}