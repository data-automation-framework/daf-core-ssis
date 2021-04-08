# Data Automation Framework - Ssis plugin (Daf Ssis)
**Note: This project is currently in an alpha state and should be considered unstable. Breaking changes to the public API will occur.**

Daf is a plugin-based data and integration automation framework primarily designed to facilitate data warehouse and ETL processes. Developers use this framework to programatically generate data integration objects using the Daf templating language.

This Daf plugin allow users to generate SSIS (SQL Server Integration Services) packages for loading and processing data.

## Installation
The plugin uses the SSIS API to generate packages which is currently Windows only. This plugin is therefore only compatible with Windows machines.

In the daf project file add a new ItemGroup containing a nuget package reference to the plugin:
```
<ItemGroup>
  <PackageReference Include="Daf.Core.Ssis" Version="*" />
<ItemGroup>
```

## Usage
The root node of the SSIS plugin is _Ssis_. This root node must start on the first column in the daf template file.

Use <# #> to inject C# code, <#= #> to get variable string values from the C# code:
![Ssis](https://user-images.githubusercontent.com/1073539/113034479-67f08200-9192-11eb-9c49-35b6d983e4b2.png)

## Links
[Daf organization](https://github.com/data-automation-framework)

[Documentation](https://data-automation-framework.com)
