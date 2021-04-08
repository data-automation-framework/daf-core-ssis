// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public class FlatFileDestinationComponentWrapper : ComponentWrapper
	{
		public FlatFileDestinationComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper) : base(dataFlowTaskWrapper, "Microsoft.FlatFileDestination") { }

		public string Header { set { SetCustomProperty(nameof(Header), value); } }

		public bool Overwrite { set { SetCustomProperty(nameof(Overwrite), value); } }
	}
}
