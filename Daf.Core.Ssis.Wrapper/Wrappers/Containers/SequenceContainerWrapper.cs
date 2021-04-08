// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Ssis.Wrapper.Wrappers.Containers
{
	public class SequenceContainerWrapper : ContainerWrapper
	{
		public SequenceContainerWrapper(ContainerWrapper containerWrapper) : base(containerWrapper, "STOCK:Sequence") { }
	}
}
