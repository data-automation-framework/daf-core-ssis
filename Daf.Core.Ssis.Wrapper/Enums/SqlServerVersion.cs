// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Ssis.Wrapper.Enums
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "I disagree.")]
	public enum SqlServerVersion
	{
		SqlServer2016 = 13,
		SqlServer2017 = 14,
		SqlServer2019 = 15
	}
}
