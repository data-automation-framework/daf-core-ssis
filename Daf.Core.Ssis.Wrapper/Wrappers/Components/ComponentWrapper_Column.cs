// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Ssis.Wrapper.Enums;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public partial class ComponentWrapper
	{
		// Set error row disposition on column level for components that support it e.g. derived column.
		public void SetColumnDisposition(string columnName, RowDispositionType rowDispositionType, RowFailureType rowFailureType, PathDirectionType direction)
		{
			dynamic column;
			if (direction == PathDirectionType.Input)
				column = Meta.InputCollection[0].InputColumnCollection[columnName];
			else
				column = Meta.OutputCollection[0].OutputColumnCollection[columnName];

			SetDisposition(column, rowDispositionType, rowFailureType);
		}

		protected dynamic SetInputColumnUsageType(string columnName, dynamic inputCollection, dynamic usageType)
		{
			if (InputColumnExists(columnName, inputCollection.InputColumnCollection))
				return inputCollection.InputColumnCollection[columnName];
			else
			{
				dynamic virtualInput = inputCollection.GetVirtualInput();
				int virtualInputColumnLineageId = virtualInput.VirtualInputColumnCollection[columnName].LineageID;

				Type[] types = { inputCollection.ID.GetType(), virtualInputType, virtualInputColumnLineageId.GetType(), usageType.GetType() };
				object[] parameters = { inputCollection.ID, virtualInput, virtualInputColumnLineageId, usageType };

				return componentWrapperType.GetMethod("SetUsageType", types).Invoke(Instance, parameters);
			}
		}

		protected static bool VirtualInputColumnExists(string columnName, dynamic inputCollection)
		{
			dynamic virtualInput = inputCollection.GetVirtualInput();

			foreach (dynamic virtualInputColumn in virtualInput.VirtualInputColumnCollection)
			{
				if (string.Compare(virtualInputColumn.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}

			return false;
		}

		private bool CustomSourceMappingExists(string columnName)
		{
			if (CustomMappingColumns.Count != 0)
			{
				foreach ((string SourceColumn, _) in CustomMappingColumns)
				{
					if (string.Equals(SourceColumn, columnName, StringComparison.OrdinalIgnoreCase))
						return true;
				}
			}

			return false;
		}

		private bool CustomTargetMappingExists(string columnName)
		{
			if (CustomMappingColumns.Count != 0)
			{
				foreach ((_, string TargetColumn) in CustomMappingColumns)
				{
					if (string.Equals(TargetColumn, columnName, StringComparison.OrdinalIgnoreCase))
						return true;
				}
			}

			return false;
		}

		private static bool ExternalMetadataColumnExists(string columnName, dynamic externalMetadataColumnCollection)
		{
			foreach (dynamic externalMetadataColumn in externalMetadataColumnCollection)
			{
				if (string.Compare(externalMetadataColumn.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}

			return false;
		}

		private static dynamic GetExternalMetadataColumnByName(string columnName, dynamic externalMetadataColumnCollection)
		{
			foreach (dynamic externalMetadataColumn in externalMetadataColumnCollection)
			{
				if (string.Compare(externalMetadataColumn.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0)
					return externalMetadataColumn;
			}

			return null;
		}

		private static dynamic GetInputColumn(string columnName, int lineageId, dynamic inputColumnCollection)
		{
			try
			{
				return inputColumnCollection.GetInputColumnByLineageID(lineageId);
			}
#pragma warning disable CA1031 // Do not catch general exception types. Don't know of any other way to handle InputColumns not existing.
			catch
#pragma warning restore CA1031 // Do not catch general exception types
			{
				// If it doesn't already exist, create and return the newly created column.
				dynamic inputColumn = inputColumnCollection.New();
				inputColumn.Name = columnName;
				inputColumn.LineageID = lineageId;

				return inputColumn;
			}
		}

		private static dynamic GetVirtualInputColumnByName(string columnName, dynamic virtualInputColumnCollection)
		{
			foreach (dynamic virtualInputColumn in virtualInputColumnCollection)
			{
				if (string.Compare(virtualInputColumn.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0)
					return virtualInputColumn;
			}

			return null;
		}

		private static bool InputColumnExists(string columnName, dynamic inputColumnCollection)
		{
			foreach (dynamic column in inputColumnCollection)
			{
				if (string.Compare(column.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}

			return false;
		}

		private void InsertOutputColumn(string columnName, dynamic outputCollection)
		{
			dynamic outputColumns = outputCollection.OutputColumnCollection;
			string outputColumnName = $"OUTPUT COLUMN {columnName}";

			Type[] types = { outputCollection.ID.GetType(), outputColumns.Count.GetType(), columnName.GetType(), outputColumnName.GetType() };
			object[] parameters = { outputCollection.ID, outputColumns.Count, columnName, outputColumnName };

			componentWrapperType.GetMethod("InsertOutputColumnAt", types).Invoke(Instance, parameters);
		}

		private void LinkVirtualInputToOutput(dynamic virtualInputColumn, dynamic virtualInput, string columnName, dynamic inputCollection)
		{
			if (InputColumnExists(columnName, inputCollection.InputColumnCollection))
				return;

			dynamic readOnly = Enum.Parse(dtsUsageTypeType, "UT_READONLY");

			Type[] types = { inputCollection.ID.GetType(), virtualInputType, virtualInputColumn.LineageID.GetType(), readOnly.GetType() };
			object[] parameters = { inputCollection.ID, virtualInput, virtualInputColumn.LineageID, readOnly };

			componentWrapperType.GetMethod("SetUsageType", types).Invoke(Instance, parameters);
		}

		private void MapInputColumn(dynamic inputColumn, dynamic externalMetadataColumn, dynamic input)
		{
			Type[] types = { input.ID.GetType(), inputColumn.ID.GetType(), externalMetadataColumn.ID.GetType() };
			object[] parameters = { input.ID, inputColumn.ID, externalMetadataColumn.ID };

			componentWrapperType.GetMethod("MapInputColumn", types).Invoke(Instance, parameters);
		}

		private static bool OutputColumnExists(string columnName, dynamic outputCollection)
		{
			foreach (dynamic column in outputCollection.OutputColumnCollection)
			{
				if (string.Compare(column.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}

			return false;
		}
	}
}
