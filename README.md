# TabularEditor
Tabular Editor is a tool that lets you easily manipulate and manage measures, calculated columns, display folders, perspectives and translations in SQL Server Analysis Services Tabular Models (from Compatibility Level 1200 and onwards). The tool is written entirely in .NET WinForms (C#).

# Command Line usage
TabularEditor.exe can be executed from the command line for automated deployment purposes. All deployment options that are available through the GUI, are also available through the command line.

Examples:

`TabularEditor.exe`
Opens the Tabular Editor GUI.

`TabularEditor.exe c:\Projects\Model.bim`
Opens the Tabular Editor GUI and loads the specified Model.bim file.

`TabularEditor.exe c:\Projects\Model.bim -deploy localhost AdventureWorks`
Deploys the specified Model.bim file to the SSAS instance running on localhost, overwriting or creating the AdventureWorks database.
