// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Ssis.IonStructure;
using Daf.Core.Ssis.Wrapper.Wrappers.ConnectionManagers;

namespace Daf.Core.Ssis.ConnectionManagers
{
	internal class FlatFileConnectionManager
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "We don't have a reason to save the results yet.")]
		public static FlatFileConnectionManagerWrapper CreateConnectionManager(IConnectionManagerContainer connectionManagerContainer, FlatFileConnection flatFileConnection)
		{
			FlatFileConnectionManagerWrapper connectionManagerWrapper = new FlatFileConnectionManagerWrapper(connectionManagerContainer, flatFileConnection.Name, flatFileConnection.ConnectionString, flatFileConnection.GUID);

			Dictionary<string, object> properties = GetProperties(flatFileConnection);
			connectionManagerWrapper.SetProperties(properties);

			foreach (IonStructure.FlatFileColumn column in flatFileConnection.FlatFileColumns)
			{
				new FlatFileColumn(connectionManagerWrapper)
				{
					Name = column.Name,
					ColumnType = flatFileConnection.Format == FlatFileFormatEnum.FixedWidth ? "FixedWidth" : "Delimited",
					DataType = column.DataType,
					Delimiter = TranslateDelimiter(column.Delimiter),
					InputWidth = flatFileConnection.Format == FlatFileFormatEnum.FixedWidth ? column.InputWidth : 0,
					OutputWidth = column.OutputWidth,
					Precision = column.Precision,
					Scale = column.Scale,
					TextQualified = column.TextQualified
				};
			}

			return connectionManagerWrapper;
		}

		private static Dictionary<string, object> GetProperties(FlatFileConnection flatFileConnection)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>
			{
				{ "CodePage", flatFileConnection.CodePage },
				{ "ColumnNamesInFirstDataRow", flatFileConnection.ColumnNamesInFirstDataRow },
				{ "HeaderRowsToSkip", flatFileConnection.HeaderRowsToSkip },
				{ "Unicode", flatFileConnection.Unicode }
			};

			switch (flatFileConnection.Format)
			{
				case FlatFileFormatEnum.Delimited:
					properties.Add("Format", "Delimited");
					break;
				case FlatFileFormatEnum.FixedWidth:
					properties.Add("Format", "FixedWidth");
					break;
				case FlatFileFormatEnum.RaggedRight:
					properties.Add("Format", "RaggedRight");
					break;
			}

			if (flatFileConnection.LocaleId != 0)
				properties.Add("LocaleID", flatFileConnection.LocaleId);

			if (flatFileConnection.TextQualifier != null)
				properties.Add("TextQualifier", flatFileConnection.TextQualifier);

			return properties;
		}

		private static string TranslateDelimiter(string delimiter)
		{
			switch (delimiter)
			{
				case "CRLF":
				case "{CR}{LF}":
					return "\r\n";
				case "CR":
				case "{CR}":
					return "\r";
				case "LF":
				case "{LF}":
					return "\n";
			}

			return delimiter;
		}
	}
}
