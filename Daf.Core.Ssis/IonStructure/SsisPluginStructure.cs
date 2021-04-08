// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using Daf.Core.Sdk;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1720 // Identifier contains type name
#pragma warning disable CA1724 // Type names should not match namespaces
#pragma warning disable CA2227 // Collection properties should be read only. We likely need to make changes in the Ion parser before we can remove this.
namespace Daf.Core.Ssis.IonStructure
{
	/// <summary>
	/// Root node for SSIS projects.
	/// </summary>
	[IsRootNode]
	public class Ssis
	{
		/// <summary>
		/// Collection of SSIS projects.
		/// </summary>
		public List<SsisProject> SsisProjects { get; set; }
	}

	/// <summary>
	/// An SSIS project holding SSIS packages and other related entities.
	/// </summary>
	public class SsisProject
	{
		/// <summary>
		/// Collection of all the connections in the project.
		/// </summary>
		public List<Connection> Connections { get; set; }

		/// <summary>
		/// Collection of all the SQL Server Integration Services (SSIS) packages in the project.
		/// </summary>
		public List<Package> Packages { get; set; }

		/// <summary>
		/// Collection of all the parameters in the project.
		/// </summary>
		public List<Parameter> Parameters { get; set; }

		/// <summary>
		/// Collection of all the project-wide, package-reusable ScriptProjects in the project.
		/// </summary>
		public List<ScriptProject> ScriptProjects { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The project's protection level
		/// </summary>
		[DefaultValue(ProtectionLevelEnum.DontSaveSensitive)]
		public ProtectionLevelEnum ProtectionLevel { get; set; }

		/// <summary>
		/// The password to use when using password-based encryption. Only applies when the project's ProtectionLevel is either EncryptAllWithPassword or EncryptSensitiveWithPassword.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// The default locale ID for the packages in the project (unless overridden by the package).
		/// </summary>
		[DefaultValue(0)]
		public int LocaleId { get; set; }

		/// <summary>
		/// The target SQL Server version.
		/// </summary>
		[DefaultValue(TargetSqlServerVersionEnum.SqlServer2019)]
		public TargetSqlServerVersionEnum TargetSqlServerVersion { get; set; }
	}

	/// <summary>
	/// Element in collection of all the connections in the project.
	/// </summary>
	public class Connection
	{
		/// <summary>
		/// Collection of SSIS expressions for property value overrides.
		/// </summary>
		public List<PropertyExpression> PropertyExpressions { get; set; }

		/// <summary>
		/// The connection string to use.
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Pre-defined GUID for the connection. Use this if you want the connection to work with incremental builds, or if you want to be able to deploy packages individually.
		/// </summary>
		public string GUID { get; set; }

		/// <summary>
		/// Validation should be delayed until runtime.
		/// </summary>
		[DefaultValue(false)]
		public bool DelayValidation { get; set; }
	}

	/// <summary>
	/// Element in collection of SSIS expressions for property value overrides.
	/// </summary>
	public class PropertyExpression
	{
		/// <summary>
		/// The target property to override.
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// Element in collection of all the SQL Server Integration Services (SSIS) packages in the project.
	/// </summary>
	public class Package
	{
		/// <summary>
		/// Collection of all the connections in the package.
		/// </summary>
		public List<Connection> Connections { get; set; }

		/// <summary>
		/// Collection of all event handlers in the package.
		/// </summary>
		public List<EventHandler> EventHandlers { get; set; }

		/// <summary>
		/// Collection of all the parameters in the package.
		/// </summary>
		public List<Parameter> Parameters { get; set; }

		/// <summary>
		/// Collection of all the tasks in the package.
		/// </summary>
		public List<Task> Tasks { get; set; }

		/// <summary>
		/// Collection of all root-level variables in the package.
		/// </summary>
		public List<Variable> Variables { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The locale ID for the package. Uses the building operating system's regional settings if left empty, or the project-level LocaleId if it's set.
		/// </summary>
		[DefaultValue(0)]
		public int LocaleId { get; set; }

		/// <summary>
		/// Validation should be delayed until runtime.
		/// </summary>
		[DefaultValue(false)]
		public bool DelayValidation { get; set; }
	}

