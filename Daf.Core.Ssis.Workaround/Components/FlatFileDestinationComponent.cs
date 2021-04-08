// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Components;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Components
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Can't make it static since it has a base class. TODO for later.")]
	internal class FlatFileDestinationComponent : Component
	{
		public static FlatFileDestinationComponentWrapper CreateComponent(FlatFileDestination flatFileDestination, DataFlowTaskWrapper dataFlowTaskWrapper, PackageWrapper packageWrapper, ProjectWrapper projectWrapper)
		{
			FlatFileDestinationComponentWrapper flatFileDestinationComponentWrapper = new FlatFileDestinationComponentWrapper(dataFlowTaskWrapper)
			{
				Name = flatFileDestination.Name,
				Overwrite = flatFileDestination.Overwrite
			};

			if (flatFileDestination.FlatFileDestinationColumns != null)
			{
				foreach (DataFlowColumnMapping dataFlowColumnMapping in flatFileDestination.FlatFileDestinationColumns)
					flatFileDestinationComponentWrapper.CustomMappingColumns.Add((dataFlowColumnMapping.SourceColumn, dataFlowColumnMapping.TargetColumn));
			}

			if (flatFileDestination.Header != null)
				flatFileDestinationComponentWrapper.Header = flatFileDestination.Header;

			dataFlowTaskWrapper.AddPath(flatFileDestination.DataFlowInputPath.OutputPathName, flatFileDestinationComponentWrapper.Meta.InputCollection[0]);
			SetConnectionManager(projectWrapper, packageWrapper, flatFileDestinationComponentWrapper, flatFileDestination.ConnectionName);

			if (flatFileDestination.ErrorHandling != null)
				flatFileDestinationComponentWrapper.SetRowDisposition((RowDispositionType)flatFileDestination.ErrorHandling.ErrorRowDisposition, RowFailureType.Error, PathDirectionType.Input);

			try
			{
				flatFileDestinationComponentWrapper.GetMetadata();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to get metadata for data flow component {flatFileDestination.Name}!", e);
			}

			return flatFileDestinationComponentWrapper;
		}
	}
}
