// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers
{
	public class ContainerWrapper : ExecutableWrapper
	{
		private static readonly Type dtsExecResultType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.DTSExecResult");
		private static readonly Type dtsPrecedenceEvalOpType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.DTSPrecedenceEvalOp");

		protected ContainerWrapper(ContainerWrapper containerWrapper, string moniker) : base(containerWrapper, moniker) { }

		protected ContainerWrapper(dynamic innerObject) : base((object)innerObject) { }

		internal ICollection<ExecutableWrapper> ExecutableWrappers { get; } = new List<ExecutableWrapper>();

		public void AddPrecedenceConstraint(ExecutableWrapper source, ExecutableWrapper destination, string constraintExpression,
			string evaluationValue, string evaluationOperation, bool logicalAnd)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			if (destination == null)
				throw new ArgumentNullException(nameof(destination));

			dynamic precedenceConstraint = InnerObject.PrecedenceConstraints.Add(source.InnerObject, destination.InnerObject);
			precedenceConstraint.Expression = constraintExpression;
			precedenceConstraint.Value = GetEvaluationValueEnumValue(evaluationValue);
			precedenceConstraint.EvalOp = GetEvaluationOperationEnumValue(evaluationOperation);
			precedenceConstraint.LogicalAnd = logicalAnd;
		}

		public ExecutableWrapper FindTask(string taskName)
		{
			foreach (ExecutableWrapper executableWrapper in ExecutableWrappers)
			{
				if (executableWrapper is ContainerWrapper containerWrapper)
				{
					if (containerWrapper.Name == taskName)
						return containerWrapper;

					ExecutableWrapper foundExecutableWrapper = containerWrapper.FindTask(taskName);

					if (foundExecutableWrapper != null)
						return foundExecutableWrapper;
				}
				else if (executableWrapper is TaskWrapper taskWrapper)
				{
					if (taskWrapper.Name == taskName)
						return executableWrapper;
				}
			}

			return null;
		}

		public void SetExpression<T>(string property, T value)
		{
			InnerObject.Properties[property].SetExpression(InnerObject, value);
		}

		private static dynamic GetEvaluationOperationEnumValue(string evaluationOperationName)
		{
			return Enum.Parse(dtsPrecedenceEvalOpType, evaluationOperationName);
		}

		private static dynamic GetEvaluationValueEnumValue(string evaluationValue)
		{
			return Enum.Parse(dtsExecResultType, evaluationValue);
		}
	}
}
