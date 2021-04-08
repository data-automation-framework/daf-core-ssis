// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

namespace Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers
{
	public class OleDbConnectionManagerWrapper : ConnectionManagerWrapper
	{
		public OleDbConnectionManagerWrapper(IConnectionManagerContainer connectionManagerContainer, string name, string connectionString, string guid)
			: base(connectionManagerContainer, name, connectionString, guid, creationName: "OLEDB") { }

		public override void SetProperties(Dictionary<string, object> properties) { }
	}
}
