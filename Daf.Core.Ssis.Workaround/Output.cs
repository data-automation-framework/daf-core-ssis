// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Daf.Core.Sdk;
using Daf.Core.Ssis.Wrapper.Enums;
using Daf.Core.Ssis.Wrapper.Wrappers;

namespace Daf.Core.Ssis
{
	internal class Output
	{
		public Output(ProjectWrapper projectWrapper)
		{
			Name = projectWrapper.Name;
			DtProjFilePath = Path.Combine(Properties.Instance.OutputDirectory, projectWrapper.Name, projectWrapper.Name + ".dtproj");
			IspacFilePath = Path.ChangeExtension(DtProjFilePath, ".ispac");
			ProjectWrapper = projectWrapper;
			SqlServerVersion = projectWrapper.Version;
		}

		public void Generate()
		{
			Directory.CreateDirectory(Path.GetDirectoryName(DtProjFilePath));
			ProjectWrapper.SaveTo(IspacFilePath);

			// Extract (from the .ispac) and create the basic files we need.
			ExtractRequiredFiles();
			GenerateDtProj();

			// Create other files required to prevent SSIS from thinking the whole project has been modified.
			GenerateDatabase();
			GenerateUser();

			// Create the obj files.
			string dtProjDirectory = Path.GetDirectoryName(DtProjFilePath);
			string objDirectory = Path.Combine(dtProjDirectory, "obj", "Development");
			Directory.CreateDirectory(objDirectory);

			foreach (string file in Directory.GetFiles(dtProjDirectory))
			{
				string extension = Path.GetExtension(file);

				if (extension == ".conmgr" || extension == ".dtproj" || extension == ".dtsx" || extension == ".params")
					File.Copy(file, Path.Combine(objDirectory, Path.GetFileName(file)));
			}

			GenerateBuildLog();

			// Copy .ispac to bin directory.
			string binDirectory = Path.Combine(dtProjDirectory, "bin", "Development");
			string ispacDestinationFilePath = Path.Combine(binDirectory, $"{Name}.ispac");

			Directory.CreateDirectory(binDirectory);
			File.Copy(IspacFilePath, ispacDestinationFilePath);

			// Touch last modified time in order to make the .ispac more recent than everything else.
			File.SetLastWriteTime(ispacDestinationFilePath, DateTime.Now);
		}

		internal string Name { get; }

		internal string DtProjFilePath { get; }

		internal string IspacFilePath { get; }

		internal ProjectWrapper ProjectWrapper { get; }

		internal SqlServerVersion SqlServerVersion { get; }

		private void ExtractRequiredFiles()
		{
			using (ZipArchive zipArchive = ZipFile.OpenRead(IspacFilePath))
			{
				string ispacDirectory = Path.GetDirectoryName(IspacFilePath);

				foreach (ZipArchiveEntry entry in zipArchive.Entries)
				{
					string extension = Path.GetExtension(entry.Name);

					if (extension == ".conmgr" || extension == ".dtsx" || extension == ".params")
						entry.ExtractToFile(Path.Combine(ispacDirectory, entry.Name));
				}
			}
		}

