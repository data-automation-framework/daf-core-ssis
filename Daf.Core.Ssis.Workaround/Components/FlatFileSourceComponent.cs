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
	internal class FlatFileSourceComponent : Component
	{
		public static FlatFileSourceComponentWrapper CreateComponent(FlatFileSource flatFileSource, DataFlowTaskWrapper dataFlowTaskWrapper, PackageWrapper packageWrapper, ProjectWrapper projectWrapper)
		{
			FlatFileSourceComponentWrapper flatFileSourceComponentWrapper = new FlatFileSourceComponentWrapper(dataFlowTaskWrapper)
			{
				Name = flatFileSource.Name,
				RetainNulls = flatFileSource.RetainNulls
			};

			if (flatFileSource.FlatFileSourceColumns != null)
			{
				foreach (DataFlowColumnMapping dataFlowColumnMapping in flatFileSource.FlatFileSourceColumns)
					flatFileSourceComponentWrapper.CustomMappingColumns.Add((dataFlowColumnMapping.SourceColumn, dataFlowColumnMapping.TargetColumn));
			}

			SetConnectionManager(projectWrapper, packageWrapper, flatFileSourceComponentWrapper, flatFileSource.ConnectionName);

			try
			{
				flatFileSourceComponentWrapper.GetMetadata();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to get metadata for data flow component {flatFileSourceComponentWrapper.Name}!", e);
			}

			return flatFileSourceComponentWrapper;
		}
	}
}
