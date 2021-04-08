// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Tasks
{
	public abstract class TaskWrapper : ExecutableWrapper
	{
		private static readonly Type taskHostType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.TaskHost");

		protected TaskWrapper(ContainerWrapper containerWrapper, string moniker) : base(containerWrapper, moniker)
		{
			TaskHost = Convert.ChangeType(InnerObject, taskHostType);
		}

		/// <summary>
		/// This will probably break if called twice. Should be fixed. Meanwhile, don't do it.
		/// </summary>
		/// <param name="propagate">True if error propagation is enabled, otherwise false</param>
		public void PropagateErrors(bool propagate)
		{
			if (!propagate)
			{
				// Create an OnError EventHandler for this task.
				dynamic eventHandler = TaskHost.EventHandlers.Add("OnError");

				// Set its Propagate variable to false.
				eventHandler.Variables["Propagate"].Value = false;
			}
		}

		public void SetExpression<T>(string property, T value)
		{
			TaskHost.Properties[property].SetExpression(TaskHost, value);
		}

		protected dynamic TaskHost { get; }
	}
}
