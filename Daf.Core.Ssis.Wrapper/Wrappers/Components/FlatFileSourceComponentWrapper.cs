// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public class FlatFileSourceComponentWrapper : ComponentWrapper
	{
		public FlatFileSourceComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper) : base(dataFlowTaskWrapper, "Microsoft.FlatFileSource") { }

		public bool RetainNulls { set { SetCustomProperty(nameof(RetainNulls), value); } }
	}
}
