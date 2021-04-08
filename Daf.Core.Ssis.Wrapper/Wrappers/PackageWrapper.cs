// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;

namespace Daf.Core.Ssis.Wrapper.Wrappers
{
	public class PackageWrapper : ContainerWrapper, IConnectionManagerContainer
	{
		private static readonly Type connectionsType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.Connections");

		public PackageWrapper() : base(AssemblyLoader.ManagedDts.CreateInstance("Microsoft.SqlServer.Dts.Runtime.Package"))
		{
			Package = InnerObject;
		}

		public ICollection<ConnectionManagerWrapper> ConnectionManagers { get; } = new List<ConnectionManagerWrapper>();

		public int LocaleId { get { return Package.LocaleID; } set { Package.LocaleID = value; } }

		public dynamic AddConnectionManager(string name, string connectionString, string creationName)
		{
			string[] parameters = { creationName };

			dynamic connectionManager = connectionsType.GetMethod("Add").Invoke(Package.Connections, parameters);
			connectionManager.Name = name;
			connectionManager.ConnectionString = connectionString;

			return connectionManager;
		}

		public void AddConnectionManagerWrapper(ConnectionManagerWrapper connectionManagerWrapper)
		{
			ConnectionManagers.Add(connectionManagerWrapper);
		}

		public object AddParameter(string parameterName, TypeCode typeCode)
		{
			return Package.Parameters.Add(parameterName, typeCode);
		}

		public bool ContainsParameter(string parameterName)
		{
			if (Package.Parameters.Contains(parameterName))
				return true;
			else
				return false;
		}

		internal dynamic Package { get; }
	}
}
