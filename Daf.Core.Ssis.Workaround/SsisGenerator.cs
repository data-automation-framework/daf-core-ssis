// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Sdk;
using Daf.Core.Sdk.Ion.Reader;
using Daf.Core.Ssis.IonStructure;

namespace Daf.Core.Ssis
{
	public static class SsisGenerator
	{
		public static void Execute()
		{
			System.Diagnostics.Stopwatch ssisTimer = System.Diagnostics.Stopwatch.StartNew();

			IonReader<IonStructure.Ssis> ssisProjectReader = new IonReader<IonStructure.Ssis>(Properties.Instance.FilePath, typeof(IonStructure.Ssis).Assembly);

			string filename = Properties.Instance.FilePath;

			if (ssisProjectReader.RootNodeExistInFile())
			{
				IonStructure.Ssis ssisRootNode = ssisProjectReader.Parse();

				GenerateProjects(ssisRootNode.SsisProjects);

				ssisTimer.Stop();
				string duration = TimeSpan.FromMilliseconds(ssisTimer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
				Console.WriteLine($"Finished generating SSIS project{(ssisRootNode.SsisProjects.Count > 1 ? "s" : "")} for {filename} in {duration}");
			}
			else
			{
				ssisTimer.Stop();
				Console.WriteLine($"No root node for SSIS plugin found in {filename}, no output generated.");
			}
		}

		private static void GenerateProjects(List<SsisProject> projects)
		{
			int count = 1;

			foreach (SsisProject ssisProject in projects)
			{
				System.Diagnostics.Stopwatch projectTimer = System.Diagnostics.Stopwatch.StartNew();

				Console.WriteLine($"Generating SSIS project {ssisProject.Name} {(projects.Count > 1 ? $"({count}/{projects.Count})" : "")}");

				Wrapper.AssemblyLoader.VersionNumber = (int)ssisProject.TargetSqlServerVersion;

				try
				{
					Project project = new Project(ssisProject);

					if (project.ProjectWrapper.PackageCount != 0)
					{
						Output output = new Output(project.ProjectWrapper);
						output.Generate();
					}
				}
				catch (Exception e)
				{
					e.Data[Constants.ExceptionProjectKey] = ssisProject.Name;
					throw;
				}

				projectTimer.Stop();

				string duration = TimeSpan.FromMilliseconds(projectTimer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);

				Console.WriteLine($"Generated SSIS project {ssisProject.Name} in {duration}{Environment.NewLine}");

				count++;
			}
		}
	}
}
