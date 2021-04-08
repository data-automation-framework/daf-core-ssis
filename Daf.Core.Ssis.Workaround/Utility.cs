// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Reflection;
using Daf.Core.Ssis.IonStructure;

namespace Daf.Core.Ssis
{
	public static class Utility
	{
		// Data type mappings from https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.dts.runtime.wrapper.datatype?view=sqlserver-2019
		public static int MapDataType(DatabaseTypeEnum dataType)
		{
			switch (dataType)
			{
				case DatabaseTypeEnum.AnsiString:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_STR");
				case DatabaseTypeEnum.AnsiStringFixedLength:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_STR");
				case DatabaseTypeEnum.Binary:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_BYTES");
				case DatabaseTypeEnum.Byte:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_UI1");
				case DatabaseTypeEnum.Boolean:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_BOOL");
				case DatabaseTypeEnum.Currency:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_CY");
				case DatabaseTypeEnum.Date:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_DBDATE");
				case DatabaseTypeEnum.DateTime:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_DBTIMESTAMP");
				case DatabaseTypeEnum.DateTime2:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_DBTIMESTAMP2");
				case DatabaseTypeEnum.DateTimeOffset:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_DBTIMESTAMPOFFSET");
				case DatabaseTypeEnum.Decimal:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_DECIMAL");
				case DatabaseTypeEnum.Double:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_R8");
				case DatabaseTypeEnum.Guid:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_GUID");
				case DatabaseTypeEnum.Int16:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_I2");
				case DatabaseTypeEnum.Int32:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_I4");
				case DatabaseTypeEnum.Int64:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_I8");
				case DatabaseTypeEnum.SByte:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_I1");
				case DatabaseTypeEnum.Single:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_R4");
				case DatabaseTypeEnum.String:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_WSTR");
				case DatabaseTypeEnum.StringFixedLength:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_WSTR");
				case DatabaseTypeEnum.Time:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_DBTIME2");
				case DatabaseTypeEnum.UInt16:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_UI2");
				case DatabaseTypeEnum.UInt32:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_UI4");
				case DatabaseTypeEnum.UInt64:
					return Wrapper.Utility.GetDataTypeEnumValue("DT_UI8");
				case DatabaseTypeEnum.VarNumeric:
					return 139;
				default:
					throw new ArgumentException($"Data type {dataType} is invalid!");
			}
		}

		public static object ParseValue(string value, TypeCode dataType)
		{
			switch (dataType)
			{
				case TypeCode.DBNull:
				case TypeCode.Empty:
					throw new InvalidOperationException($"Data type {dataType} is invalid!");
				case TypeCode.Object:
					return new object(); // Return a generic object.
				case TypeCode.String:
					if (value == null)
						return string.Empty; // SSIS doesn't like strings being null, return an empty string.

					return value;
				default:
					break;
			}

			if (value == null)
				throw new ArgumentNullException($"Parameter or variable of data type {dataType} can't be null!");

			// If we made it all the way through to here, we need to use reflection in order to return an object of the correct type.
			MethodInfo method = Type.GetType($"System.{dataType}").GetMethod
				(
					"Parse",
					BindingFlags.Static | BindingFlags.Public,
					null,
					new Type[] { value.GetType() },
					null
				);

			return method.Invoke(null, new object[] { value });
		}
	}
}
