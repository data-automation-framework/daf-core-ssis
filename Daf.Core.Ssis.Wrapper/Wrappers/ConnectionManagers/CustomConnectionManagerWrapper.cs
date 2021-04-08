// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

namespace Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers
{
	public class CustomConnectionManagerWrapper : ConnectionManagerWrapper
	{
		public CustomConnectionManagerWrapper(IConnectionManagerContainer connectionManagerContainer, string name, string connectionString, string guid, string creationName)
			: base(connectionManagerContainer, name, connectionString, guid, creationName) { }

		public override void SetProperties(Dictionary<string, object> properties) { }
	}
}
