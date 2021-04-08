// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.Tasks;

namespace Daf.Core.Ssis.Tasks
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Can't make it static since it has a base class. TODO for later.")]
	internal class ExecuteProcessTask : Task
	{
		public static ExecuteProcessTaskWrapper CreateTask(ExecuteProcess executeProcess, ContainerWrapper containerWrapper)
		{
			ExecuteProcessTaskWrapper executeProcessTaskWrapper = new ExecuteProcessTaskWrapper(containerWrapper)
			{
				Name = executeProcess.Name,
				DelayValidation = executeProcess.DelayValidation,
				ForceExecutionResult = executeProcess.ForceExecutionResult.ToString(),
				Arguments = executeProcess.Arguments,
				Executable = executeProcess.Executable,
				RequireFullFileName = executeProcess.RequireFullFileName,
				FailTaskIfReturnCodeIsNotSuccessValue = executeProcess.FailTaskIfReturnCodeIsNotSuccessValue,
				StandardInputVariable = executeProcess.StandardInputVariableName,
				StandardErrorVariable = executeProcess.StandardErrorVariableName,
				StandardOutputVariable = executeProcess.StandardOutputVariableName,
				SuccessValue = executeProcess.SuccessValue,
				TerminateProcessAfterTimeOut = executeProcess.TerminateProcessAfterTimeOut,
				TimeOut = executeProcess.TimeOut,
				WindowStyle = (System.Diagnostics.ProcessWindowStyle)executeProcess.WindowStyle,
				WorkingDirectory = executeProcess.WorkingDirectory,
			};

			executeProcessTaskWrapper.PropagateErrors(executeProcess.PropagateErrors);
			AddExpressions(executeProcessTaskWrapper, executeProcess.PropertyExpressions);

			return executeProcessTaskWrapper;
		}
	}
}