	/// <summary>
	/// Element in collection of all event handlers in the package.
	/// </summary>
	public class EventHandler
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of event to handle.
		/// </summary>
		public EventTypeEnum EventType { get; set; }
	}

	public enum EventTypeEnum
	{
		OnError,
		OnExecStatusChanged,
		OnInformation,
		OnPostExecute,
		OnPostValidate,
		OnPreExecute,
		OnPreValidate,
		OnProgress,
		OnQueryCancel,
		OnTaskFailed,
		OnVariableValueChanged,
		OnWarning,
	}

	/// <summary>
	/// Element in collection of all the parameters in the project.
	/// </summary>
	public class Parameter
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The data type of the parameter.
		/// </summary>
		public TypeCode DataType { get; set; }

		/// <summary>
		/// This parameter requires a value to be set for each execution.
		/// </summary>
		[DefaultValue(false)]
		public bool IsRequired { get; set; }

		/// <summary>
		/// The parameter's value.
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// Element in collection of child tasks.
	/// </summary>
	public abstract class Task
	{
		/// <summary>
		/// Collection of precedence constraints.
		/// </summary>
		public PrecedenceConstraintList PrecedenceConstraints { get; set; }

		/// <summary>
		/// Collection of SSIS expressions for property value overrides.
		/// </summary>
		public List<PropertyExpression> PropertyExpressions { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Validation should be delayed until runtime.
		/// </summary>
		[DefaultValue(false)]
		public bool DelayValidation { get; set; }

		/// <summary>
		/// The ForceExecutionResult property of the object.
		/// </summary>
		[DefaultValue(ForceExecutionResultEnum.None)]
		public ForceExecutionResultEnum ForceExecutionResult { get; set; }

		/// <summary>
		/// If false, an OnError EventHandler with Propagate = false is created for this task.
		/// </summary>
		[DefaultValue(true)]
		public bool PropagateErrors { get; set; }
	}

	/// <summary>
	/// Element in collection of precedence constraints.
	/// </summary>
	public class PrecedenceConstraintList
	{
		/// <summary>
		/// Collection of precedence constraint input paths.
		/// </summary>
		public List<InputPath> Inputs { get; set; }

		/// <summary>
		/// Is this an "And" or an "Or" precedence constraint list?
		/// </summary>
		[DefaultValue(LogicalOperationEnum.And)]
		public LogicalOperationEnum LogicalType { get; set; }
	}

	/// <summary>
	/// Element in collection of precedence constraint input paths.
	/// </summary>
	public class InputPath
	{
		/// <summary>
		/// The output path to which this input path is connected.
		/// </summary>
		public string OutputPathName { get; set; }

		/// <summary>
		/// The evaluation operation.
		/// </summary>
		[DefaultValue(TaskEvaluationOperationTypeEnum.Constraint)]
		public TaskEvaluationOperationTypeEnum EvaluationOperation { get; set; }

		/// <summary>
		/// The evaluation value.
		/// </summary>
		[DefaultValue(TaskEvaluationOperationValueEnum.Success)]
		public TaskEvaluationOperationValueEnum EvaluationValue { get; set; }

		/// <summary>
		/// The SSIS expression that must evaluate to "True" for execution to continue along this path
		/// </summary>
		public string Expression { get; set; }
	}

	public enum TaskEvaluationOperationTypeEnum
	{
		Constraint,
		Expression,
		ExpressionAndConstraint,
		ExpressionOrConstraint,
	}

	public enum TaskEvaluationOperationValueEnum
	{
		Success,
		Failure,
		Completion,
	}

	public enum LogicalOperationEnum
	{
		And,
		Or,
	}

	public enum ForceExecutionResultEnum
	{
		None,
		Success,
		Failure,
		Completion,
	}

