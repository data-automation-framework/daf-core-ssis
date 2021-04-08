// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Tasks
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Can't make it static since it has a base class. TODO for later.")]
	internal class ExecuteSqlTask : Task
	{
		public ExecuteSqlTask(ExecuteSql executeSql, ContainerWrapper containerWrapper)
		{
			ExecuteSqlTaskWrapper = new ExecuteSqlTaskWrapper(containerWrapper)
			{
				Name = executeSql.Name,
				DelayValidation = executeSql.DelayValidation,
				ForceExecutionResult = executeSql.ForceExecutionResult.ToString(),
				BypassPrepare = executeSql.BypassPrepare,
				CodePage = executeSql.CodePage,
				ConnectionName = executeSql.ConnectionName,
				ResultSetType = (int)executeSql.ResultSet,
				SqlStatementSource = executeSql.SqlStatement.Value,
				TimeOut = executeSql.TimeOut,
				TypeConversionMode = (int)executeSql.TypeConversionMode
			};

			ExecuteSqlTaskWrapper.PropagateErrors(executeSql.PropagateErrors);
			AddExpressions(ExecuteSqlTaskWrapper, executeSql.PropertyExpressions);

			AddResultSetBindings(executeSql.ResultSet, executeSql.Results);
			AddParameterBindings(executeSql.SqlParameters);
		}

		internal ExecuteSqlTaskWrapper ExecuteSqlTaskWrapper { get; }

		private void AddParameterBindings(List<SqlParameter> parameters)
		{
			if (parameters == null)
				return;

			foreach (SqlParameter parameter in parameters)
			{
				int dataType = Utility.MapDataType(parameter.DataType);
				int parameterDirection;

				switch (parameter.Direction)
				{
					case ParameterDirectionEnum.Input:
						parameterDirection = 1;
						break;
					case ParameterDirectionEnum.Output:
						parameterDirection = 2;
						break;
					case ParameterDirectionEnum.ReturnValue:
						parameterDirection = 4;
						break;
					default:
						throw new InvalidOperationException("Direction must be Input, Output or ReturnValue!");
				}

				int parameterSize = int.Parse(parameter.Size, CultureInfo.InvariantCulture);

				ExecuteSqlTaskWrapper.AddParameterBinding(dataType, parameter.VariableName, parameterDirection, parameter.ParameterName, parameterSize);
			}
		}

		private void AddResultSetBindings(ExecuteSqlResultSetEnum resultSetType, List<Result> resultMappings)
		{
			if (resultMappings == null && resultSetType != ExecuteSqlResultSetEnum.None)
				throw new InvalidOperationException("Result set variable mappings must be defined if result set type is anything but None!");

			if (resultSetType == ExecuteSqlResultSetEnum.SingleRow)
			{
				if (resultMappings.Count == 0)
					throw new InvalidOperationException("Result set variable mappings must contain at least 1 entry for result set type SingleRow!");

				foreach (Result resultMapping in resultMappings)
					ExecuteSqlTaskWrapper.AddResultSetBinding(resultMapping.ResultName, resultMapping.VariableName);
			}

			if (resultSetType == ExecuteSqlResultSetEnum.Full || resultSetType == ExecuteSqlResultSetEnum.Xml)
			{
				if (resultMappings.Count != 1)
					throw new InvalidOperationException("Result set variable mappings must contain 1 entry for result set types Full and Xml!");

				ExecuteSqlTaskWrapper.AddResultSetBinding(resultMappings[0].ResultName, resultMappings[0].VariableName);
			}
		}
	}
}
