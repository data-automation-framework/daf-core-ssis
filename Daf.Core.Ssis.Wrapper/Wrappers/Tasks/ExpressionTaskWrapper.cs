// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Ssis.Wrapper.Wrappers.Tasks
{
	public class ExpressionTaskWrapper : TaskWrapper
	{
		public ExpressionTaskWrapper(ContainerWrapper containerWrapper) : base(containerWrapper, "Microsoft.ExpressionTask") { }

		public string Expression
		{
			get { return TaskHost.Properties["Expression"].GetValue(TaskHost); }
			set { TaskHost.Properties["Expression"].SetValue(TaskHost, value); }
		}
	}
}
