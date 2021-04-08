// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Daf.Core.Sdk;

namespace Daf.Core.Ssis
{
	public class SsisPlugin : IPlugin
	{
		public string Name { get => "SSIS Plugin"; }
		public string Description { get => "Generates SSIS projects and packages."; }
		public string Version { get => ThisAssembly.AssemblyInformationalVersion; }
		public string TimeStamp { get => ThisAssembly.GitCommitDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }

		public int Execute()
		{
			if (Convert.ToBoolean(Properties.Instance.OtherProperties["BuildSqlOnly"], CultureInfo.InvariantCulture))
			{
				Console.WriteLine($"Skipped generating SSIS packages.");

				return 0;
			}
			else
			{
				string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

				int exitCode = 0;

				using (Process wrapper = new()
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = Path.Combine(assemblyPath, "../../runtimes/net48/Daf.Core.Ssis.Workaround/SsisWorkaround.exe"),
						Arguments = Properties.Instance.CommandLine!
					}
				})
				{
					wrapper.Start();
					wrapper.WaitForExit(); // Wait until the application is done. This also results in console output being piped back in real time.
					exitCode = wrapper.ExitCode;
				}

				return exitCode;
			}
		}
	}
}
