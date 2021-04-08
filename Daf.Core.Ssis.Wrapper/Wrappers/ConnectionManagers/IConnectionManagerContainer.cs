// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers
{
	/// <summary>
	/// Allows implementing objects to have connection managers added.
	/// </summary>
	public interface IConnectionManagerContainer
	{
		/// <summary>
		/// Add a connection manager to the underlying SSIS COM object and return the created ConnectionManager
		/// </summary>
		/// <param name="name">Name of the connection manager</param>
		/// <param name="connectionString">Connection string of the connection manager</param>
		/// <param name="creationName">The connection manager type (OLEDB, FILE, etc)</param>
		/// <returns>Connection manager COM object created by the SSIS API process</returns>
		dynamic AddConnectionManager(string name, string connectionString, string creationName);

		/// <summary>
		/// Add a ConnectionManagerWrapper to an internal list of the connection manager container
		/// </summary>
		/// <param name="connectionManagerWrapper">The connection manager to add</param>
		void AddConnectionManagerWrapper(ConnectionManagerWrapper connectionManagerWrapper);
	}
}
