// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Ssis.Wrapper.Wrappers
{
	public class ParameterWrapper
	{
		public ParameterWrapper(dynamic parameter)
		{
			Parameter = parameter;
		}

		public bool IsRequired { get { return Parameter.Required; } set { Parameter.Required = value; } }

		public object Value { get { return Parameter.Value; } set { Parameter.Value = value; } }

		private dynamic Parameter { get; }
	}
}
