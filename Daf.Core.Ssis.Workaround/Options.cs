// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using CommandLine;

namespace Daf.Core.Ssis
{
	public class Options
	{
		[Option('m', "intermediate", Required = true, HelpText = "The intermediate.ion file to process.")]
		public string IntermediateFile { get; set; }

		[Option('o', "output", Required = true, HelpText = "The directory that output should be generated to.")]
		public string OutputDirectory { get; set; }

		[Option('p', "parameters", Separator = ';', Required = false, HelpText = "Optional parameters.")]
		public IEnumerable<string> Parameters { get; set; }
	}
}
