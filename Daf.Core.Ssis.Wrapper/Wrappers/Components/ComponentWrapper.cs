// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public partial class ComponentWrapper
	{
		private static readonly Type componentWrapperType = AssemblyLoader.DtsPipelineWrap.GetType("Microsoft.SqlServer.Dts.Pipeline.Wrapper.CManagedComponentWrapperClass");
		private static readonly Type dtsConvertType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.DtsConvert");
		private static readonly Type dtsRowDispositionType = AssemblyLoader.DtsPipelineWrap.GetType("Microsoft.SqlServer.Dts.Pipeline.Wrapper.DTSRowDisposition");
		private static readonly Type metadataType = AssemblyLoader.DtsPipelineWrap.GetType("Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100");
		private static readonly Type virtualInputType = AssemblyLoader.DtsPipelineWrap.GetType("Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInput100");

		private static readonly dynamic FailComponent = Enum.Parse(dtsRowDispositionType, "RD_FailComponent");
		private static readonly dynamic IgnoreFailure = Enum.Parse(dtsRowDispositionType, "RD_IgnoreFailure");
		private static readonly dynamic RedirectRow = Enum.Parse(dtsRowDispositionType, "RD_RedirectRow");
		private static readonly dynamic NotUsed = Enum.Parse(dtsRowDispositionType, "RD_NotUsed");

		protected static readonly Type dtsUsageTypeType = AssemblyLoader.DtsPipelineWrap.GetType("Microsoft.SqlServer.Dts.Pipeline.Wrapper.DTSUsageType");

		protected ComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper, string componentClassId)
		{
			if (dataFlowTaskWrapper == null)
				throw new ArgumentNullException(nameof(dataFlowTaskWrapper));

			Meta = dataFlowTaskWrapper.ComponentMetaDataCollection.New();
			Meta.ComponentClassId = componentClassId;
			Instance = metadataType.GetMethod("Instantiate").Invoke(Meta, null);

			try
			{
				componentWrapperType.GetMethod("ProvideComponentProperties").Invoke(Instance, null);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to find an implementation of dataflow component {componentClassId}", e);
			}
		}

		public ICollection<(string SourceColumn, string TargetColumn)> CustomMappingColumns { get; } = new List<(string SourceColumn, string TargetColumn)>();

		public dynamic Meta { get; }

		public string Name { get { return Meta.Name; } set { Meta.Name = value; } }

		private dynamic Instance { get; }

		public void GetMetadata()
		{
			Connect();

			// Nothing else to do if we don't have an input collection.
			if (Meta.InputCollection.Count == 0)
				return;

			dynamic inputCollection = Meta.InputCollection[0];
			dynamic externalMetadataColumnCollection = inputCollection.ExternalMetadataColumnCollection;
			dynamic virtualInputCollection = inputCollection.GetVirtualInput();

			foreach (dynamic virtualInputColumn in virtualInputCollection.VirtualInputColumnCollection)
			{
				dynamic columnName = virtualInputColumn.Name;

				if (ExternalMetadataColumnExists(columnName, externalMetadataColumnCollection) && !CustomTargetMappingExists(columnName))
					LinkVirtualInputToOutput(virtualInputCollection.VirtualInputColumnCollection[columnName], virtualInputCollection, columnName, inputCollection);
			}

			// Refresh virtual input.
			virtualInputCollection = inputCollection.GetVirtualInput();

			// Handle custom mappings, if there are any.
			if (CustomMappingColumns.Count != 0)
			{
				foreach ((string SourceColumn, string TargetColumn) in CustomMappingColumns)
				{
					dynamic virtualInputColumn = GetVirtualInputColumnByName(SourceColumn, virtualInputCollection.VirtualInputColumnCollection);
					dynamic externalMetadataColumn = GetExternalMetadataColumnByName(TargetColumn, externalMetadataColumnCollection);

					if (virtualInputColumn == null || externalMetadataColumn == null || string.IsNullOrEmpty(TargetColumn))
						continue;

					dynamic inputColumn = GetInputColumn(virtualInputColumn.Name, virtualInputColumn.LineageID, inputCollection.InputColumnCollection);

					MapInputColumn(inputColumn, externalMetadataColumn, inputCollection);
				}
			}

			// Handle implicit column mappings (excluding columns that have been custom-mapped).
			foreach (dynamic virtualInputColumn in virtualInputCollection.VirtualInputColumnCollection)
			{
				if (ExternalMetadataColumnExists(virtualInputColumn.Name, externalMetadataColumnCollection) && !CustomSourceMappingExists(virtualInputColumn.Name) && !CustomTargetMappingExists(virtualInputColumn.Name))
				{
					dynamic inputColumn = GetInputColumn(virtualInputColumn.Name, virtualInputColumn.LineageID, inputCollection.InputColumnCollection);
					dynamic externalMetadataColumn = GetExternalMetadataColumnByName(virtualInputColumn.Name, externalMetadataColumnCollection);

					MapInputColumn(inputColumn, externalMetadataColumn, inputCollection);
				}
			}
		}

		public void SetConnectionManager(ConnectionManagerWrapper connectionManagerWrapper)
		{
			if (connectionManagerWrapper == null)
				throw new ArgumentNullException(nameof(connectionManagerWrapper));

			Type[] types = new Type[] { connectionManagerWrapper.ConnectionManager.GetType() };
			object[] parameters = new object[] { connectionManagerWrapper.ConnectionManager };

			dynamic connectionManager = dtsConvertType.GetMethod("GetExtendedInterface", types).Invoke(null, parameters);

			Meta.RuntimeConnectionCollection[0].ConnectionManager = connectionManager;
			Meta.RuntimeConnectionCollection[0].ConnectionManagerID = connectionManagerWrapper.ConnectionManager.ID;
		}

		// Set error row disposition on row level for components that supports it e.g. OLEDB destination
		public void SetRowDisposition(RowDispositionType rowDispositionType, RowFailureType rowFailureType, PathDirectionType direction)
		{
			dynamic collection;
			if (direction == PathDirectionType.Input)
				collection = Meta.InputCollection[0];
			else
				collection = Meta.OutputCollection[0];

			SetDisposition(collection, rowDispositionType, rowFailureType);
		}

		private void AcquireConnections()
		{
			try
			{
				Type[] types = { typeof(object) };
				object[] parameters = { null };

				componentWrapperType.GetMethod("AcquireConnections", types).Invoke(Instance, parameters);
			}
			catch (TargetInvocationException e)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"An exception was thrown in dataflow component {Meta.Name} while attempting to acquire a connection!");
				stringBuilder.AppendLine("This could indicate a connectivity error or a misconfigured connection.");
				stringBuilder.Append($"The inner exception message might contain useful information: {e.InnerException.Message}");

				throw new InvalidOperationException(stringBuilder.ToString(), e);
			}
		}

		/// <summary>
		/// Connects, gets initial metadata and finally releases the connection.
		/// </summary>
		private void Connect()
		{
			AcquireConnections();
			ReinitializeMetadata();
			componentWrapperType.GetMethod("ReleaseConnections").Invoke(Instance, null);
		}

		private void ReinitializeMetadata()
		{
			try
			{
				componentWrapperType.GetMethod("ReinitializeMetaData").Invoke(Instance, null);
			}
			catch (TargetInvocationException e)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"An exception was thrown in dataflow component {Meta.Name} while attempting to reinitialize metadata!");
				stringBuilder.Append("This could indicate a connectivity error or a misconfigured component. Ensure that the referenced database object exists in the target database.");

				throw new InvalidOperationException(stringBuilder.ToString(), e);
			}
		}

		private static void SetDisposition(dynamic obj, RowDispositionType rowDispositionType, RowFailureType rowFailureType)
		{
			switch (rowDispositionType)
			{
				case RowDispositionType.FailComponent:
					if (rowFailureType == RowFailureType.Error)
						obj.ErrorRowDisposition = FailComponent;
					else if (rowFailureType == RowFailureType.Truncation)
						obj.TruncationRowDisposition = FailComponent;
					break;
				case RowDispositionType.IgnoreFailure:
					if (rowFailureType == RowFailureType.Error)
						obj.ErrorRowDisposition = IgnoreFailure;
					else if (rowFailureType == RowFailureType.Truncation)
						obj.TruncationRowDisposition = IgnoreFailure;
					break;
				case RowDispositionType.RedirectRow:
					if (rowFailureType == RowFailureType.Error)
						obj.ErrorRowDisposition = RedirectRow;
					else if (rowFailureType == RowFailureType.Truncation)
						obj.TruncationRowDisposition = RedirectRow;
					break;
				case RowDispositionType.NotUsed:
					if (rowFailureType == RowFailureType.Error)
						obj.ErrorRowDisposition = NotUsed;
					else if (rowFailureType == RowFailureType.Truncation)
						obj.TruncationRowDisposition = NotUsed;
					break;
			}
		}
	}
}
