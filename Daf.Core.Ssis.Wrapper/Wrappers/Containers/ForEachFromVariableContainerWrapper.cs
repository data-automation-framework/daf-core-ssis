// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.Wrapper.Enums;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Containers
{
	public class ForEachFromVariableLoopContainerWrapper : ForEachLoopContainerWrapper
	{
		public ForEachFromVariableLoopContainerWrapper(ContainerWrapper containerWrapper) : base(containerWrapper, ForEachEnumeratorHost.Variable, true, "STOCK:ForEachLoop") { }

		public string CollectionVariableName { set { ForEachLoopContainer.ForEachEnumerator.InnerObject.VariableName = value; } }

		public void AddVariableMapping(int valueIndex, string variableName)
		{
			dynamic mapping = ForEachLoopContainer.VariableMappings.Add();
			mapping.ValueIndex = valueIndex;
			mapping.VariableName = variableName;
		}
	}
}
