// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;

namespace Daf.Core.Ssis.Tasks
{
	internal static partial class TaskFactory
	{
		public static void ConstructTask(ProjectWrapper project, PackageWrapper package, ContainerWrapper taskContainer, Task task, ScriptProject innerTask = null)
		{
			try
			{
				ExecutableWrapper executableTask = null;

				switch (task)
				{
					case DataFlow dataflowTask:
						executableTask = DataFlowTask.ConstructTask(dataflowTask, project, package, taskContainer);
						break;
					case ExecuteSql executeSqlTask:
						executableTask = ConstructExecuteSqlTask(executeSqlTask, taskContainer);
						break;
					case ExecuteProcess executeProcessTask:
						executableTask = ConstructExecuteProcessTask(executeProcessTask, taskContainer);
						break;
					case Expression expressionTask:
						executableTask = ConstructExpressionTask(expressionTask, taskContainer);
						break;
					case Script scriptTask:
						executableTask = ConstructScriptTask(scriptTask, project, package, taskContainer, innerTask);
						break;
					case ForLoopContainer forLoopContainer:
						executableTask = ContainerExecutable.ConstructForLoopContainer(forLoopContainer, project, package, taskContainer);
						break;
					case ForEachFromVariableLoopContainer forEachFromVarLoopContainer:
						executableTask = ContainerExecutable.ConstructForEachFromVariableLoopContainer(forEachFromVarLoopContainer, project, package, taskContainer);
						break;
					case SequenceContainer sequenceContainer:
						executableTask = ContainerExecutable.ConstructSequenceContainer(sequenceContainer, project, package, taskContainer);
						break;
				}

				AddPrecedenceConstraints(taskContainer, executableTask, task.PrecedenceConstraints);
			}
			catch (Exception e)
			{
				if (e.Data[Constants.ExceptionTaskKey] == null)
					e.Data[Constants.ExceptionTaskKey] = task.Name;
				else
					e.Data[Constants.ExceptionTaskKey] = task.Name + "/" + e.Data[Constants.ExceptionTaskKey]; // If task is not null then the task being constructed is a container and an exception has been thrown in one of the underlying tasks.
				throw;
			}
		}

		private static void AddExpressions(TaskWrapper task, PropertyExpression[] expressions)
		{
			if (expressions != null)
			{
				foreach (PropertyExpression expression in expressions)
					task.SetExpression(expression.PropertyName, expression.Value);
			}
		}

		private static void AddPrecedenceConstraints(ContainerWrapper taskContainer, ExecutableWrapper task, PrecedenceConstraintList constraints)
		{
			if (constraints != null)
			{
				foreach (InputPath input in constraints.Inputs)
				{
					ExecutableWrapper sourceTask = taskContainer.FindTask(input.OutputPathName);

					if (sourceTask == null)
						throw new Exception($"Failed to find task {input.OutputPathName}, referenced by {task.Name} in container {taskContainer.Name}!");

					string evaluationValue = input.EvaluationValue.ToString();
					string evaluationOperation = input.EvaluationOperation.ToString();
					bool logicalAnd = constraints.LogicalType == LogicalOperationEnum.And;

					taskContainer.AddPrecedenceConstraint(sourceTask, task, input.Expression, evaluationValue, evaluationOperation, logicalAnd);
				}
			}
		}
	}
}
