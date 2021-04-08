// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using Daf.Core.Ssis.Wrapper.Enums;

namespace Daf.Core.Ssis.Wrapper.Wrappers.Containers
{
	public abstract class ForEachLoopContainerWrapper : ContainerWrapper
	{
		private static readonly Type applicationType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.Application");
		private static readonly Type forEachLoopType = AssemblyLoader.ManagedDts.GetType("Microsoft.SqlServer.Dts.Runtime.ForEachLoop");

		// Force this to only run once, takes quite a while.
		private static readonly dynamic forEachEnumeratorInfos = ((dynamic)Activator.CreateInstance(applicationType)).ForEachEnumeratorInfos;

		protected ForEachLoopContainerWrapper(ContainerWrapper containerWrapper, ForEachEnumeratorHost host, bool collectionEnumerator, string moniker) : base(containerWrapper, moniker)
		{
			ForEachLoopContainer = Convert.ChangeType(InnerObject, forEachLoopType);
			ForEachLoopContainer.ForEachEnumerator = forEachEnumeratorInfos[GetName(host)].CreateNew();
			ForEachLoopContainer.ForEachEnumerator.CollectionEnumerator = collectionEnumerator;
		}

		protected dynamic ForEachLoopContainer { get; }

		private static string GetName(ForEachEnumeratorHost host)
		{
			switch (host)
			{
				case ForEachEnumeratorHost.File:
					return "Foreach File Enumerator";
				case ForEachEnumeratorHost.Item:
					return "Foreach Item Enumerator";
				case ForEachEnumeratorHost.ADO:
					return "Foreach ADO Enumerator";
				case ForEachEnumeratorHost.ADONETSchemaRowset:
					return "Foreach ADO.NET Schema Rowset Enumerator";
				case ForEachEnumeratorHost.Variable:
					return "Foreach From Variable Enumerator";
				case ForEachEnumeratorHost.NodeList:
					return "Foreach NodeList Enumerator";
				case ForEachEnumeratorHost.SMO:
					return "Foreach SMO Enumerator";
				case ForEachEnumeratorHost.HDFS:
					return "Foreach HDFS File Enumerator";
				default:
					throw new ArgumentException($"Unexpected foreach enumerator host: {host}");
			}
		}
	}
}
