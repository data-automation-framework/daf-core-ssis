// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Globalization;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Tasks
{
	public class ExecuteSqlTaskWrapper : TaskWrapper
	{
		private static readonly Type executeSqlTaskType = AssemblyLoader.ExecuteSqlTask.GetType("Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ExecuteSQLTask");
		private static readonly Type parameterBindingsType = AssemblyLoader.ExecuteSqlTask.GetType("Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.IDTSParameterBindings");
		private static readonly Type parameterBindingType = AssemblyLoader.ExecuteSqlTask.GetType("Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.IDTSParameterBinding");
		private static readonly Type resultSetTypeType = AssemblyLoader.ExecuteSqlTask.GetType("Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.ResultSetType");
		private static readonly Type resultBindingsType = AssemblyLoader.ExecuteSqlTask.GetType("Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.IDTSResultBindings");
		private static readonly Type resultBindingType = AssemblyLoader.ExecuteSqlTask.GetType("Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.IDTSResultBinding");
		private static readonly Type typeConversionModeType = AssemblyLoader.ExecuteSqlTask.GetType("Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask.TypeConversionMode");

		public ExecuteSqlTaskWrapper(ContainerWrapper containerWrapper) : base(containerWrapper, "STOCK:SQLTask")
		{
			ExecuteSqlTask = Convert.ChangeType(TaskHost.InnerObject, executeSqlTaskType);
		}

		public bool BypassPrepare { set { ExecuteSqlTask.BypassPrepare = value; } }

		public uint CodePage { set { ExecuteSqlTask.CodePage = value; } }

		public string ConnectionName { set { ExecuteSqlTask.Connection = value; } }

		public dynamic ResultSetType
		{
			set { ExecuteSqlTask.ResultSetType = Convert.ChangeType(Enum.ToObject(resultSetTypeType, value), resultSetTypeType, CultureInfo.InvariantCulture); }
		}

		public string SqlStatementSource { set { ExecuteSqlTask.SqlStatementSource = value; } }

		public uint TimeOut { set { ExecuteSqlTask.TimeOut = value; } }

		public dynamic TypeConversionMode
		{
			set { ExecuteSqlTask.TypeConversionMode = Convert.ChangeType(Enum.ToObject(typeConversionModeType, value), typeConversionModeType, CultureInfo.InvariantCulture); }
		}

		public void AddParameterBinding(int dataType, string variableName, int parameterDirection, string parameterName, int parameterSize)
		{
			dynamic binding = parameterBindingsType.GetMethod("Add").Invoke(ExecuteSqlTask.ParameterBindings, null);

			parameterBindingType.GetProperty("DataType").SetValue(binding, dataType);
			parameterBindingType.GetProperty("DtsVariableName").SetValue(binding, variableName);
			parameterBindingType.GetProperty("ParameterDirection").SetValue(binding, parameterDirection);
			parameterBindingType.GetProperty("ParameterName").SetValue(binding, parameterName);
			parameterBindingType.GetProperty("ParameterSize").SetValue(binding, parameterSize);
		}

		public void AddResultSetBinding(string resultName, string variableName)
		{
			dynamic binding = resultBindingsType.GetMethod("Add").Invoke(ExecuteSqlTask.ResultSetBindings, null);

			resultBindingType.GetProperty("DtsVariableName").SetValue(binding, variableName);
			resultBindingType.GetProperty("ResultName").SetValue(binding, resultName);
		}

		private dynamic ExecuteSqlTask { get; }
	}
}
