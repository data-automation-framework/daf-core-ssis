// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Containers
{
	public class ForLoopContainerWrapper : ContainerWrapper
	{
		private static readonly Type forLoopType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.ForLoop");

		public ForLoopContainerWrapper(ContainerWrapper containerWrapper) : base(containerWrapper, "STOCK:ForLoop")
		{
			ForLoopContainer = Convert.ChangeType(InnerObject, forLoopType);
		}

		public string AssignExpression { set { ForLoopContainer.AssignExpression = value; } }

		public string EvalExpression { set { ForLoopContainer.EvalExpression = value; } }

		public string InitExpression { set { ForLoopContainer.InitExpression = value; } }

		private dynamic ForLoopContainer { get; }
	}
}
