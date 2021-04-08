// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Sdk;
using Daf.Core.Ssis.Factories;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Tasks;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers;

namespace Daf.Core.Ssis
{
	internal class Project
	{
		// TODO: This shouldn't be global, it prevents us from handling multiple projects per build.
		internal static List<ScriptProject> GlobalScriptProjects { get; } = new List<ScriptProject>();

		public Project(SsisProject ssisProject)
		{
			ProjectWrapper = new ProjectWrapper()
			{
				Name = ssisProject.Name,
				LocaleId = ssisProject.LocaleId,
				StopBuildOnScriptErrors = Convert.ToBoolean(Properties.Instance.OtherProperties["StopBuildOnScriptErrors"], CultureInfo.InvariantCulture),
				Version = (SqlServerVersion)ssisProject.TargetSqlServerVersion
			};

			SetProtectionLevel(ssisProject.ProtectionLevel, ssisProject.Password);
			SetParameters(ssisProject.Parameters);
			ConnectionManagerFactory.CreateConnectionManagers(ProjectWrapper, ssisProject.Connections);
			CreateScriptProjects(ssisProject.ScriptProjects);
			CreatePackages(ssisProject.Packages);
		}

		internal ProjectWrapper ProjectWrapper { get; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "We don't have a reason to save the results yet.")]
		private void CreateScriptProjects(List<ScriptProject> scriptProjects)
		{
			if (scriptProjects == null || scriptProjects.Count == 0)
				return;

			System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();

			GlobalScriptProjects.AddRange(scriptProjects);

			// This is a hidden, "fake" package that will contain all of our global script projects.
			// It will not be added to the actual project, and it will not be written to disk.
			PackageWrapper hiddenFakePackage = new PackageWrapper()
			{
				Name = "HiddenScriptProjectPackage",
				LocaleId = CultureInfo.CurrentCulture.LCID
			};

			foreach (ScriptProject scriptProject in scriptProjects)
				new ScriptTask(scriptProject, ProjectWrapper, hiddenFakePackage, hiddenFakePackage);

			timer.Stop();
			string duration = TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
			Console.WriteLine($"Finished generating global script projects in {duration}");
		}

		private void CreatePackages(List<IonStructure.Package> packages)
		{
			if (packages == null)
				return;

			System.Diagnostics.Stopwatch packagesTimer = System.Diagnostics.Stopwatch.StartNew();

			for (int i = 0; i < packages.Count; i++)
			{
				System.Diagnostics.Stopwatch packageTimer = System.Diagnostics.Stopwatch.StartNew();
				CreatePackage(packages[i]);
				packageTimer.Stop();

				string packageDuration = TimeSpan.FromMilliseconds(packageTimer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
				Console.WriteLine($"Generated package {packages[i].Name}.dtsx in {packageDuration} ({i + 1}/{packages.Count})");
			}

			packagesTimer.Stop();
			string packagesDuration = TimeSpan.FromMilliseconds(packagesTimer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
			Console.WriteLine($"Generated {packages.Count} packages in {packagesDuration}. Now validating and building .ispac, this usually takes a while...");
		}

		private void CreatePackage(IonStructure.Package ionPackage)
		{
			try
			{
				Package package = new Package(ionPackage, ProjectWrapper);
				ProjectWrapper.AddPackage(package.PackageWrapper);
			}
			catch (Exception e)
			{
				e.Data[Constants.ExceptionPackageKey] = $"{ionPackage.Name}.dtsx";
				throw;
			}
		}

		private void SetParameters(List<Parameter> parameters)
		{
			if (parameters == null)
				return;

			foreach (Parameter parameter in parameters)
				ProjectWrapper.AddParameter(parameter.Name, parameter.DataType, parameter.Value);
		}

		private void SetProtectionLevel(ProtectionLevelEnum protectionLevel, string password)
		{
			ProjectWrapper.ProtectionLevel = protectionLevel.ToString();

			if (protectionLevel != ProtectionLevelEnum.DontSaveSensitive && password == null)
				throw new InvalidOperationException($"Specify a password for protection level {protectionLevel}!");
			else
				ProjectWrapper.Password = password;
		}
	}
}
