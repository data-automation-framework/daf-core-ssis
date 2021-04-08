// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Tasks
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Can't make it static since it has a base class. TODO for later.")]
	internal class ExpressionTask : Task
	{
		public static ExpressionTaskWrapper CreateTask(Expression expression, ContainerWrapper containerWrapper)
		{
			ExpressionTaskWrapper expressionTaskWrapper = new ExpressionTaskWrapper(containerWrapper)
			{
				Name = expression.Name,
				DelayValidation = expression.DelayValidation,
				ForceExecutionResult = expression.ForceExecutionResult.ToString(),
				Expression = expression.ExpressionValue
			};

			expressionTaskWrapper.PropagateErrors(expression.PropagateErrors);
			AddExpressions(expressionTaskWrapper, expression.PropertyExpressions);

			return expressionTaskWrapper;
		}
	}
}
