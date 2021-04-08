// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public class CustomComponentWrapper : ComponentWrapper
	{
		public CustomComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper, string componentTypeName) : base(dataFlowTaskWrapper, componentTypeName) { }

		// SetCustomProperty needs to be public for CustomComponentWrapper, since we don't know which properties to handle ahead of time.
		public new void SetCustomProperty<T>(string property, T value)
		{
			base.SetCustomProperty(property, value);
		}
	}
}
