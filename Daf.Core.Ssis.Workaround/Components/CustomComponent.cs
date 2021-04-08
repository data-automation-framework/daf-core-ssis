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
	internal class CustomComponent : Component
	{
		public static CustomComponentWrapper CreateComponent(IonStructure.CustomComponent customComponent, DataFlowTaskWrapper dataFlowTaskWrapper, PackageWrapper packageWrapper, ProjectWrapper projectWrapper)
		{
			CustomComponentWrapper customComponentWrapper = new CustomComponentWrapper(dataFlowTaskWrapper, customComponent.ComponentTypeName)
			{
				Name = customComponent.Name
			};

			if (customComponent.CustomProperties != null)
			{
				foreach (CustomProperty customProperty in customComponent.CustomProperties)
					customComponentWrapper.SetCustomProperty(customProperty.Name, customProperty.Value);
			}

			SetConnectionManager(projectWrapper, packageWrapper, customComponentWrapper, customComponent.ConnectionName);

			try
			{
				customComponentWrapper.GetMetadata();
			}
			catch (Exception e)
			{
				throw new Exception($"Failed to get metadata for data flow component {customComponent.Name}!", e);
			}

			return customComponentWrapper;
		}
	}
}
