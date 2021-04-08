// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Linq.Expressions;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;

namespace Daf.Core.Ssis.Wrapper.Wrappers
{
	public class FlatFileColumnWrapper
	{
		private static readonly Type IDtsNameType = AssemblyLoader.DtsRuntimeWrap.GetType("Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSName100");

		public FlatFileColumnWrapper(FlatFileConnectionManagerWrapper flatFileConnectionManagerWrapper)
		{
			if (flatFileConnectionManagerWrapper == null)
				throw new ArgumentNullException(nameof(flatFileConnectionManagerWrapper));

			FlatFileColumn = flatFileConnectionManagerWrapper.Columns.Add();
		}

		public string ColumnDelimiter { get { return FlatFileColumn.ColumnDelimiter; } set { FlatFileColumn.ColumnDelimiter = value; } }

		public string ColumnType { get { return FlatFileColumn.ColumnType; } set { FlatFileColumn.ColumnType = value; } }

		public int ColumnWidth { get { return FlatFileColumn.ColumnWidth; } set { FlatFileColumn.ColumnWidth = value; } }

		public int DataType { get { return FlatFileColumn.DataType; } set { FlatFileColumn.DataType = value; } }

		public int DataPrecision { get { return FlatFileColumn.DataPrecision; } set { FlatFileColumn.DataPrecision = value; } }

		public int DataScale { get { return FlatFileColumn.DataScale; } set { FlatFileColumn.DataScale = value; } }

		public int MaximumWidth { get { return FlatFileColumn.MaximumWidth; } set { FlatFileColumn.MaximumWidth = value; } }

		public string Name
		{
			set
			{
				dynamic IDtsName = ReflectionCast(IDtsNameType, FlatFileColumn);

				IDtsNameType.GetProperty("Name").SetValue(IDtsName, value, null);
			}
		}

		public bool TextQualified { get { return FlatFileColumn.TextQualified; } set { FlatFileColumn.TextQualified = value; } }

		/// <summary>
		/// Cast between classes/interfaces that don't implement IConvertible.
		/// From https://stackoverflow.com/questions/1398796/casting-with-reflection/1398934
		/// </summary>
		/// <param name="type">The type to convert the object to</param>
		/// <param name="obj">The object to convert</param>
		/// <returns>The converted object</returns>
		private static object ReflectionCast(Type type, object obj)
		{
			ParameterExpression dataParameter = Expression.Parameter(typeof(object), "data");
			BlockExpression body = Expression.Block(Expression.Convert(Expression.Convert(dataParameter, obj.GetType()), type));

			Delegate run = Expression.Lambda(body, dataParameter).Compile();
			object convertedObject = run.DynamicInvoke(obj);

			return convertedObject;
		}

		internal dynamic FlatFileColumn { get; }
	}
}
