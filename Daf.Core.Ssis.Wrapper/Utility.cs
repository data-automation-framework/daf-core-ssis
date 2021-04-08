// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Ssis.Wrapper
{
	public static class Utility
	{
		private static readonly Type dataTypeType = AssemblyLoader.DtsRuntimeWrap.GetType("Microsoft.SqlServer.Dts.Runtime.Wrapper.DataType");

		public static int GetDataTypeEnumValue(string dataTypeName)
		{
			return (int)Enum.Parse(dataTypeType, dataTypeName);
		}
	}
}
