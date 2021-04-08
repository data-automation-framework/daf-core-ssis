// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using Daf.Core.Ssis.Wrapper.Wrappers.Components;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Tasks
{
	public class DataFlowTaskWrapper : TaskWrapper
	{
		public DataFlowTaskWrapper(ContainerWrapper containerWrapper) : base(containerWrapper, "STOCK:PipelineTask") { }

		public bool AutoAdjustBufferSize
		{
			get { return TaskHost.Properties["AutoAdjustBufferSize"].GetValue(TaskHost); }
			set { TaskHost.Properties["AutoAdjustBufferSize"].SetValue(TaskHost, value); }
		}

		public int DefaultBufferMaxRows
		{
			get { return TaskHost.Properties["DefaultBufferMaxRows"].GetValue(TaskHost); }
			set { TaskHost.Properties["DefaultBufferMaxRows"].SetValue(TaskHost, value); }
		}

		public int DefaultBufferSize
		{
			get { return TaskHost.Properties["DefaultBufferSize"].GetValue(TaskHost); }
			set { TaskHost.Properties["DefaultBufferSize"].SetValue(TaskHost, value); }
		}

		internal dynamic ComponentMetaDataCollection { get { return TaskHost.Properties["ComponentMetaDataCollection"].GetValue(TaskHost); } }

		private ICollection<ComponentWrapper> Components { get; } = new List<ComponentWrapper>();

		private dynamic PathCollection { get { return TaskHost.Properties["PathCollection"].GetValue(TaskHost); } }

		public void AddPath(string outputPath, dynamic destination)
		{
			if (outputPath == null)
				throw new ArgumentNullException(nameof(outputPath));

			dynamic source = GetOutputPath(outputPath);
			dynamic path = PathCollection.New();

			path.AttachPathAndPropagateNotifications(source, destination);
		}

		public void ReinitializeMetaData()
		{
			foreach (ComponentWrapper component in Components)
			{
				// If the component's InputCollection is empty, its metadata hasn't been initialized yet.
				if (component.Meta.InputCollection.Count == 0)
					ReinitializeComponentMetaData(component);
			}

			void ReinitializeComponentMetaData(ComponentWrapper component)
			{
				component.GetMetadata();

				// Ensure that any components connected to the one being reinitialized are also reinitialized.
				foreach (dynamic path in PathCollection)
				{
					if (path.StartPoint.Component.ID != component.Meta.ID)
						continue;

					foreach (ComponentWrapper otherComponent in Components)
					{
						if (otherComponent.Meta.ID == path.EndPoint.Component.ID)
							ReinitializeComponentMetaData(otherComponent);
					}
				}
			}
		}

		private dynamic GetOutputPath(string outputPath)
		{
			string[] splitPath = outputPath.Split('.');

			foreach (dynamic component in ComponentMetaDataCollection)
			{
				// TODO: Handle multiple paths, error paths etc.
				if (splitPath[0] == component.Name)
					return component.OutputCollection[0];
			}

			throw new InvalidOperationException($"Failed to find output path: {outputPath}");
		}
	}
}
