// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.IO;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Tasks
{
	internal class ScriptTask : Task
	{
		/// <summary>
		/// Constructs a local (task-specific) script task.
		/// </summary>
		public ScriptTask(Script script,
			ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper, ScriptProject referencedGlobalScriptProject)
		{
			bool hasReference = script.ScriptProjectReference != null;

			ScriptTaskWrapper = new ScriptTaskWrapper(containerWrapper, hasReference ? script.ScriptProjectReference.ScriptProjectName : script.ScriptProject.Name, hasReference, ScriptTaskScope.Package)
			{
				Name = script.Name,
				DelayValidation = script.DelayValidation,
				ForceExecutionResult = script.ForceExecutionResult.ToString()
			};

			AddExpressions(ScriptTaskWrapper, script.PropertyExpressions);

			if (hasReference)
			{
				ScriptTaskWrapper.ReadOnlyVariables = GetVariableNames(referencedGlobalScriptProject.ReadOnlyVariables, ScriptTaskScope.Package, projectWrapper, packageWrapper, containerWrapper);
				ScriptTaskWrapper.ReadWriteVariables = GetVariableNames(referencedGlobalScriptProject.ReadWriteVariables, ScriptTaskScope.Package, projectWrapper, packageWrapper, containerWrapper);
			}
			else
			{
				ScriptTaskWrapper.ReadOnlyVariables = GetVariableNames(script.ScriptProject.ReadOnlyVariables, ScriptTaskScope.Package, projectWrapper, packageWrapper, containerWrapper);
				ScriptTaskWrapper.ReadWriteVariables = GetVariableNames(script.ScriptProject.ReadWriteVariables, ScriptTaskScope.Package, projectWrapper, packageWrapper, containerWrapper);

				AddAssemblyReferences(script.ScriptProject.AssemblyReferences);
				AddSourceFiles(script.ScriptProject.Files);
			}

			CheckForBuildErrors(projectWrapper.StopBuildOnScriptErrors);
			ScriptTaskWrapper.PropagateErrors(script.PropagateErrors);
		}

		/// <summary>
		/// Constructs a "global" script task.
		/// </summary>
		public ScriptTask(ScriptProject scriptProject, ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper)
		{
			ScriptTaskWrapper = new ScriptTaskWrapper(containerWrapper, scriptProject.Name, false, ScriptTaskScope.Project)
			{
				Name = scriptProject.Name
			};

			ScriptTaskWrapper.ReadOnlyVariables = GetVariableNames(scriptProject.ReadOnlyVariables, ScriptTaskScope.Project, projectWrapper, packageWrapper, containerWrapper);
			ScriptTaskWrapper.ReadWriteVariables = GetVariableNames(scriptProject.ReadWriteVariables, ScriptTaskScope.Project, projectWrapper, packageWrapper, containerWrapper);

			AddAssemblyReferences(scriptProject.AssemblyReferences);
			AddSourceFiles(scriptProject.Files);
		}

		internal ScriptTaskWrapper ScriptTaskWrapper { get; }

		private void AddAssemblyReferences(List<AssemblyReference> assemblyReferences)
		{
			if (assemblyReferences == null)
				return;

			IEnumerable<string> assemblyPaths = assemblyReferences.ConvertAll(assemblyReference => assemblyReference.AssemblyPath);
			ScriptTaskWrapper.AddReferences(assemblyPaths);
		}

		private void AddSourceFiles(List<IonStructure.File> files)
		{
			if (files == null)
				return;

			foreach (IonStructure.File file in files)
				AddSourceFile(file);

			// We need to save the project once all files have been added.
			ScriptTaskWrapper.SaveProject();
		}

		private void AddSourceFile(IonStructure.File file)
		{
			if (file.Path == "AssemblyInfo.cs")
				ScriptTaskWrapper.AddFileToProject(Path.Combine("Properties", file.Path), file.Content);
			else
				ScriptTaskWrapper.AddFileToProject(file.Path, file.Content);
		}

		private void CheckForBuildErrors(bool stopBuildOnScriptErrors)
		{
			ICollection<string> buildErrors = ScriptTaskWrapper.BuildErrors;

			if (buildErrors.Count != 0)
			{
				Console.WriteLine($"{ScriptTaskWrapper.Name} contained {buildErrors.Count} error(s):");

				foreach (string error in buildErrors)
					Console.WriteLine(error);

				if (stopBuildOnScriptErrors)
					throw new InvalidOperationException("Stopping build due to ScriptTask error(s).");
			}
		}

		private static string GetVariableNames(List<ScriptVariable> scriptVariables, ScriptTaskScope scope,
			ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ContainerWrapper containerWrapper)
		{
			if (scriptVariables == null)
				return string.Empty; // Return early if there are no variables configured for the script.

			List<string> qualifiedVariableNames = new List<string>();

			foreach (ScriptVariable scriptVariable in scriptVariables)
			{
				string qualifiedScriptVariableName = GetQualifiedVariableName(scriptVariable);

				switch (scriptVariable.Namespace)
				{
					case "$Package":
						if (scope == ScriptTaskScope.Package)
						{
							if (!packageWrapper.ContainsParameter(scriptVariable.VariableName))
								throw new InvalidOperationException($"Could find referenced package parameter {scriptVariable.VariableName}!");
						}
						break;
					case "$Project":
						if (!projectWrapper.ContainsParameter(scriptVariable.VariableName))
							throw new InvalidOperationException($"Could find referenced project parameter {scriptVariable.VariableName}!");
						break;
					default:
						if (scope == ScriptTaskScope.Package)
						{
							if (!containerWrapper.ContainsVariable(qualifiedScriptVariableName) && !packageWrapper.ContainsVariable(qualifiedScriptVariableName))
								throw new InvalidOperationException($"Couldn't find referenced variable {qualifiedScriptVariableName}!");
						}
						break;
				}

				qualifiedVariableNames.Add(qualifiedScriptVariableName);
			}

			return string.Join(",", qualifiedVariableNames);
		}

		private static string GetQualifiedVariableName(ScriptVariable scriptVariable)
		{
			if (scriptVariable.Namespace == null)
				return scriptVariable.VariableName;
			else
				return $"{scriptVariable.Namespace}::{scriptVariable.VariableName}";
		}
	}
}
