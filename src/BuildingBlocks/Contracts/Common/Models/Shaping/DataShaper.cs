﻿using System.Reflection;

namespace Contracts.Common.Models.Shaping
{
	public class DataShaper<T> : IDataShaper<T>
	{
		public PropertyInfo[] Properties { get; set; } = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

		public IEnumerable<T> ShapeData(IEnumerable<T> entities, string fieldsString)
		{
			var requiredProperties = GetRequiredProperties(fieldsString);

			return FetchData(entities, requiredProperties);
		}

		public T ShapeData(T entity, string fieldsString)
		{
			var requiredProperties = GetRequiredProperties(fieldsString);

			return FetchDataForEntity(entity, requiredProperties);
		}

		private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
		{
			var requiredProperties = new List<PropertyInfo>();

			if (!string.IsNullOrWhiteSpace(fieldsString))
			{
				var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

				foreach (var field in fields)
				{
					var property = Properties.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

					if (property == null)
						continue;

					requiredProperties.Add(property);
				}
			}
			else
			{
				requiredProperties = Properties.ToList();
			}

			return requiredProperties;
		}

		private IEnumerable<T> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
		{
			var shapedData = new List<T>();

			foreach (var entity in entities)
			{
				var shapedObject = FetchDataForEntity(entity, requiredProperties);
				shapedData.Add(shapedObject);
			}

			return shapedData;
		}

		private T FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
		{
			var shapedObject = Activator.CreateInstance<T>();

			foreach (var property in requiredProperties)
			{
				var objectPropertyValue = property.GetValue(entity);
				property.SetValue(shapedObject, objectPropertyValue);
			}

			return shapedObject;
		}
	}
}
