// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public class OleDbSourceComponentWrapper : ComponentWrapper
	{
		public OleDbSourceComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper) : base(dataFlowTaskWrapper, "Microsoft.OleDbSource") { }

		public string SqlCommand
		{
			set
			{
				SetCustomProperty("AccessMode", 2);
				SetCustomProperty(nameof(SqlCommand), value);
			}
		}

		public string SqlCommandVariable
		{
			set
			{
				SetCustomProperty("AccessMode", 3);
				SetCustomProperty(nameof(SqlCommandVariable), value);
			}
		}

		private string Parameters
		{
			get { return GetCustomProperty<string>("ParameterMapping"); }
			set { SetCustomProperty("ParameterMapping", value); }
		}

		public void AddParameter(int index, string parameterName, string variableName)
		{
			if (Parameters == null)
				Parameters = CreateParameterString(index, parameterName, variableName);
			else
				Parameters += CreateParameterString(index, parameterName, variableName);
		}

		private static string CreateParameterString(int index, string parameterName, string variableName)
		{
			if (parameterName == null)
				return $"\"Parameter{index}:Input\",{variableName};";
			else
				return $"\"{parameterName}:Input\",{variableName};";
		}
	}
}
