// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using Daf.Core.Ssis.Wrapper.Enums;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Tasks
{
	public class ScriptTaskWrapper : TaskWrapper
	{
		private static readonly Type scriptTaskType = AssemblyLoader.ScriptTask.GetType("Microsoft.SqlServer.Dts.Tasks.ScriptTask.ScriptTask");
		private static readonly Type vstaScriptLanguagesType = AssemblyLoader.VstaScriptingLib.GetType("Microsoft.SqlServer.VSTAHosting.VSTAScriptLanguages");
		private static readonly Type vstaScriptFileType = AssemblyLoader.VstaScriptingLib.GetType("Microsoft.SqlServer.VSTAHosting.VSTAScriptProjectStorage+VSTAScriptFile");
		private static readonly Type vstaEncodingType = AssemblyLoader.VstaScriptingLib.GetType("Microsoft.SqlServer.VSTAHosting.VSTAScriptProjectStorage+Encoding");

		private static readonly string cSharpDisplayName = (string)vstaScriptLanguagesType.GetMethod("GetDisplayName", new Type[] { typeof(string) }).Invoke(null, new string[] { "CSharp" });
		private static readonly Dictionary<string, dynamic> scriptStorages = new Dictionary<string, dynamic>();

		public ScriptTaskWrapper(ContainerWrapper containerWrapper, string scriptProjectName, bool hasReference, ScriptTaskScope scope) : base(containerWrapper, "STOCK:ScriptTask")
		{
			ScriptTask = Convert.ChangeType(TaskHost.InnerObject, scriptTaskType);
			ScriptTask.ScriptLanguage = cSharpDisplayName;

			if (hasReference)
				ScriptingEngine.VstaHelper.LoadProjectFromStorage(scriptStorages[scriptProjectName]);
			else
			{
				try
				{
					ScriptingEngine.VstaHelper.LoadNewProject(ScriptTask.ProjectTemplatePath, null, scriptProjectName);
				}
				catch (System.IO.FileNotFoundException ex)
				{
					throw new InvalidOperationException($"Failed to load dependency {ex.FileName}. Ensure that you have installed the required SSIS components for your version of SQL Server.", ex);
				}

				// Add the ScriptStorage to the global list so it can be accessed later by a ScriptTaskReference.
				if (scope == ScriptTaskScope.Project)
					scriptStorages[scriptProjectName] = ScriptStorage;
			}

			ScriptingEngine.SaveProjectToStorage();
		}

		public ICollection<string> BuildErrors { get { return ScriptingEngine.VstaHelper.GetBuildErrors(string.Empty); } }

		public string ReadOnlyVariables { get { return ScriptTask.ReadOnlyVariables; } set { ScriptTask.ReadOnlyVariables = value; } }

		public string ReadWriteVariables { get { return ScriptTask.ReadWriteVariables; } set { ScriptTask.ReadWriteVariables = value; } }

		public void AddReferences(IEnumerable<string> references)
		{
			ScriptingEngine.VstaHelper.LoadVSTA2Project(ScriptStorage, null, references);

			// The project needs to be saved after references have been added.
			SaveProject();
		}

		public void SaveProject()
		{
			ScriptingEngine.VstaHelper.Build(string.Empty);
			ScriptingEngine.SaveProjectToStorage();
		}

		public void AddFileToProject(string fileName, string fileContent)
		{
			bool success = ScriptTask.ScriptingEngine.VstaHelper.AddFileToProject(fileName, fileContent);

			if (!success)
			{
				object[] args = { Enum.Parse(vstaEncodingType, "UTF8"), fileContent };
				ScriptStorage.ScriptFiles[fileName] = Activator.CreateInstance(vstaScriptFileType, args);

				ScriptingEngine.LoadProjectFromStorage();
			}
		}

		private dynamic ScriptingEngine { get { return ScriptTask.ScriptingEngine; } }

		private dynamic ScriptStorage { get { return ScriptTask.ScriptStorage; } }

		private dynamic ScriptTask { get; }
	}
}
