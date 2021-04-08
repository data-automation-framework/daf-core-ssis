// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Ssis.Components;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Tasks
{
	internal class DataFlowTask
	{
		public DataFlowTask(DataFlow dataFlow, ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper)
		{
			DataFlowTaskWrapper = new DataFlowTaskWrapper(containerWrapper)
			{
				Name = dataFlow.Name,
				DelayValidation = dataFlow.DelayValidation,
				ForceExecutionResult = dataFlow.ForceExecutionResult.ToString(),
				AutoAdjustBufferSize = dataFlow.AutoAdjustBufferSize,
				DefaultBufferMaxRows = int.Parse(dataFlow.DefaultBufferMaxRows, CultureInfo.InvariantCulture),
				DefaultBufferSize = int.Parse(dataFlow.DefaultBufferSize, CultureInfo.InvariantCulture)
			};

			DataFlowTaskWrapper.PropagateErrors(dataFlow.PropagateErrors);
			AddComponents(dataFlow.Components, packageWrapper, projectWrapper);
		}

		internal DataFlowTaskWrapper DataFlowTaskWrapper { get; }

		private void AddComponents(List<IonStructure.Component> components, PackageWrapper packageWrapper, ProjectWrapper projectWrapper)
		{
			if (components == null)
				return;

			foreach (IonStructure.Component component in components)
			{
				try
				{
					switch (component)
					{
						case IonStructure.CustomComponent customComponent:
							Components.CustomComponent.CreateComponent(customComponent, DataFlowTaskWrapper, packageWrapper, projectWrapper);
							break;
						case DerivedColumn derivedComponent:
							DerivedColumnComponent.CreateComponent(derivedComponent, DataFlowTaskWrapper);
							break;
						case FlatFileSource flatFileSource:
							FlatFileSourceComponent.CreateComponent(flatFileSource, DataFlowTaskWrapper, packageWrapper, projectWrapper);
							break;
						case FlatFileDestination flatFileDestination:
							FlatFileDestinationComponent.CreateComponent(flatFileDestination, DataFlowTaskWrapper, packageWrapper, projectWrapper);
							break;
						case OleDbSource oleDbSource:
							OleDbSourceComponent.CreateComponent(oleDbSource, DataFlowTaskWrapper, packageWrapper, projectWrapper);
							break;
						case OleDbDestination oleDbDestination:
							OleDbDestinationComponent.CreateComponent(oleDbDestination, DataFlowTaskWrapper, packageWrapper, projectWrapper);
							break;
						case RowCount rowCount:
							RowCountComponent.CreateComponent(rowCount, DataFlowTaskWrapper);
							break;
					}
				}
				catch (Exception e)
				{
					e.Data.Add(Constants.ExceptionComponentKey, component.Name);
					throw;
				}
			}

			DataFlowTaskWrapper.ReinitializeMetaData();
		}
	}
}
