// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Components;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Components
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Can't make it static since it has a base class. TODO for later.")]
	internal class OleDbSourceComponent : Component
	{
		public static OleDbSourceComponentWrapper CreateComponent(OleDbSource oleDbSource, DataFlowTaskWrapper dataFlowTaskWrapper, PackageWrapper packageWrapper, ProjectWrapper projectWrapper)
		{
			OleDbSourceComponentWrapper oleDbSourceComponentWrapper = new OleDbSourceComponentWrapper(dataFlowTaskWrapper)
			{
				Name = oleDbSource.Name
			};

			if (oleDbSource.OleDbSourceColumns != null)
			{
				foreach (DataFlowColumnMapping dataFlowColumnMapping in oleDbSource.OleDbSourceColumns)
					oleDbSourceComponentWrapper.CustomMappingColumns.Add((dataFlowColumnMapping.SourceColumn, dataFlowColumnMapping.TargetColumn));
			}

			if (oleDbSource.SqlCommand != null)
				oleDbSourceComponentWrapper.SqlCommand = oleDbSource.SqlCommand.Value;
			else if (oleDbSource.VariableInput != null)
				oleDbSourceComponentWrapper.SqlCommandVariable = oleDbSource.VariableInput.VariableName;
			else
				throw new InvalidOperationException($"Data flow component {oleDbSourceComponentWrapper.Name} needs to have an input type set!");

			if (oleDbSource.VariableParameterMappings != null)
			{
				for (int i = 0; i < oleDbSource.VariableParameterMappings.Count; i++)
					oleDbSourceComponentWrapper.AddParameter(i, oleDbSource.VariableParameterMappings[i].ParameterName, oleDbSource.VariableParameterMappings[i].VariableName);
			}

			SetConnectionManager(projectWrapper, packageWrapper, oleDbSourceComponentWrapper, oleDbSource.ConnectionName);

			try
			{
				oleDbSourceComponentWrapper.GetMetadata();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to get metadata for data flow component {oleDbSourceComponentWrapper.Name}!", e);
			}

			return oleDbSourceComponentWrapper;
		}
	}
}
