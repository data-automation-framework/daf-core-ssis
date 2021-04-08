// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public class RowCountComponentWrapper : ComponentWrapper
	{
		public RowCountComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper) : base(dataFlowTaskWrapper, "Microsoft.RowCount") { }

		public string VariableName
		{
			get { return GetCustomProperty<string>(nameof(VariableName)); }
			set { SetCustomProperty(nameof(VariableName), value); }
		}
	}
}