		private void GenerateBuildLog()
		{
			DateTime currentDateTime = DateTime.UtcNow;

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<?xml version=\"1.0\"?>");
			stringBuilder.AppendLine("<BuildLog xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
			stringBuilder.AppendLine("  <ProjectInfo>");
			stringBuilder.AppendLine($"    <Name>{Name}</Name>");
			stringBuilder.AppendLine($"    <LastWriteTime>{currentDateTime:yyyy-MM-ddTHH:mm:ssZ}</LastWriteTime>");
			stringBuilder.AppendLine("    <LastKnownProtectionLevel>EncryptSensitiveWithPassword</LastKnownProtectionLevel>");
			stringBuilder.AppendLine("    <Salt>bE6L26lU</Salt>");
			stringBuilder.AppendLine("    <LastKnownProtectedHashedPassword>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAa6xfYeRqnEO091WR9mxJtAAAAAACAAAAAAAQZgAAAAEAACAAAADchpxiLtzA/WHKiDl4UUr8QgOcoMUPGE3Ih5Eh35391QAAAAAOgAAAAAIAACAAAADuldCLcurOAbnnMZJ0PNlzdmX4oBIWuflkC2igZnY6CTAAAABwyug1O+Z7n0ccn//5Ezz1oaF48qQt2C/h9DTz7XbGS7AABtK8bdGE1huhLp2+gjdAAAAAhosDR2UEZCU+yn4jOeUi6+Z9gD1Si3XSUljYKqpldQuaicTxTPM6S5UUpoKTPqk8f3GUgjzOj0uRfdRHrCvHKw==</LastKnownProtectedHashedPassword>");
			stringBuilder.AppendLine("  </ProjectInfo>");
			stringBuilder.AppendLine("  <LastBuildInfos>");

			string objDirectory = Path.Combine(Path.GetDirectoryName(DtProjFilePath), "obj", "Development");

			foreach (string file in Directory.GetFiles(objDirectory, "*.dtsx"))
			{
				stringBuilder.AppendLine("    <ProjectBuildItemInfo>");
				stringBuilder.AppendLine($"      <Name>{Path.GetFileName(file)}</Name>");
				stringBuilder.AppendLine($"      <LastWriteTime>{currentDateTime:yyyy-MM-ddTHH:mm:ssZ}</LastWriteTime>");
				stringBuilder.AppendLine("      <LastKnownProtectionLevel>EncryptSensitiveWithPassword</LastKnownProtectionLevel>");
				stringBuilder.AppendLine("      <Salt>bE6L26lU</Salt>");
				stringBuilder.AppendLine("      <LastKnownProtectedHashedPassword>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAa6xfYeRqnEO091WR9mxJtAAAAAACAAAAAAAQZgAAAAEAACAAAADchpxiLtzA/WHKiDl4UUr8QgOcoMUPGE3Ih5Eh35391QAAAAAOgAAAAAIAACAAAADuldCLcurOAbnnMZJ0PNlzdmX4oBIWuflkC2igZnY6CTAAAABwyug1O+Z7n0ccn//5Ezz1oaF48qQt2C/h9DTz7XbGS7AABtK8bdGE1huhLp2+gjdAAAAAhosDR2UEZCU+yn4jOeUi6+Z9gD1Si3XSUljYKqpldQuaicTxTPM6S5UUpoKTPqk8f3GUgjzOj0uRfdRHrCvHKw==</LastKnownProtectedHashedPassword>");
				stringBuilder.AppendLine("    </ProjectBuildItemInfo>");
			}

			stringBuilder.AppendLine("  </LastBuildInfos>");
			stringBuilder.Append("</BuildLog>");

			File.WriteAllText(Path.Combine(objDirectory, "BuildLog.xml"), stringBuilder.ToString());
		}

