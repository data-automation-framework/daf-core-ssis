// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;

namespace Daf.Core.Ssis.Wrapper.Wrappers
{
	public class ProjectWrapper : IConnectionManagerContainer
	{
		private static readonly Type connectionManagerItemsType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.ConnectionManagerItems");
		private static readonly Type dtsProtectionLevelType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.DTSProtectionLevel");
		private static readonly Type projectType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.Project");

		public ProjectWrapper()
		{
			Project = projectType.GetMethod("CreateProject", Array.Empty<Type>()).Invoke(null, null);
		}

		public ICollection<ConnectionManagerWrapper> ConnectionManagers { get; } = new List<ConnectionManagerWrapper>();

		public int LocaleId { get; set; }

		public string Name { get { return Project.Name; } set { Project.Name = value; } }

		public int PackageCount { get { return Project.PackageItems.Count; } }

		public ICollection<string> PackageNames
		{
			get
			{
				List<string> packageNames = new List<string>();

				foreach (dynamic task in Project.PackageItems)
					packageNames.Add(task.Package.Name);

				return packageNames;
			}
		}

		public string Password { set { Project.Password = value; } }

		public dynamic ProtectionLevel
		{
			set
			{
				dynamic protectionLevel = Enum.Parse(dtsProtectionLevelType, value);
				Project.ProtectionLevel = Convert.ChangeType(protectionLevel, dtsProtectionLevelType);
			}
		}

		public bool StopBuildOnScriptErrors { get; set; }

		public SqlServerVersion Version { get; set; }

		public dynamic AddConnectionManager(string name, string connectionString, string creationName)
		{
			string[] parameters = { creationName, $"{name}.conmgr" };
			dynamic connectionManager = connectionManagerItemsType.GetMethod("Add").Invoke(Project.ConnectionManagerItems, parameters).ConnectionManager;

			connectionManager.Name = name;
			connectionManager.ConnectionString = connectionString;

			return connectionManager;
		}

		public void AddConnectionManagerWrapper(ConnectionManagerWrapper connectionManagerWrapper)
		{
			if (connectionManagerWrapper == null)
				throw new ArgumentNullException(nameof(connectionManagerWrapper));

			ConnectionManagers.Add(connectionManagerWrapper);
		}

		public void AddPackage(PackageWrapper packageWrapper)
		{
			if (packageWrapper == null)
				throw new ArgumentNullException(nameof(packageWrapper));

			Project.PackageItems.Add(packageWrapper.Package, $"{packageWrapper.Name}.dtsx");
		}

		public void AddParameter(string name, TypeCode type, string value)
		{
			dynamic parameter = Project.Parameters.Add(name, type);
			parameter.Value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}

		public void SaveTo(string ispacFilePath)
		{
			Project.SaveTo(ispacFilePath);
		}

		public bool ContainsParameter(string parameterName)
		{
			if (Project.Parameters.Contains(parameterName))
				return true;
			else
				return false;
		}

		private dynamic Project { get; }
	}
}
