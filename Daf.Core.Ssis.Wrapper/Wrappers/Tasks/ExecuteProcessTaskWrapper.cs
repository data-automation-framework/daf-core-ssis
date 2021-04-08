// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Diagnostics;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Tasks
{
	public class ExecuteProcessTaskWrapper : TaskWrapper
	{
		public ExecuteProcessTaskWrapper(ContainerWrapper containerWrapper) : base(containerWrapper, "Microsoft.ExecuteProcess") { }

		public string Arguments
		{
			get { return TaskHost.Properties["Arguments"].GetValue(TaskHost); }
			set { TaskHost.Properties["Arguments"].SetValue(TaskHost, value); }
		}

		public string Executable
		{
			get { return TaskHost.Properties["Executable"].GetValue(TaskHost); }
			set { TaskHost.Properties["Executable"].SetValue(TaskHost, value); }
		}

		public bool FailTaskIfReturnCodeIsNotSuccessValue
		{
			get { return TaskHost.Properties["FailTaskIfReturnCodeIsNotSuccessValue"].GetValue(TaskHost); }
			set { TaskHost.Properties["FailTaskIfReturnCodeIsNotSuccessValue"].SetValue(TaskHost, value); }
		}

		public bool RequireFullFileName
		{
			get { return TaskHost.Properties["RequireFullFileName"].GetValue(TaskHost); }
			set { TaskHost.Properties["RequireFullFileName"].SetValue(TaskHost, value); }
		}

		public string StandardErrorVariable
		{
			get { return TaskHost.Properties["StandardErrorVariable"].GetValue(TaskHost); }
			set { TaskHost.Properties["StandardErrorVariable"].SetValue(TaskHost, value); }
		}

		public string StandardInputVariable
		{
			get { return TaskHost.Properties["StandardInputVariable"].GetValue(TaskHost); }
			set { TaskHost.Properties["StandardInputVariable"].SetValue(TaskHost, value); }
		}

		public string StandardOutputVariable
		{
			get { return TaskHost.Properties["StandardOutputVariable"].GetValue(TaskHost); }
			set { TaskHost.Properties["StandardOutputVariable"].SetValue(TaskHost, value); }
		}

		public int SuccessValue
		{
			get { return TaskHost.Properties["SuccessValue"].GetValue(TaskHost); }
			set { TaskHost.Properties["SuccessValue"].SetValue(TaskHost, value); }
		}

		public bool TerminateProcessAfterTimeOut
		{
			get { return TaskHost.Properties["TerminateProcessAfterTimeOut"].GetValue(TaskHost); }
			set { TaskHost.Properties["TerminateProcessAfterTimeOut"].SetValue(TaskHost, value); }
		}

		public int TimeOut
		{
			get { return TaskHost.Properties["TimeOut"].GetValue(TaskHost); }
			set { TaskHost.Properties["TimeOut"].SetValue(TaskHost, value); }
		}

		public ProcessWindowStyle WindowStyle
		{
			get { return TaskHost.Properties["WindowStyle"].GetValue(TaskHost); }
			set { TaskHost.Properties["WindowStyle"].SetValue(TaskHost, value); }
		}

		public string WorkingDirectory
		{
			get { return TaskHost.Properties["WorkingDirectory"].GetValue(TaskHost); }
			set { TaskHost.Properties["WorkingDirectory"].SetValue(TaskHost, value); }
		}
	}
}
