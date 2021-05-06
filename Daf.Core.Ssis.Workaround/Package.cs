// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Ssis.Factories;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;

namespace Daf.Core.Ssis
{
	internal class Package
	{
		public Package(IonStructure.Package package, ProjectWrapper projectWrapper, List<ScriptProject> globalScriptProjects)
		{
			PackageWrapper = new PackageWrapper()
			{
				Name = package.Name,
				DelayValidation = package.DelayValidation,
				LocaleId = package.LocaleId,
			};

			SetParameters(package.Parameters);
			SetVariables(package.Variables);
			ConnectionManagerFactory.CreateConnectionManagers(PackageWrapper, package.Connections);
			CreateTasks(projectWrapper, package.Tasks, globalScriptProjects);
			SetPackageLocaleId(projectWrapper, package);
		}

		internal PackageWrapper PackageWrapper { get; }

		private void CreateTasks(ProjectWrapper projectWrapper, List<Task> tasks, List<ScriptProject> globalScriptProjects)
		{
			if (tasks == null)
				return;

			foreach (Task task in tasks)
			{
				if (task is Script script && script.ScriptProjectReference != null)
				{
					foreach (ScriptProject globalScriptProject in globalScriptProjects)
					{
						// Re-use the global script project if there is a match.
						if (script.ScriptProjectReference.ScriptProjectName == globalScriptProject.Name)
						{
							TaskFactory.CreateTask(projectWrapper, PackageWrapper, PackageWrapper, task, globalScriptProjects);
							break;
						}
					}
				}
				else
					TaskFactory.CreateTask(projectWrapper, PackageWrapper, PackageWrapper, task, globalScriptProjects);
			}
		}

		private void SetPackageLocaleId(ProjectWrapper projectWrapper, IonStructure.Package package)
		{
			if (package.LocaleId != 0)
				PackageWrapper.LocaleId = package.LocaleId; // If the package has a non-default LocaleId, use that.
			else if (projectWrapper.LocaleId != 0)
				PackageWrapper.LocaleId = projectWrapper.LocaleId; // Otherwise, use the project's LocalId if it isn't default.
			else
				PackageWrapper.LocaleId = CultureInfo.CurrentCulture.LCID; // And finally, if none of the above applies, use the user's locale.
		}

		private void SetParameters(List<Parameter> parameters)
		{
			if (parameters == null)
				return;

			foreach (Parameter parameter in parameters)
				SetParameter(parameter);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "We don't have a reason to save the results yet.")]
		private void SetParameter(Parameter parameter)
		{
			object addedParameter = PackageWrapper.AddParameter(parameter.Name, parameter.DataType);

			new ParameterWrapper(addedParameter)
			{
				IsRequired = parameter.IsRequired,
				Value = Utility.ParseValue(parameter.Value, parameter.DataType)
			};
		}

		private void SetVariables(List<Variable> variables)
		{
			if (variables == null)
				return;

			foreach (Variable variable in variables)
				SetVariable(variable);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "We don't have a reason to save the results yet.")]
		private void SetVariable(Variable variable)
		{
			if (variable.EvaluateAsExpression)
			{
				new VariableWrapper(PackageWrapper, variable.Namespace, variable.Name, false, string.Empty)
				{
					EvaluateAsExpression = true,
					Expression = variable.Value ?? string.Empty // Expressions aren't valid if null, set to empty string.
				};
			}
			else
			{
				new VariableWrapper
					(
						PackageWrapper,
						variable.Namespace,
						variable.Name,
						variable.ReadOnly,
						Utility.ParseValue(variable.Value, variable.DataType)
					);
			}
		}
	}
}
