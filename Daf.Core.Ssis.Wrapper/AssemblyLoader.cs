// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Globalization;
using System.Reflection;
using Daf.Core.Ssis.Wrapper.Enums;

namespace Daf.Core.Ssis.Wrapper
{
	public static class AssemblyLoader
	{
		private static Assembly dtsPipelineWrap;
		internal static Assembly DtsPipelineWrap
		{
			get
			{
				if (dtsPipelineWrap == null)
					dtsPipelineWrap = LoadAssembly("Microsoft.SqlServer.DTSPipelineWrap, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

				return dtsPipelineWrap;
			}
		}

		private static Assembly dtsRuntimeWrap;
		internal static Assembly DtsRuntimeWrap
		{
			get
			{
				if (dtsRuntimeWrap == null)
					dtsRuntimeWrap = LoadAssembly("Microsoft.SqlServer.DTSRuntimeWrap, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

				return dtsRuntimeWrap;
			}
		}

		private static Assembly executeSqlTask;
		internal static Assembly ExecuteSqlTask
		{
			get
			{
				if (executeSqlTask == null)
					executeSqlTask = LoadAssembly("Microsoft.SqlServer.SQLTask, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

				return executeSqlTask;
			}
		}

		private static Assembly managedDts;
		internal static Assembly ManagedDts
		{
			get
			{
				if (managedDts == null)
					managedDts = LoadAssembly("Microsoft.SqlServer.ManagedDTS, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

				return managedDts;
			}
		}

		private static Assembly pipelineHost;
		internal static Assembly PipelineHost
		{
			get
			{
				if (pipelineHost == null)
					pipelineHost = LoadAssembly("Microsoft.SQLServer.PipelineHost, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

				return pipelineHost;
			}
		}

		private static Assembly scriptTask;
		internal static Assembly ScriptTask
		{
			get
			{
				if (scriptTask == null)
					scriptTask = LoadAssembly("Microsoft.SqlServer.ScriptTask, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

				return scriptTask;
			}
		}

		private static Assembly vstaScriptingLib;
		internal static Assembly VstaScriptingLib
		{
			get
			{
				if (vstaScriptingLib == null)
					vstaScriptingLib = LoadAssembly("Microsoft.SqlServer.VSTAScriptingLib, Version={0}.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91");

				return vstaScriptingLib;
			}
		}

		private static int? versionNumber;
		public static int VersionNumber
		{
			get
			{
				if (!versionNumber.HasValue)
					throw new InvalidOperationException("No version has been set in AssemblyLoader!");

				return versionNumber.Value;
			}
			set
			{
				//if (versionNumber.HasValue)
				//	throw new NotSupportedException("Changing the AssemblyLoader version after it has already been set is not supported.");

				versionNumber = value;
			}
		}

		private static Assembly LoadAssembly(string assembly)
		{
			string formattedAssembly = string.Format(CultureInfo.InvariantCulture, assembly, VersionNumber);
			Assembly loadedAssembly = Assembly.Load(formattedAssembly);

			if (loadedAssembly == null)
				throw new InvalidOperationException($"Failed to load dependency {formattedAssembly}. Ensure that you have installed the required SSIS components for {(SqlServerVersion)VersionNumber}.");

			return loadedAssembly;
		}
	}
}
