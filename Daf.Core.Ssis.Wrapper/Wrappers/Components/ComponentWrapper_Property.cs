// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public partial class ComponentWrapper
	{
		protected T GetCustomProperty<T>(string propertyName)
		{
			return Meta.CustomPropertyCollection[propertyName].Value;
		}

		protected void SetCustomProperty<T>(string propertyName, T value)
		{
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));

			Type[] types = { propertyName.GetType(), value.GetType() };
			object[] parameters = { propertyName, value };

			componentWrapperType.GetMethod("SetComponentProperty", types).Invoke(Instance, parameters);
		}

		protected void SetInputColumnProperty(string columnName, string propertyName, dynamic value)
		{
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));

			dynamic inputCollection = Meta.InputCollection[0];

			dynamic inputColumn;
			if (InputColumnExists(columnName, inputCollection.InputColumnCollection))
				inputColumn = inputCollection.InputColumnCollection[columnName];
			else
				inputColumn = SetInputColumnUsageType(columnName, inputCollection, Enum.Parse(dtsUsageTypeType, "UT_READONLY"));

			Type[] types = { inputCollection.ID.GetType(), inputColumn.ID.GetType(), propertyName.GetType(), value.GetType() };
			object[] parameters = { inputCollection.ID, inputColumn.ID, propertyName, value };

			componentWrapperType.GetMethod("SetInputColumnProperty", types).Invoke(Instance, parameters);
		}

		protected void SetOutputColumnProperty(string columnName, string propertyName, dynamic value)
		{
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));

			dynamic outputCollection = Meta.OutputCollection[0];

			if (!OutputColumnExists(columnName, outputCollection))
				InsertOutputColumn(columnName, outputCollection);

			dynamic outputColumn = outputCollection.OutputColumnCollection[columnName];

			Type[] types = { outputCollection.ID.GetType(), outputColumn.ID.GetType(), propertyName.GetType(), value.GetType() };
			object[] parameters = { outputCollection.ID, outputColumn.ID, propertyName, value };

			componentWrapperType.GetMethod("SetOutputColumnProperty", types).Invoke(Instance, parameters);
		}
	}
}
