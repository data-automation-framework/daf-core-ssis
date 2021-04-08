// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Linq;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Components;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;

namespace Daf.Core.Ssis.Components
{
	internal abstract class Component
	{
		protected static void SetConnectionManager(ProjectWrapper projectWrapper, PackageWrapper packageWrapper, ComponentWrapper componentWrapper, string connectionName)
		{
			// See if the referenced connection manager exists by name, in both the package and the project.
			ConnectionManagerWrapper projectConnectionManager = projectWrapper.ConnectionManagers.ToList().Find(connection => connection.Name == connectionName);
			ConnectionManagerWrapper packageConnectionManager = packageWrapper.ConnectionManagers.ToList().Find(connection => connection.Name == connectionName);

			if (projectConnectionManager == null && packageConnectionManager == null) // If both are null, no matching connection could be found.
				throw new InvalidOperationException($"Failed to find connection manager {connectionName}!");
			else if (projectConnectionManager != null && packageConnectionManager != null) // If neither are null, we don't know which one to pick. The package and the project shouldn't both contain the same connection.
				throw new InvalidOperationException($"Connection manager {connectionName} is defined in both the project and the package, this isn't valid!");
			else if (projectConnectionManager != null)
				componentWrapper.SetConnectionManager(projectConnectionManager);
			else if (packageConnectionManager != null)
				componentWrapper.SetConnectionManager(packageConnectionManager);
		}
	}
}
