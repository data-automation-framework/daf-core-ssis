// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public class DerivedColumnComponentWrapper : ComponentWrapper
	{
		private readonly List<string> readWriteColumns;
		private bool readWriteColumnsPrepared;

		public DerivedColumnComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper, List<string> replaceColumns) : base(dataFlowTaskWrapper, "Microsoft.DerivedColumn")
		{
			readWriteColumns = replaceColumns;
		}

		public void AddNewColumn(string columnName, string expression)
		{
			if (!readWriteColumnsPrepared)
				PrepareReadWriteInputColumns();

			PrepareReadOnlyInputColumns(expression);

			SetOutputColumnProperty(columnName, "Expression", expression);
			SetOutputColumnProperty(columnName, "FriendlyExpression", expression);
		}

		public void ReplaceColumn(string columnName, string expression)
		{
			if (!readWriteColumnsPrepared)
				PrepareReadWriteInputColumns();

			PrepareReadOnlyInputColumns(expression);

			SetInputColumnProperty(columnName, "Expression", expression);
			SetInputColumnProperty(columnName, "FriendlyExpression", expression);
		}

		// Scans the expression for used columns and creates input columns from the virtual input column list.
		private void PrepareReadOnlyInputColumns(string friendlyExpression)
		{
			List<string> columnsUsedInExpression = new List<string>();
			dynamic virtualInput = Meta.InputCollection[0].GetVirtualInput();

			foreach (dynamic virtualInputColumn in virtualInput.VirtualInputColumnCollection)
			{
				string inputColumnName = virtualInputColumn.Name;
				string searchPattern = @"\b" + Regex.Escape(inputColumnName) + @"\b"; // Regex word boundary, escape special regex chars.

				if (Regex.IsMatch(friendlyExpression, searchPattern))
					columnsUsedInExpression.Add(inputColumnName);
			}

			foreach (string inputColumnName in columnsUsedInExpression)
			{
				// Creates the input column from the virtual input columns and sets the usage type to read only.
				// Skip columns that have already been set to READWRITE, since they are meant to replace existing columns.
				if (!readWriteColumns.Contains(inputColumnName))
					SetInputColumnUsageType(inputColumnName, Meta.InputCollection[0], Enum.Parse(dtsUsageTypeType, "UT_READONLY"));
			}
		}

		// Verifies that all derived columns marked as Replace have a corresponding virtual input column 
		// and creates an actual input column to the component with READWRITE since it will be modified. 
		private void PrepareReadWriteInputColumns()
		{
			foreach (string readWriteColumn in readWriteColumns)
			{
				if (!VirtualInputColumnExists(readWriteColumn, Meta.InputCollection[0]))
					throw new ArgumentException($"Derived column name {readWriteColumn} must match an existing column name if replacing.");

				SetInputColumnUsageType(readWriteColumn, Meta.InputCollection[0], Enum.Parse(dtsUsageTypeType, "UT_READWRITE"));
			}

			readWriteColumnsPrepared = true;
		}
	}
}
