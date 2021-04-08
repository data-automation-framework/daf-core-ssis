// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Ssis.Wrapper.Wrappers
{
	public class VariableWrapper
	{
		public VariableWrapper(PackageWrapper packageWrapper, string nameSpace, string name, bool variableType, object value)
		{
			if (packageWrapper == null)
				throw new ArgumentNullException(nameof(packageWrapper));

			Variable = packageWrapper.Package.Variables.Add(name, variableType, nameSpace, value);
		}

		public bool EvaluateAsExpression { get { return Variable.EvaluateAsExpression; } set { Variable.EvaluateAsExpression = value; } }

		public string Expression { get { return Variable.Expression; } set { Variable.Expression = value; } }

		private dynamic Variable { get; }
	}
}
