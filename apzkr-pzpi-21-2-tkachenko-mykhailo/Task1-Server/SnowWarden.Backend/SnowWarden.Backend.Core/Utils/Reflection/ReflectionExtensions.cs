using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SnowWarden.Backend.Core.Utils.Reflection;

public static class ReflectionExtensions
{
	public static MemberExpression? GetMemberExpression(this Expression? expression)
	{
		if (expression is MemberExpression memberExpression)
		{
			return memberExpression;
		}

		if (expression is not LambdaExpression lambdaExpression) return null;

		return lambdaExpression.Body switch
		{
			MemberExpression body => body,
			UnaryExpression unaryExpression => (MemberExpression)unaryExpression.Operand,
			_ => null
		};
	}

	public static string GetPropertyPath(this Expression expr)
	{
		StringBuilder path = new();
		MemberExpression? memberExpression = expr.GetMemberExpression();
		do
		{
			if (path.Length > 0)
			{
				path.Insert(0, ".");
			}
			path.Insert(0, memberExpression?.Member.Name);
			memberExpression = GetMemberExpression(memberExpression?.Expression);
		}
		while (memberExpression != null);
		return path.ToString();
	}

	public static object? GetPropertyValue(this object obj, string propertyPath)
	{
		object? propertyValue = null;
		if (propertyPath.IndexOf('.') < 0)
		{
			Type? objType = obj.GetType();
			propertyValue = objType?.GetProperty(propertyPath)?.GetValue(obj, null);
			return propertyValue;
		}
		List<string> properties = propertyPath.Split('.').ToList();
		object? midPropertyValue = obj;
		while (properties.Count > 0)
		{
			string propertyName = properties.First();
			properties.Remove(propertyName);
			propertyValue = midPropertyValue?.GetPropertyValue(propertyName);
			midPropertyValue = propertyValue;
		}
		return propertyValue;
	}

	public static void SetPropertyValue(this object obj, string propertyPath, object value)
	{
		if (string.IsNullOrEmpty(propertyPath))
			throw new ArgumentException("Property path cannot be null or empty.", nameof(propertyPath));

		if (propertyPath.IndexOf('.') < 0)
		{
			Type objType = obj.GetType();
			PropertyInfo? property = objType.GetProperty(propertyPath);
			if (property == null)
				throw new ArgumentException($"Property '{propertyPath}' not found on type '{objType}'.");
			property.SetValue(obj, value);
			return;
		}

		List<string> properties = propertyPath.Split('.').ToList();
		object? midPropertyValue = obj;

		while (properties.Count > 1)
		{
			string propertyName = properties.First();
			properties.RemoveAt(0);
			PropertyInfo? property = midPropertyValue?.GetType().GetProperty(propertyName);
			if (property == null)
				throw new ArgumentException($"Property '{propertyName}' not found on type '{midPropertyValue?.GetType()}'.");
			object? nestedPropertyValue = property.GetValue(midPropertyValue);
			if (nestedPropertyValue == null)
			{
				nestedPropertyValue = Activator.CreateInstance(property.PropertyType);
				property.SetValue(midPropertyValue, nestedPropertyValue);
			}
			midPropertyValue = nestedPropertyValue;
		}

		string finalPropertyName = properties.First();
		PropertyInfo? finalProperty = midPropertyValue?.GetType().GetProperty(finalPropertyName);
		if (finalProperty == null)
			throw new ArgumentException($"Property '{finalPropertyName}' not found on type '{midPropertyValue?.GetType()}'.");
		finalProperty.SetValue(midPropertyValue, value);
	}
}