		private void GenerateDatabase()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<Database xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\" xmlns:ddl100_100=\"http://schemas.microsoft.com/analysisservices/2008/engine/100/100\" xmlns:ddl200=\"http://schemas.microsoft.com/analysisservices/2010/engine/200\" xmlns:ddl200_200=\"http://schemas.microsoft.com/analysisservices/2010/engine/200/200\" xmlns:ddl300=\"http://schemas.microsoft.com/analysisservices/2011/engine/300\" xmlns:ddl300_300=\"http://schemas.microsoft.com/analysisservices/2011/engine/300/300\" xmlns:ddl400=\"http://schemas.microsoft.com/analysisservices/2012/engine/400\" xmlns:ddl400_400=\"http://schemas.microsoft.com/analysisservices/2012/engine/400/400\" xmlns:ddl500=\"http://schemas.microsoft.com/analysisservices/2013/engine/500\" xmlns:ddl500_500=\"http://schemas.microsoft.com/analysisservices/2013/engine/500/500\" xmlns:dwd=\"http://schemas.microsoft.com/DataWarehouse/Designer/1.0\" dwd:design-time-name=\"e7d0eb5a-51a1-405b-b4aa-75e101d4169c\" xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">");
			stringBuilder.AppendLine($"  <ID>{Name}</ID>");
			stringBuilder.AppendLine($"  <Name>{Name}</Name>");
			stringBuilder.AppendLine("  <CreatedTimestamp>0001-01-01T00:00:00Z</CreatedTimestamp>");
			stringBuilder.AppendLine("  <LastSchemaUpdate>0001-01-01T00:00:00Z</LastSchemaUpdate>");
			stringBuilder.AppendLine("  <LastProcessed>0001-01-01T00:00:00Z</LastProcessed>");
			stringBuilder.AppendLine("  <State>Unprocessed</State>");
			stringBuilder.AppendLine("  <LastUpdate>0001-01-01T00:00:00Z</LastUpdate>");
			stringBuilder.AppendLine("  <DataSourceImpersonationInfo>");
			stringBuilder.AppendLine("    <ImpersonationMode>Default</ImpersonationMode>");
			stringBuilder.AppendLine("    <ImpersonationInfoSecurity>Unchanged</ImpersonationInfoSecurity>");
			stringBuilder.AppendLine("  </DataSourceImpersonationInfo>");
			stringBuilder.Append("</Database>");

			File.WriteAllText(Path.ChangeExtension(DtProjFilePath, ".database"), stringBuilder.ToString());
		}

		private void GenerateDtProj()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			stringBuilder.AppendLine("<Project xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
			stringBuilder.AppendLine("  <DeploymentModel>Project</DeploymentModel>");
			stringBuilder.AppendLine("  <ProductVersion>15.0.2000.157</ProductVersion>");
			stringBuilder.AppendLine("  <SchemaVersion>9.0.1.0</SchemaVersion>");
			stringBuilder.AppendLine("  <Database>");
			stringBuilder.AppendLine($"    <Name>{Name}.database</Name>");
			stringBuilder.AppendLine($"    <FullPath>{Name}.database</FullPath>");
			stringBuilder.AppendLine("  </Database>");
			stringBuilder.AppendLine("  <DataSources />");
			stringBuilder.AppendLine("  <DataSourceViews />");
			stringBuilder.AppendLine("  <DeploymentModelSpecificContent>");
			stringBuilder.AppendLine("    <Manifest>");

			stringBuilder.AppendLine(GetDtProjManifest());

			stringBuilder.AppendLine("    </Manifest>");
			stringBuilder.AppendLine("  </DeploymentModelSpecificContent>");
			stringBuilder.AppendLine("  <ControlFlowParts />");
			stringBuilder.AppendLine("  <Miscellaneous />");
			stringBuilder.AppendLine("  <Configurations>");
			stringBuilder.AppendLine("    <Configuration>");
			stringBuilder.AppendLine("      <Name>Development</Name>");
			stringBuilder.AppendLine("      <Options>");
			stringBuilder.AppendLine("        <OutputPath>bin</OutputPath>");
			stringBuilder.AppendLine("        <ConnectionMappings/>");
			stringBuilder.AppendLine("        <ConnectionProviderMappings/>");
			stringBuilder.AppendLine("        <ConnectionSecurityMappings/>");
			stringBuilder.AppendLine("        <DatabaseStorageLocations/>");
			stringBuilder.AppendLine($"        <TargetServerVersion>{SqlServerVersion.ToString().Replace("Sql", "SQL")}</TargetServerVersion>");
			stringBuilder.AppendLine("        <ParameterConfigurationValues>");
			stringBuilder.AppendLine("          <ConfigurationSetting>");
			stringBuilder.AppendLine("            <Id>LastModifiedTime</Id>");
			stringBuilder.AppendLine("            <Name>LastModifiedTime</Name>");
			stringBuilder.AppendLine("            <Value xsi:type=\"xsd:dateTime\">0001-01-01T00:00:00Z</Value>");
			stringBuilder.AppendLine("          </ConfigurationSetting>");
			stringBuilder.AppendLine("        </ParameterConfigurationValues>");
			stringBuilder.AppendLine("      </Options>");
			stringBuilder.AppendLine("    </Configuration>");
			stringBuilder.AppendLine("  </Configurations>");
			stringBuilder.Append("</Project>");

			File.WriteAllText(DtProjFilePath, stringBuilder.ToString());
		}

