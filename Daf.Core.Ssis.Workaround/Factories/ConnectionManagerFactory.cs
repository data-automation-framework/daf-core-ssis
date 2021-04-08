// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using Daf.Core.Ssis.ConnectionManagers;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;

namespace Daf.Core.Ssis.Factories
{
	public static class ConnectionManagerFactory
	{
		public static void CreateConnectionManagers(IConnectionManagerContainer connectionManagerContainer, List<Connection> connectionManagers)
		{
			if (connectionManagers == null)
				return;

			foreach (Connection connection in connectionManagers)
				CreateConnectionManager(connectionManagerContainer, connection);
		}

		private static void CreateConnectionManager(IConnectionManagerContainer connectionManagerContainer, Connection connection)
		{
			if (connectionManagerContainer == null)
				throw new ArgumentNullException(nameof(connectionManagerContainer), "Connection can't be added to a null project or package!");

			ConnectionManagerWrapper connectionManagerWrapper;

			switch (connection)
			{
				case FlatFileConnection flatFileConnection:
					connectionManagerWrapper = FlatFileConnectionManager.CreateConnectionManager(connectionManagerContainer, flatFileConnection);
					break;
				case OleDbConnection _:
					connectionManagerWrapper = new OleDbConnectionManagerWrapper(connectionManagerContainer, connection.Name, connection.ConnectionString, connection.GUID);
					break;
				case CustomConnection customConnection:
					connectionManagerWrapper = new CustomConnectionManagerWrapper(connectionManagerContainer, connection.Name, connection.ConnectionString, connection.GUID, customConnection.CreationName);
					break;
				default:
					throw new ArgumentException($"Connection manager {connection.Name} is of an invalid type!");
			}

			connectionManagerWrapper.DelayValidation = connection.DelayValidation;
			AddExpressions(connectionManagerWrapper, connection.PropertyExpressions);

			connectionManagerContainer.AddConnectionManagerWrapper(connectionManagerWrapper);
		}

		private static void AddExpressions(ConnectionManagerWrapper connectionManagerWrapper, List<PropertyExpression> expressions)
		{
			if (expressions == null)
				return;

			foreach (PropertyExpression expression in expressions)
				connectionManagerWrapper.SetExpression(expression.PropertyName, expression.Value);
		}
	}
}
