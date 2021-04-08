// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Globalization;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Components;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Components
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Can't make it static since it has a base class. TODO for later.")]
	internal class OleDbDestinationComponent : Component
	{
		public static OleDbDestinationComponentWrapper CreateComponent(OleDbDestination oleDbDestination, DataFlowTaskWrapper dataFlowTaskWrapper, PackageWrapper packageWrapper, ProjectWrapper projectWrapper)
		{
			OleDbDestinationComponentWrapper oleDbDestinationComponentWrapper = new OleDbDestinationComponentWrapper(dataFlowTaskWrapper)
			{
				Name = oleDbDestination.Name,
				Table = oleDbDestination.ExternalTableOutput.Table,
				UseFastLoadIfAvailable = oleDbDestination.UseFastLoadIfAvailable
			};

			if (oleDbDestination.OleDbDestinationColumns != null)
			{
				foreach (DataFlowColumnMapping dataFlowColumnMapping in oleDbDestination.OleDbDestinationColumns)
					oleDbDestinationComponentWrapper.CustomMappingColumns.Add((dataFlowColumnMapping.SourceColumn, dataFlowColumnMapping.TargetColumn));
			}

			if (oleDbDestination.UseFastLoadIfAvailable)
			{
				oleDbDestinationComponentWrapper.CheckConstraints = oleDbDestination.CheckConstraints;
				oleDbDestinationComponentWrapper.KeepIdentity = oleDbDestination.KeepIdentity;
				oleDbDestinationComponentWrapper.KeepNulls = oleDbDestination.KeepNulls;
				oleDbDestinationComponentWrapper.MaximumInsertCommitSize = int.Parse(oleDbDestination.MaximumInsertCommitSize, CultureInfo.InvariantCulture);
				oleDbDestinationComponentWrapper.TableLock = oleDbDestination.TableLock;
			}

			dataFlowTaskWrapper.AddPath(oleDbDestination.DataFlowInputPath.OutputPathName, oleDbDestinationComponentWrapper.Meta.InputCollection[0]);
			SetConnectionManager(projectWrapper, packageWrapper, oleDbDestinationComponentWrapper, oleDbDestination.ConnectionName);

			if (oleDbDestination.ErrorHandling != null)
				oleDbDestinationComponentWrapper.SetRowDisposition((RowDispositionType)oleDbDestination.ErrorHandling.ErrorRowDisposition, RowFailureType.Error, PathDirectionType.Input);

			try
			{
				oleDbDestinationComponentWrapper.GetMetadata();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to get metadata for data flow component {oleDbDestination.Name}!", e);
			}

			return oleDbDestinationComponentWrapper;
		}
	}
}
