// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers.Components;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Components
{
	internal class RowCountComponent
	{
		public static RowCountComponentWrapper CreateComponent(RowCount rowCount, DataFlowTaskWrapper dataFlowTaskWrapper)
		{
			RowCountComponentWrapper rowCountComponentWrapper = new RowCountComponentWrapper(dataFlowTaskWrapper)
			{
				Name = rowCount.Name,
				VariableName = rowCount.VariableName
			};

			dataFlowTaskWrapper.AddPath(rowCount.DataFlowInputPath.OutputPathName, rowCountComponentWrapper.Meta.InputCollection[0]);

			try
			{
				rowCountComponentWrapper.GetMetadata();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to get metadata for data flow component {rowCount.Name}!", e);
			}

			return rowCountComponentWrapper;
		}
	}
}
