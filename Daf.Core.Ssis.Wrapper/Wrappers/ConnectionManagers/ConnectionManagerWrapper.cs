// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers
{
	public abstract class ConnectionManagerWrapper
	{
		protected ConnectionManagerWrapper(IConnectionManagerContainer connectionManagerContainer, string name, string connectionString, string guid, string creationName)
		{
			if (connectionManagerContainer == null)
				throw new ArgumentNullException(nameof(connectionManagerContainer));

			Name = name;
			SetGuid(guid);

			try
			{
				ConnectionManager = connectionManagerContainer.AddConnectionManager(name, connectionString, creationName);
			}
			catch (TargetInvocationException ex)
			{
				throw new InvalidOperationException($"Failed to create custom Connection {creationName}. Is the corresponding SSIS plugin installed?", ex);
			}
		}

		public bool DelayValidation { get { return ConnectionManager.DelayValidation; } set { ConnectionManager.DelayValidation = value; } }

		public string Name { get; set; }

		internal dynamic ConnectionManager { get; set; }

		private string GUID { get; set; }

		public void SetExpression<T>(string property, T value)
		{
			ConnectionManager.Properties[property].SetExpression(ConnectionManager, value);
		}

		public abstract void SetProperties(Dictionary<string, object> properties);

		private void SetGuid(string inputGuid)
		{
			if (string.IsNullOrEmpty(inputGuid))
				GUID = "{" + Guid.NewGuid().ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			else if (Guid.TryParse(inputGuid, out Guid parsedGuid))
				GUID = '{' + parsedGuid.ToString().ToUpper(CultureInfo.InvariantCulture) + '}';
			else
				throw new ArgumentException($"Failed to parse GUID value {inputGuid}! Valid format is 32 digits separated by hyphens: 00000000-0000-0000-0000-000000000000 (D)");
		}
	}
}
