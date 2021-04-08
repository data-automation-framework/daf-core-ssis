// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Linq;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers.Components;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Components
{
	internal class DerivedColumnComponent
	{
		public static DerivedColumnComponentWrapper CreateComponent(DerivedColumn derivedColumn, DataFlowTaskWrapper dataFlowTaskWrapper)
		{
			List<string> duplicates = derivedColumn.DerivedColumns.GroupBy(c => c.ColumnName)
														.Where(c => c.Count() > 1)
														.Select(c => c.Key)
														.Distinct()
														.ToList();

			if (duplicates.Count > 0)
			{
				string duplicateNames = string.Join(", ", duplicates);
				throw new InvalidOperationException($"The following columns are duplicated in the derived column list: {duplicateNames}");
			}

			List<string> replaceColumns = derivedColumn.DerivedColumns.Where(c => c.Replace).Select(c => c.ColumnName).ToList();

			DerivedColumnComponentWrapper derivedColumnComponentWrapper = new DerivedColumnComponentWrapper(dataFlowTaskWrapper, replaceColumns)
			{
				Name = derivedColumn.Name
			};

			dataFlowTaskWrapper.AddPath(derivedColumn.DataFlowInputPath.OutputPathName, derivedColumnComponentWrapper.Meta.InputCollection[0]);

			try
			{
				derivedColumnComponentWrapper.GetMetadata(); // Get virtual input column metadata to the component.
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to get metadata for data flow component {derivedColumn.Name}!", e);
			}

			foreach (DerivedColumnEntry column in derivedColumn.DerivedColumns)
			{
				try
				{
					if (column.Replace)
					{
						derivedColumnComponentWrapper.ReplaceColumn(column.ColumnName, column.Expression);
						derivedColumnComponentWrapper.SetColumnDisposition(column.ColumnName, (RowDispositionType)column.ErrorRowDisposition, RowFailureType.Error, PathDirectionType.Input);
						derivedColumnComponentWrapper.SetColumnDisposition(column.ColumnName, (RowDispositionType)column.TruncationRowDisposition, RowFailureType.Truncation, PathDirectionType.Input);
					}
					else
					{
						derivedColumnComponentWrapper.AddNewColumn(column.ColumnName, column.Expression);
						derivedColumnComponentWrapper.SetColumnDisposition(column.ColumnName, (RowDispositionType)column.ErrorRowDisposition, RowFailureType.Error, PathDirectionType.Output);
						derivedColumnComponentWrapper.SetColumnDisposition(column.ColumnName, (RowDispositionType)column.TruncationRowDisposition, RowFailureType.Truncation, PathDirectionType.Output);
					}
				}
				catch (Exception e)
				{
					string msg = $"Could not set expression {column.Expression} on derived column {column.ColumnName}! Verify that it is a valid expression.";
					e.Data.Add(Constants.ExceptionComponentMessageKey, msg);
					throw;
				}
			}

			return derivedColumnComponentWrapper;
		}
	}
}
