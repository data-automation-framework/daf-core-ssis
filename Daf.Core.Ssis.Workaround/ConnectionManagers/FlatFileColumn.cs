// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;

namespace Daf.Core.Ssis.ConnectionManagers
{
	public class FlatFileColumn
	{
		public FlatFileColumn(FlatFileConnectionManagerWrapper connectionWrapper)
		{
			FlatFileColumnWrapper = new FlatFileColumnWrapper(connectionWrapper);
		}

		public string ColumnType { get { return FlatFileColumnWrapper.ColumnType; } set { FlatFileColumnWrapper.ColumnType = value; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1044:Properties should not be write only", Justification = "Wrapper doesn't have a setter.")]
		public DatabaseTypeEnum DataType { set { FlatFileColumnWrapper.DataType = Utility.MapDataType(value); } }

		public string Delimiter { get { return FlatFileColumnWrapper.ColumnDelimiter; } set { FlatFileColumnWrapper.ColumnDelimiter = value; } }

		public int InputWidth { get { return FlatFileColumnWrapper.ColumnWidth; } set { FlatFileColumnWrapper.ColumnWidth = value; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1044:Properties should not be write only", Justification = "Wrapper doesn't have a setter.")]
		public string Name { set { FlatFileColumnWrapper.Name = value; } }

		public int OutputWidth { get { return FlatFileColumnWrapper.MaximumWidth; } set { FlatFileColumnWrapper.MaximumWidth = value; } }

		public int Precision { get { return FlatFileColumnWrapper.DataPrecision; } set { FlatFileColumnWrapper.DataPrecision = value; } }

		public int Scale { get { return FlatFileColumnWrapper.DataScale; } set { FlatFileColumnWrapper.DataScale = value; } }

		public bool TextQualified { get { return FlatFileColumnWrapper.TextQualified; } set { FlatFileColumnWrapper.TextQualified = value; } }

		private FlatFileColumnWrapper FlatFileColumnWrapper { get; }
	}
}
