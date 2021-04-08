// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Components
{
	public class OleDbDestinationComponentWrapper : ComponentWrapper
	{
		public OleDbDestinationComponentWrapper(DataFlowTaskWrapper dataFlowTaskWrapper) : base(dataFlowTaskWrapper, "Microsoft.OleDbDestination") { }

		public bool CheckConstraints
		{
			get { return FastLoadOptions.ToUpper(CultureInfo.InvariantCulture).Contains("CHECK_CONSTRAINTS"); }
			set { FastLoadOptions = FormatFastLoadOptions("CHECK_CONSTRAINTS", value); }
		}

		public bool KeepIdentity { set { SetCustomProperty("FastLoadKeepIdentity", value); } }

		public bool KeepNulls { set { SetCustomProperty("FastLoadKeepNulls", value); } }

		public int MaximumInsertCommitSize { set { SetCustomProperty("FastLoadMaxInsertCommitSize", value); } }

		public string Table { set { SetCustomProperty("OpenRowset", value); } }

		public bool TableLock
		{
			get { return FastLoadOptions.ToUpper(CultureInfo.InvariantCulture).Contains("TABLOCK"); }
			set { FastLoadOptions = FormatFastLoadOptions("TABLOCK", value); }
		}

		// TODO: Refactor this so we can support the following missing access modes:
		// 1 (OPENROWSET_VARIABLE), 2 (SQLCOMMAND), 4 (OPENROWSET_FASTLOAD_VARIABLE)
		public bool UseFastLoadIfAvailable
		{
			set
			{
				if (value)
					SetCustomProperty("AccessMode", 3);
				else
					SetCustomProperty("AccessMode", 0);
			}
		}

		private string FastLoadOptions
		{
			get { return GetCustomProperty<string>(nameof(FastLoadOptions)); }
			set { SetCustomProperty(nameof(FastLoadOptions), value); }
		}

		/// <summary>
		/// Builds a string containing fast load options.
		/// </summary>
		/// <param name="option">The fast load option to add or remove</param>
		/// <param name="shouldAdd">True if it should be added, false if it should be removed</param>
		/// <returns>The string of fast load options</returns>
		private string FormatFastLoadOptions(string option, bool shouldAdd)
		{
			List<string> fastLoadOptions = FastLoadOptions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

			if (shouldAdd)
			{
				if (!fastLoadOptions.Contains(option))
					fastLoadOptions.Add(option);
			}
			else
			{
				if (fastLoadOptions.Contains(option))
					fastLoadOptions.Remove(option);
			}

			return string.Join(",", fastLoadOptions);
		}
	}
}
