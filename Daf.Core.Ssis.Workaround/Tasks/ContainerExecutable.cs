// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Ssis.Factories;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Containers;

namespace Daf.Core.Ssis.Tasks
{
	internal class ContainerExecutable
	{
		public static SequenceContainerWrapper CreateSequenceContainer(SequenceContainer sequenceContainer,
			ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper, List<ScriptProject> globalScriptProjects)
		{
			SequenceContainerWrapper sequenceContainerWrapper = new SequenceContainerWrapper(containerWrapper)
			{
				Name = sequenceContainer.Name,
				DelayValidation = sequenceContainer.DelayValidation,
				ForceExecutionResult = sequenceContainer.ForceExecutionResult.ToString()
			};

			AddExpressions(sequenceContainerWrapper, sequenceContainer.PropertyExpressions);
			CreateTasks(projectWrapper, packageWrapper, sequenceContainerWrapper, sequenceContainer.Tasks, globalScriptProjects);

			return sequenceContainerWrapper;
		}

		public static ForLoopContainerWrapper CreateForLoopContainer(ForLoopContainer forLoopContainer,
			ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper, List<ScriptProject> globalScriptProjects)
		{
			ForLoopContainerWrapper forLoopContainerWrapper = new ForLoopContainerWrapper(containerWrapper)
			{
				Name = forLoopContainer.Name,
				DelayValidation = forLoopContainer.DelayValidation,
				ForceExecutionResult = forLoopContainer.ForceExecutionResult.ToString(),
				InitExpression = forLoopContainer.InitExpression,
				EvalExpression = forLoopContainer.EvalExpression,
				AssignExpression = forLoopContainer.AssignExpression
			};

			AddExpressions(forLoopContainerWrapper, forLoopContainer.PropertyExpressions);
			CreateTasks(projectWrapper, packageWrapper, forLoopContainerWrapper, forLoopContainer.Tasks, globalScriptProjects);

			return forLoopContainerWrapper;
		}

		public static ForEachFromVariableLoopContainerWrapper CreateForEachFromVariableLoopContainer(ForEachFromVariableLoopContainer forEachFromVarLoopContainer,
			ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper, List<ScriptProject> globalScriptProjects)
		{
			ForEachFromVariableLoopContainerWrapper fefvlContainerWrapper = new ForEachFromVariableLoopContainerWrapper(containerWrapper)
			{
				Name = forEachFromVarLoopContainer.Name,
				DelayValidation = forEachFromVarLoopContainer.DelayValidation,
				ForceExecutionResult = forEachFromVarLoopContainer.ForceExecutionResult.ToString(),
				CollectionVariableName = forEachFromVarLoopContainer.CollectionVariableName
			};

			foreach (VariableIndexMapping variableIndexMapping in forEachFromVarLoopContainer.ForEachFromVariableMappings)
				fefvlContainerWrapper.AddVariableMapping(variableIndexMapping.Index, variableIndexMapping.VariableName);

			AddExpressions(fefvlContainerWrapper, forEachFromVarLoopContainer.PropertyExpressions);
			CreateTasks(projectWrapper, packageWrapper, fefvlContainerWrapper, forEachFromVarLoopContainer.Tasks, globalScriptProjects);

			return fefvlContainerWrapper;
		}

		private static void AddExpressions(ContainerWrapper containerWrapper, List<PropertyExpression> expressions)
		{
			if (expressions == null)
				return;

			foreach (PropertyExpression expression in expressions)
				containerWrapper.SetExpression(expression.PropertyName, expression.Value);
		}

		private static void CreateTasks(ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper, List<IonStructure.Task> tasks, List<ScriptProject> globalScriptProjects)
		{
			if (tasks == null)
				return;

			foreach (IonStructure.Task task in tasks)
			{
				if (task is Script scriptTask && scriptTask.ScriptProjectReference != null)
				{
					foreach (ScriptProject scriptProject in globalScriptProjects)
					{
						if (scriptTask.ScriptProjectReference.ScriptProjectName == scriptProject.Name)
							TaskFactory.CreateTask(projectWrapper, packageWrapper, containerWrapper, task, globalScriptProjects, scriptProject);
					}
				}
				else
					TaskFactory.CreateTask(projectWrapper, packageWrapper, containerWrapper, task, globalScriptProjects);
			}
		}
	}
}