	/// <summary>
	/// Element in collection of all root-level variables in the package.
	/// </summary>
	public class Variable
	{
		/// <summary>
		/// The namespace of the variable.
		/// </summary>
		[DefaultValue("User")]
		public string Namespace { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The data type of the variable.
		/// </summary>
		public TypeCode DataType { get; set; }

		/// <summary>
		/// Initial variable value is read-only.
		/// </summary>
		[DefaultValue(false)]
		public bool ReadOnly { get; set; }

		/// <summary>
		/// The variable's value should be evaluated as an expression.
		/// </summary>
		[DefaultValue(false)]
		public bool EvaluateAsExpression { get; set; }

		/// <summary>
		/// The variable's value.
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// Element in collection of all the project-wide, package-reusable ScriptProjects in the project.
	/// </summary>
	public class ScriptProject
	{
		/// <summary>
		/// Collection of C#/VB code files to be included in the script project.
		/// </summary>
		public List<File> Files { get; set; }

		/// <summary>
		/// Collection SSIS variable references that should be accessible to the ScriptProject as read-only variables.
		/// </summary>
		public List<ScriptVariable> ReadOnlyVariables { get; set; }

		/// <summary>
		/// Collection SSIS variable references that should be accessible to the ScriptProject as read-write variables.
		/// </summary>
		public List<ScriptVariable> ReadWriteVariables { get; set; }

		/// <summary>
		/// Collection of assembly references to be imported into the script project.
		/// </summary>
		public List<AssemblyReference> AssemblyReferences { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Element in collection of C#/VB code files to be included in the script project.
	/// </summary>
	public class File
	{
		/// <summary>
		/// The relative path of the code file within the script project.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// The content of the generated script file.
		/// </summary>
		public string Content { get; set; }
	}

	/// <summary>
	/// Element in collection SSIS variable references that should be accessible to the ScriptProject as read-only variables.
	/// </summary>
	public class ScriptVariable
	{
		/// <summary>
		/// The SSIS namespace of the variable.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// The name of the SSIS variable.
		/// </summary>
		public string VariableName { get; set; }
	}

	/// <summary>
	/// Element in collection of assembly references to be imported into the script project.
	/// </summary>
	public class AssemblyReference
	{
		/// <summary>
		/// The path of the assembly to import.
		/// </summary>
		public string AssemblyPath { get; set; }
	}

	public enum ProtectionLevelEnum
	{
		DontSaveSensitive,
		EncryptAllWithPassword,
		EncryptSensitiveWithPassword,
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Doesn't apply here.")]
	public enum TargetSqlServerVersionEnum
	{
		SqlServer2016 = 13,
		SqlServer2017 = 14,
		SqlServer2019 = 15,
	}

	/// <summary>
	/// 
	/// </summary>
	public class CustomConnection : Connection
	{
		/// <summary>
		/// The CreationName of the custom connection.
		/// </summary>
		public string CreationName { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class OleDbConnection : Connection
	{
	}

	/// <summary>
	/// 
	/// </summary>
	public class FlatFileConnection : Connection
	{
		/// <summary>
		/// Collection of flat file columns.
		/// </summary>
		public List<FlatFileColumn> FlatFileColumns { get; set; }

		/// <summary>
		/// The file's format (FixedWidth, Delimited or RaggedRight).
		/// </summary>
		[DefaultValue(FlatFileFormatEnum.Delimited)]
		public FlatFileFormatEnum Format { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true)]
		public bool ColumnNamesInFirstDataRow { get; set; }

		/// <summary>
		/// The number of rows to skip before reading data.
		/// </summary>
		[DefaultValue(0)]
		public int HeaderRowsToSkip { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true)]
		public bool Unicode { get; set; }

		/// <summary>
		/// The encoding of the file.
		/// </summary>
		[DefaultValue(65001)]
		public int CodePage { get; set; }

		/// <summary>
		/// The locale ID of the file. Uses the building operating system's regional settings if left empty.
		/// </summary>
		[DefaultValue(0)]
		public int LocaleId { get; set; }

		/// <summary>
		/// The text qualifier of the file.
		/// </summary>
		public string TextQualifier { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class SequenceContainer : Task
	{
		/// <summary>
		/// Collection of child tasks.
		/// </summary>
		public List<Task> Tasks { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class DataFlow : Task
	{
		/// <summary>
		/// Collection of dataflow components.
		/// </summary>
		public List<Component> Components { get; set; }

		/// <summary>
		/// Indicates that the buffer size should be auto-adjusted at runtime. Makes DefaultBufferSize irrelevant.
		/// </summary>
		[DefaultValue(false)]
		public bool AutoAdjustBufferSize { get; set; }

		/// <summary>
		/// The maximum number of rows to store in the row buffer.
		/// </summary>
		[DefaultValue("10000")]
		public string DefaultBufferMaxRows { get; set; }

		/// <summary>
		/// The size of the row buffer.
		/// </summary>
		[DefaultValue("10485760")]
		public string DefaultBufferSize { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class ExecuteProcess : Task
	{
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true)]
		public bool RequireFullFileName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Executable { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Arguments { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string WorkingDirectory { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string StandardInputVariableName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string StandardOutputVariableName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string StandardErrorVariableName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true)]
		public bool FailTaskIfReturnCodeIsNotSuccessValue { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(0)]
		public int SuccessValue { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(0)]
		public int TimeOut { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(true)]
		public bool TerminateProcessAfterTimeOut { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(WindowStyleEnum.Normal)]
		public WindowStyleEnum WindowStyle { get; set; }
	}

	public class SqlStatement
	{
		public string Value { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class ExecuteSql : Task
	{
		/// <summary>
		/// The SQL statement to execute.
		/// </summary>
		public SqlStatement SqlStatement { get; set; }

		/// <summary>
		/// Collection of result-to-variable mappings for SQL query results.
		/// </summary>
		public List<Result> Results { get; set; }

		/// <summary>
		/// Collection of variable-to-parameter mappings for SQL query parameters.
		/// </summary>
		public List<SqlParameter> SqlParameters { get; set; }

		/// <summary>
		/// Specifies the connection used to access the data.
		/// </summary>
		public string ConnectionName { get; set; }

		/// <summary>
		/// Specifies the format of the query results.
		/// </summary>
		[DefaultValue(ExecuteSqlResultSetEnum.None)]
		public ExecuteSqlResultSetEnum ResultSet { get; set; }

		/// <summary>
		/// Specifies how the task will perform value to variable type conversions.
		/// </summary>
		[DefaultValue(ExecuteSqlTypeConversionModeEnum.Allowed)]
		public ExecuteSqlTypeConversionModeEnum TypeConversionMode { get; set; }

		/// <summary>
		/// Indicates wheter the task should prepare the query before executing it.
		/// </summary>
		[DefaultValue(true)]
		public bool BypassPrepare { get; set; }

		/// <summary>
		/// Specifies the time-out value.
		/// </summary>
		[DefaultValue(0U)]
		public uint TimeOut { get; set; }

		/// <summary>
		/// Specifies the code page value.
		/// </summary>
		[DefaultValue(1252U)]
		public uint CodePage { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class Expression : Task
	{
		/// <summary>
		/// Expression.
		/// </summary>
		public string ExpressionValue { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class Script : Task
	{
		/// <summary>
		/// Script project, specifically compiled for this task.
		/// </summary>
		public ScriptProject ScriptProject { get; set; }

		/// <summary>
		/// Reference to a project-wide ("global") ScriptProject, located in the parent Project's ScriptProjects list and only compiled once.
		/// </summary>
		public ScriptProjectReference ScriptProjectReference { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class ForEachFromVariableLoopContainer : SequenceContainer
	{
		/// <summary>
		/// Maps fields in the collection to variables, by index.
		/// </summary>
		public List<VariableIndexMapping> ForEachFromVariableMappings { get; set; }

		/// <summary>
		/// Name of the collection variable to loop over.
		/// </summary>
		public string CollectionVariableName { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class ForLoopContainer : SequenceContainer
	{
		/// <summary>
		/// An expression to increment or decrement the loop counter.
		/// </summary>
		public string AssignExpression { get; set; }

		/// <summary>
		/// An expression to test whether the loop should continue or stop looping.
		/// </summary>
		public string EvalExpression { get; set; }

		/// <summary>
		/// An expression to initialize the loop counter.
		/// </summary>
		public string InitExpression { get; set; }
	}

	/// <summary>
	/// Element in collection of flat file columns.
	/// </summary>
	public class FlatFileColumn
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The column's data type.
		/// </summary>
		[DefaultValue(DatabaseTypeEnum.Int32)]
		public DatabaseTypeEnum DataType { get; set; }

		/// <summary>
		/// The width of the column in the source file.
		/// </summary>
		[DefaultValue(0)]
		public int InputWidth { get; set; }

		/// <summary>
		/// The width of the column in the data flow.
		/// </summary>
		[DefaultValue(0)]
		public int OutputWidth { get; set; }

		/// <summary>
		/// The precision of this column's data type. This only applies to data type definitions that accept a precision parameter, such as Decimal.
		/// </summary>
		[DefaultValue(0)]
		public int Precision { get; set; }

		/// <summary>
		/// The scale of this column's data type. This only applies to data type definitions that accept a scale parameter, such as Decimal.
		/// </summary>
		[DefaultValue(0)]
		public int Scale { get; set; }

		/// <summary>
		/// The delimiter of the column.
		/// </summary>
		[DefaultValue(",")]
		public string Delimiter { get; set; }

		/// <summary>
		/// The codepage of the column. This only applies to string-type columns.
		/// </summary>
		[DefaultValue(0)]
		public int CodePage { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(false)]
		public bool TextQualified { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(false)]
		public bool FastParse { get; set; }
	}

	public enum DatabaseTypeEnum
	{
		AnsiString,
		AnsiStringFixedLength,
		Binary,
		Byte,
		Boolean,
		Currency,
		Date,
		DateTime,
		DateTime2,
		DateTimeOffset,
		Decimal,
		Double,
		Guid,
		Int16,
		Int32,
		Int64,
		Object,
		SByte,
		Single,
		String,
		StringFixedLength,
		Time,
		UInt16,
		UInt32,
		UInt64,
		VarNumeric,
		Xml,
	}

	public enum FlatFileFormatEnum
	{
		Delimited,
		FixedWidth,
		RaggedRight,
	}

	/// <summary>
	/// Element in collection of dataflow components.
	/// </summary>
	public class Component
	{
		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }
	}

	public enum WindowStyleEnum
	{
		Normal,
		Hidden,
		Minimized,
		Maximized,
	}

	/// <summary>
	/// Element in collection of result-to-variable mappings for SQL query results.
	/// </summary>
	public class Result
	{
		/// <summary>
		/// The name of the result to map into a variable.
		/// </summary>
		public string ResultName { get; set; }

		/// <summary>
		/// The variable to map the result into.
		/// </summary>
		public string VariableName { get; set; }
	}

	/// <summary>
	/// Element in collection of variable-to-parameter mappings for SQL query parameters.
	/// </summary>
	public class SqlParameter
	{
		/// <summary>
		/// The variable to map into the specified query parameter.
		/// </summary>
		public string VariableName { get; set; }

		/// <summary>
		/// The name of the query parameter to map the variable name against.
		/// </summary>
		public string ParameterName { get; set; }

		/// <summary>
		/// The data type of the query parameter.
		/// </summary>
		public DatabaseTypeEnum DataType { get; set; }

		/// <summary>
		/// The direction of the query parameter.
		/// </summary>
		[DefaultValue(ParameterDirectionEnum.Input)]
		public ParameterDirectionEnum Direction { get; set; }

		/// <summary>
		/// The size (length) of the mapped parameter value. Only relevant when used with an applicable data type (string, binary etc).
		/// </summary>
		[DefaultValue("0")]
		public string Size { get; set; }
	}

	public enum ParameterDirectionEnum
	{
		Input,
		Output,
		ReturnValue
	}

	// Match int values to SSIS API.
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Doesn't apply here.")]
	public enum ExecuteSqlResultSetEnum
	{
		None = 1,
		SingleRow = 2,
		Full = 3,
		Xml = 4
	}

	public enum ExecuteSqlTypeConversionModeEnum
	{
		None,
		Allowed
	}

	/// <summary>
	/// Reference to a project-wide ("global") ScriptProject, located in the parent Project's ScriptProjects list and only compiled once.
	/// </summary>
	public class ScriptProjectReference
	{
		/// <summary>
		/// The name of the project-wide ("global") ScriptProject.
		/// </summary>
		public string ScriptProjectName { get; set; }
	}

	/// <summary>
	/// Maps fields in the collection to variables, by index.
	/// </summary>
	public class VariableIndexMapping
	{
		/// <summary>
		/// Index in the collection.
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Name of the variable to map to.
		/// </summary>
		public string VariableName { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class DerivedColumn : Component
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public DataflowInputPath DataFlowInputPath { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public List<DerivedColumnEntry> DerivedColumns { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class RowCount : Component
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public DataflowInputPath DataFlowInputPath { get; set; }

		/// <summary>
		/// The integer variable where the row count will be stored.
		/// </summary>
		public string VariableName { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class CustomComponent : Component
	{
		/// <summary>
		/// Collection of custom properties for the custom component.
		/// </summary>
		public List<CustomProperty> CustomProperties { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public DataflowInputPath DataFlowInputPath { get; set; }

		/// <summary>
		/// The ComponentTypeName of the custom component.
		/// </summary>
		public string ComponentTypeName { get; set; }

		/// <summary>
		/// The connection used to retrieve data.
		/// </summary>
		public string ConnectionName { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class OleDbDestination : Component
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public DataflowInputPath DataFlowInputPath { get; set; }

		/// <summary>
		/// Description missing. Columns that aren't listed will be mapped implicitly based on name.
		/// </summary>
		public List<DataFlowColumnMapping> OleDbDestinationColumns { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public ExternalTable ExternalTableOutput { get; set; }

		/// <summary>
		/// Description missing. This can be overridden at the column level.
		/// </summary>
		public ComponentErrorHandling ErrorHandling { get; set; }

		/// <summary>
		/// The OLE DB connection used to insert data.
		/// </summary>
		public string ConnectionName { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(true)]
		public bool UseFastLoadIfAvailable { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(false)]
		public bool KeepIdentity { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(false)]
		public bool KeepNulls { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(true)]
		public bool TableLock { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(true)]
		public bool CheckConstraints { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue("2147483647")]
		public string MaximumInsertCommitSize { get; set; }
	}

	public class SqlCommand
	{
		public string Value { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class OleDbSource : Component
	{
		/// <summary>
		/// The SQL command to execute.
		/// </summary>
		public SqlCommand SqlCommand { get; set; }

		/// <summary>
		/// Description missing. Columns that aren't listed will be mapped implicitly based on name.
		/// </summary>
		public List<DataFlowColumnMapping> OleDbSourceColumns { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public VariableResource VariableInput { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public List<VariableParameterMapping> VariableParameterMappings { get; set; }

		/// <summary>
		/// The OLE DB connection used to retrieve data.
		/// </summary>
		public string ConnectionName { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class FlatFileDestination : Component
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public DataflowInputPath DataFlowInputPath { get; set; }

		/// <summary>
		/// Description missing. Columns that aren't listed will be mapped implicitly based on name.
		/// </summary>
		public List<DataFlowColumnMapping> FlatFileDestinationColumns { get; set; }

		/// <summary>
		/// The header to write to the top of the file.
		/// </summary>
		public string Header { get; set; }

		/// <summary>
		/// Description missing. This can be overridden at the column level.
		/// </summary>
		public ComponentErrorHandling ErrorHandling { get; set; }

		/// <summary>
		/// The Flat File connection used to insert data.
		/// </summary>
		public string ConnectionName { get; set; }

		/// <summary>
		/// Overwrite data in the target file.
		/// </summary>
		[DefaultValue(true)]
		public bool Overwrite { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class FlatFileSource : Component
	{
		/// <summary>
		/// Description missing. Columns that aren't listed will be mapped implicitly based on name.
		/// </summary>
		public List<DataFlowColumnMapping> FlatFileSourceColumns { get; set; }

		/// <summary>
		/// The Flat File connection used to retrieve data.
		/// </summary>
		public string ConnectionName { get; set; }

		/// <summary>
		/// Retain empty values from the source as null values in the data flow.
		/// </summary>
		[DefaultValue(false)]
		public bool RetainNulls { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class DataflowInputPath
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public string OutputPathName { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class DerivedColumnEntry
	{
		/// <summary>
		/// The output name of the derived column.
		/// </summary>
		public string ColumnName { get; set; }

		/// <summary>
		/// The expression that defines the derived column.
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// The derived column replaces an existing column. If true, ColumnName must match an existing column and data type will not be modified regardless of expression value.
		/// </summary>
		[DefaultValue(false)]
		public bool Replace { get; set; }

		/// <summary>
		/// Row disposition if general error occur.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum ErrorRowDisposition { get; set; }

		/// <summary>
		/// Row disposition if general truncation occur.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum TruncationRowDisposition { get; set; }
	}

	public enum RowDispositionEnum
	{
		FailComponent,
		IgnoreFailure,
		RedirectRow,
		NotUsed,
	}

	/// <summary>
	/// Element in collection of custom properties for the custom component.
	/// </summary>
	public class CustomProperty
	{
		/// <summary>
		/// The name of the custom property.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The custom property's data type. !!!NOT IMPLEMENTED!!!
		/// </summary>
		public string DataType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// Description missing. Columns that aren't listed will be mapped implicitly based on name.
	/// </summary>
	public class DataFlowColumnMapping
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public string SourceColumn { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public string TargetColumn { get; set; }

		/// <summary>
		/// Description missing. !!!NOT IMPLEMENTED!!!
		/// </summary>
		[DefaultValue(true)]
		public bool IsUsed { get; set; }

		/// <summary>
		/// Description missing. !!!NOT IMPLEMENTED!!!
		/// </summary>
		[DefaultValue("0")]
		public string SortKeyPosition { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class ExternalTable
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public string Table { get; set; }
	}

	/// <summary>
	/// Description missing. This can be overridden at the column level.
	/// </summary>
	public class ComponentErrorHandling
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public List<OutputErrorHandling> Outputs { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public List<InputErrorHandling> ErrorInputs { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum ErrorRowDisposition { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum TruncationRowDisposition { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class OutputErrorHandling
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public List<ColumnErrorHandling> OutputErrorHandlingColumns { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public string OutputName { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum ErrorRowDisposition { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum TruncationRowDisposition { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class ColumnErrorHandling
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public string ColumnName { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum ErrorRowDisposition { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum TruncationRowDisposition { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class InputErrorHandling
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public List<ColumnErrorHandling> InputErrorHandlingColumns { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public string InputName { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum ErrorRowDisposition { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		[DefaultValue(RowDispositionEnum.FailComponent)]
		public RowDispositionEnum TruncationRowDisposition { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class VariableResource
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public string VariableName { get; set; }
	}

	/// <summary>
	/// Description missing.
	/// </summary>
	public class VariableParameterMapping
	{
		/// <summary>
		/// Description missing.
		/// </summary>
		public string VariableName { get; set; }

		/// <summary>
		/// Description missing.
		/// </summary>
		public string ParameterName { get; set; }
	}
}
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1720 // Identifier contains type name
#pragma warning restore CA1724 // Type names should not match namespaces
#pragma warning restore CA2227 // Collection properties should be read only. We likely need to make changes in the Ion parser before we can remove this.