		private void GenerateUser()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			stringBuilder.AppendLine("<DataTransformationsUserConfiguration xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
			stringBuilder.AppendLine("  <Configurations>");
			stringBuilder.AppendLine("    <Configuration>");
			stringBuilder.AppendLine("      <Name>Development</Name>");
			stringBuilder.AppendLine("      <Options>");
			stringBuilder.AppendLine("        <ServerName />");
			stringBuilder.AppendLine("        <PathOnServer />");
			stringBuilder.AppendLine("        <UserIDs />");
			stringBuilder.AppendLine("        <UserPasswords />");
			stringBuilder.AppendLine("        <OfflineMode>false</OfflineMode>");
			stringBuilder.AppendLine("        <ProgressReporting>true</ProgressReporting>");
			stringBuilder.AppendLine("        <ParameterConfigurationSensitiveValues>");
			stringBuilder.AppendLine("          <ConfigurationSetting>");
			stringBuilder.AppendLine("            <Id>LastModifiedTime</Id>");
			stringBuilder.AppendLine("            <Name>LastModifiedTime</Name>");
			stringBuilder.AppendLine("            <Value xsi:type=\"xsd:dateTime\">0001-01-01T00:00:00Z</Value>");
			stringBuilder.AppendLine("          </ConfigurationSetting>");
			stringBuilder.AppendLine("        </ParameterConfigurationSensitiveValues>");
			stringBuilder.AppendLine("      </Options>");
			stringBuilder.AppendLine("    </Configuration>");
			stringBuilder.AppendLine("  </Configurations>");
			stringBuilder.Append("</DataTransformationsUserConfiguration>");

			File.WriteAllText($"{DtProjFilePath}.user", stringBuilder.ToString());
		}

		private string GetDtProjManifest()
		{
			using (ZipArchive zipArchive = ZipFile.OpenRead(IspacFilePath))
			{
				foreach (ZipArchiveEntry entry in zipArchive.Entries)
				{
					if (entry.Name == "@Project.manifest")
					{
						using (StreamReader reader = new StreamReader(entry.Open()))
							return ProcessProjectManifest(reader.ReadToEnd());
					}
				}
			}

			throw new FileNotFoundException($"Failed to find project manifest in .ispac when generating {Name}!");
		}

		private static string ProcessProjectManifest(string projectManifest)
		{
			// Update project manifest to properly handle DevArt MySql connections.
			string processedProjectManifest = projectManifest.Replace("<SSIS:Property SSIS:Name=\"Value\">130</SSIS:Property>", "<SSIS:Property SSIS:Name=\"Value\">150</SSIS:Property>");

			// Update project manifest to properly handle project parameters.
			string originalParameterString = String.Join
				(
					Environment.NewLine,
					"              <SSIS:Property SSIS:Name=\"Sensitive\">0</SSIS:Property>",
					"              <SSIS:Property SSIS:Name=\"Value\">",
					"              </SSIS:Property>",
					"              <SSIS:Property SSIS:Name=\"DataType\">18</SSIS:Property>"
				);

			string newParameterString = String.Join
				(
					Environment.NewLine,
					"              <SSIS:Property SSIS:Name=\"Sensitive\">0</SSIS:Property>",
					"              <SSIS:Property SSIS:Name=\"DataType\">18</SSIS:Property>"
				);

			return processedProjectManifest.Replace(originalParameterString, newParameterString);
		}
	}
}
