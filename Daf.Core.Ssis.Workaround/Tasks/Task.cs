// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Tasks
{
	internal abstract class Task
	{
		protected static void AddExpressions(TaskWrapper taskWrapper, List<PropertyExpression> expressions)
		{
			if (expressions == null)
				return;

			foreach (PropertyExpression expression in expressions)
				taskWrapper.SetExpression(expression.PropertyName, expression.Value);
		}
	}
}
