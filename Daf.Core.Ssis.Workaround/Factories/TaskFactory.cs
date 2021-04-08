// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Tasks;
using Daf.Core.Ssis.Wrapper.Wrappers;

namespace Daf.Core.Ssis.Factories
{
	internal static class TaskFactory
	{
		public static void CreateTask(ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper,
			IonStructure.Task task, ScriptProject referencedGlobalScriptProject = null)
		{
			try
			{
				ExecutableWrapper executableWrapper = null;

				switch (task)
				{
					case DataFlow dataFlow:
						DataFlowTask dataFlowTask = new DataFlowTask(dataFlow, projectWrapper, packageWrapper, containerWrapper);
						executableWrapper = dataFlowTask.DataFlowTaskWrapper;
						break;
					case ExecuteSql executeSql:
						ExecuteSqlTask executeSqlTask = new ExecuteSqlTask(executeSql, containerWrapper);
						executableWrapper = executeSqlTask.ExecuteSqlTaskWrapper;
						break;
					case ExecuteProcess executeProcess:
						executableWrapper = ExecuteProcessTask.CreateTask(executeProcess, containerWrapper);
						break;
					case Expression expression:
						executableWrapper = ExpressionTask.CreateTask(expression, containerWrapper);
						break;
					case Script script:
						ScriptTask scriptTask = new ScriptTask(script, projectWrapper, packageWrapper, containerWrapper, referencedGlobalScriptProject);
						executableWrapper = scriptTask.ScriptTaskWrapper;
						break;
					case ForLoopContainer forLoopContainer:
						executableWrapper = ContainerExecutable.CreateForLoopContainer(forLoopContainer, projectWrapper, packageWrapper, containerWrapper);
						break;
					case ForEachFromVariableLoopContainer forEachFromVarLoopContainer:
						executableWrapper = ContainerExecutable.CreateForEachFromVariableLoopContainer(forEachFromVarLoopContainer, projectWrapper, packageWrapper, containerWrapper);
						break;
					case SequenceContainer sequenceContainer:
						executableWrapper = ContainerExecutable.CreateSequenceContainer(sequenceContainer, projectWrapper, packageWrapper, containerWrapper);
						break;
				}

				AddPrecedenceConstraints(containerWrapper, executableWrapper, task.PrecedenceConstraints);
			}
			catch (Exception e)
			{
				if (e.Data[Constants.ExceptionTaskKey] == null)
					e.Data[Constants.ExceptionTaskKey] = task.Name;
				else
					e.Data[Constants.ExceptionTaskKey] = task.Name + "/" + e.Data[Constants.ExceptionTaskKey]; // If task is not null then the task being created is a container, and an exception has been thrown in one of its underlying tasks.
				throw;
			}
		}

		private static void AddPrecedenceConstraints(ContainerWrapper containerWrapper, ExecutableWrapper destinationTask, PrecedenceConstraintList constraints)
		{
			if (constraints == null)
				return;

			foreach (InputPath input in constraints.Inputs)
			{
				ExecutableWrapper sourceTask = containerWrapper.FindTask(input.OutputPathName);

				if (sourceTask == null)
					throw new InvalidOperationException($"Failed to find task {input.OutputPathName} in container {containerWrapper.Name} (referenced by {destinationTask.Name})!");

				string evaluationValue = input.EvaluationValue.ToString();
				string evaluationOperation = input.EvaluationOperation.ToString();
				bool logicalAnd = constraints.LogicalType == LogicalOperationEnum.And;

				containerWrapper.AddPrecedenceConstraint(sourceTask, destinationTask, input.Expression, evaluationValue, evaluationOperation, logicalAnd);
			}
		}
	}
}
