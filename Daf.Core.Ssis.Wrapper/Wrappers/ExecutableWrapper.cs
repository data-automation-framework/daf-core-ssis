// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Core.Ssis.Wrapper.Wrappers
{
	public class ExecutableWrapper
	{
		private static readonly Type dtsForcedExecResultType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.DTSForcedExecResult");

		protected ExecutableWrapper(ContainerWrapper containerWrapper, string moniker)
		{
			if (containerWrapper == null)
				throw new ArgumentNullException(nameof(containerWrapper));

			try
			{
				InnerObject = containerWrapper.InnerObject.Executables.Add(moniker);
			}
			catch (Exception)
			{
				throw new InvalidOperationException($"Failed to create executable with moniker {moniker}!");
			}

			containerWrapper.ExecutableWrappers.Add(this);
		}

		protected ExecutableWrapper(dynamic innerObject)
		{
			InnerObject = innerObject;
		}

		public bool DelayValidation { get { return InnerObject.DelayValidation; } set { InnerObject.DelayValidation = value; } }

		public dynamic ForceExecutionResult
		{
			set
			{
				dynamic forceExecutionResult = Enum.Parse(dtsForcedExecResultType, value);
				InnerObject.ForceExecutionResult = Convert.ChangeType(forceExecutionResult, dtsForcedExecResultType);
			}
		}

		public string Name { get { return InnerObject.Name; } set { InnerObject.Name = value; } }

		/// <summary>
		/// Check whether a variable exists in the executable.
		/// </summary>
		/// <param name="qualifiedVariableName">The fully qualified name of the variable</param>
		/// <returns>True if it exists, otherwise false</returns>
		public bool ContainsVariable(string qualifiedVariableName)
		{
			if (InnerObject.Variables.Contains(qualifiedVariableName))
				return true;
			else
				return false;
		}

		internal dynamic InnerObject { get; }
	}
}
