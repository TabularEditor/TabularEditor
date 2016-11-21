# TabularEditor
Tabular Editor is a tool that lets you easily manipulate and manage measures, calculated columns, display folders, perspectives and translations in SQL Server Analysis Services Tabular Models (from Compatibility Level 1200 and onwards). The tool is written entirely in .NET WinForms (C#).

View the article on www.kapacity.dk/tabular-editor for a general presentation of the tool and the motivations behind it.

## Features at a glance
Tabular Editor has a lot of features that makes it easier to work with Tabular Models. The recommended workflow is to set up the tables and relationships using SSDT as normal, and then use Tabular Editor to do the rest. That is: Create calculated columns, measures, hierarchies, perspectives, translations, display folders, and every other kind of fine-tuning you can think of.

![The main UI of Tabular Editor](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/Main%20UI.png)

## Command Line usage
TabularEditor.exe can be executed from the command line for automated deployment purposes. All deployment options that are available through the GUI, are also available through the command line.

### Deployment Examples

`TabularEditor.exe`

Opens the Tabular Editor GUI.

`TabularEditor.exe c:\Projects\Model.bim`

Opens the Tabular Editor GUI and loads the specified Model.bim file.

`TabularEditor.exe c:\Projects\Model.bim -deploy localhost AdventureWorks`

Deploys the specified Model.bim file to the SSAS instance running on localhost, overwriting or creating the AdventureWorks database.

### Note
Since TabularEditor.exe is a Windows Forms application, running it from the command line will execute the application in a different thread, returning control to the caller immediately. This may cause issues when running deployments as part of a batch job where you need to await succesful deployment before proceeding with the job. If you experience these issues, use the following command to execute the deployment:

`start /wait TabularEditor.exe c:\Projects\Model.bim -deploy localhost AdventureWorks`
