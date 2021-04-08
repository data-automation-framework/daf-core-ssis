// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;

namespace Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers
{
	public class FlatFileConnectionManagerWrapper : ConnectionManagerWrapper
	{
		public FlatFileConnectionManagerWrapper(IConnectionManagerContainer connectionManagerContainer, string name, string connectionString, string guid)
			: base(connectionManagerContainer, name, connectionString, guid, creationName: "FLATFILE") { }

		internal dynamic Columns { get { return ConnectionManager.Properties["Columns"].GetValue(ConnectionManager); } }

		public override void SetProperties(Dictionary<string, object> properties)
		{
			if (properties == null)
				throw new ArgumentNullException(nameof(properties));

			foreach (KeyValuePair<string, object> entry in properties)
				ConnectionManager.Properties[entry.Key].SetValue(ConnectionManager, entry.Value);
		}
	}
}
