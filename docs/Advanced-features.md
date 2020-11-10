# Advanced Features

In addition to the features mentioned in the [Features at a glance](/Features-at-a-glance) article, Tabular Editor also supports the following features for advanced usage.

## Replace tables
As of version 2.7, you can now replace a table simply by copying (CTRL+C) one table - even from another instance of Tabular Editor - and then selecting the table you want to replace, before hitting paste (CTRL+V). A prompt will ask you to confirm whether you really want to replace the table ("Yes"), insert as a new table ("No") or cancel the operation entirely:

![image](https://user-images.githubusercontent.com/8976200/36545892-40983114-17ea-11e8-8825-e8de6fd4e284.png)

If you choose "Yes", the selected table will be replaced with the table in the clipboard. Furthermore, all relationships pointing to or from that table will be updated to use the new table. For this to work, columns participating in relationships must have the same name and data type in both the original table, and the inserted table.

## Roles and Row-Level Security
As of version 2.1, Roles are now visible in the Explorer Tree. You can right-click the tree to create new roles, delete or duplicate existing roles. You can view and edit the members of each role, by locating the role in the Explorer Tree, and navigating to the "Role Members" property in the Property Grid. Note that when deploying, the [Deployment Wizard](/Advanced-features#deployment-wizard) does not deploy role members by default.

The biggest advantage of working with Roles through Tabular Editor, is that each Table object has a "Row Level Filters" property, which lets you view and edit the filters defined on that table, across all roles:

![](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/RLSTableContext.png)

Of course, you can also view the filters across all tables in one particular role, similar to the UI of SSMS or Visual Studio:

![](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/RLSRoleContext.png)

## View Table Partitions
TODO
## DAX Expression Editor
TODO
## Script Editor
TODO (For now, please view [this article](/Advanced-Scripting))
## Scripting/referencing objects
You can use drag-and-drop functionality, to script out objects in the following ways:

* Drag one or more objects to another Windows application (text editor or SSMS)
JSON code representing the dragged object(s) will be created. When dragging the Model node, a Table, a Role or a Data Source, a "createOrReplace" script is created.

* Dragging an object (measure, column or table) into the DAX expression editor, will insert a fully-qualified DAX-reference to the object in question.

* Dragging an object to the Advanced Script editor, will insert the C# code necessary to access the object through the TOM tree.
## Deployment Wizard
Tabular Editor comes with a deployment wizard that provides a few benefits compared to deploying from SSDT - especially when deploying to an existing database. After choosing a server and a database to deploy to, you have the following options for the deployment at hand:

![Deployment Wizard](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/Deployment.png)

Leaving the "Deploy Connections" box unchecked, will make sure that all the data sources on the target database stay untouched. You will get an error if your model contains one or more tables with a data source, that does not already exist in the target database.

Similarly, leaving out "Deploy Table Partitions", will make sure that existing partitions on your tables are not changed, leaving the data in the partitions intact.

When the "Deploy Roles" box is checked, the roles in the target database will be updated to reflect what you have in the loaded model, however if the "Deploy Role Members" is unchecked, the members of each role will be unchanged in the target database.

## Metadata Backup
If you wish, Tabular Editor can automatically save a backup copy of the existing model metadata, prior to each save (when connected to an existing database) or deployment. This is useful if you're not using Version Control software, but still need to rollback to a previous version of your model.

To enable this setting, go to "File" > "Preferences", enable the checkbox and choose a folder to place the metadata backups:

<img src="https://user-images.githubusercontent.com/8976200/91543926-3de69100-e91f-11ea-88de-3def2b97eae0.png" width="300" />

If the setting is enabled, a compressed (zipped) version of the existing model metadata will be saved to this location whenever you use the Deployment Wizard, or when you click the "Save" button while connected to a (workspace) database.

## Formula Fix-up and Formula Dependencies
Tabular Editor continuously parses the DAX expressions of all measures, calculated columns and calculated tables in your model, to construct a dependency tree of these objects. This dependency tree is used for the Formula Fix-up functionality, which may be enabled under "File" > "Preferences". Formula Fix-up automatically updates the DAX expression of any measure, calculated column or calculated table, whenever an object that was referenced in the expression is renamed.

To visualize the dependency tree, right-click the object in the explorer tree and choose "Show dependencies..."

![image](https://cloud.githubusercontent.com/assets/8976200/22482528/b37d27e2-e7f9-11e6-8b89-c503f9fffcac.png)

## Import/Export Translations
Select one or more cultures in the Explorer Tree, right-click and choose "Export Translations..." to generate a .json file that can be imported later in either Tabular Editor or Visual Studio. Choose "Import Translations..." to import a corresponding .json file. You can choose whether to overwrite existing translations. If you don't, translations defined in the .json file will only be applied to objects that do not already have a translation for the given culture.

## Folder Serialization
This feature allows you to more easily integrate your SSAS Tabular Models in a file-based source control environment such as TFS, SubVersion or Git. By choosing "File" > "Save to Folder...", Tabular Editor will deconstruct the Model.bim file and save its content as separate files in a folder structure similar to the structure of the JSON within the Model.bim. When subsequently saving the model, only files with changed metadata will be touched, meaning most version control software can easily detect which changes have been done to the model, making source merging and conflict handling a lot easier, than when working with a single Model.bim file.

![image](https://cloud.githubusercontent.com/assets/8976200/22483167/5e07ad52-e7fc-11e6-890f-5c0d20fff0cb.png)

By default, objects are serialized down to the lowest object level (meaning measures, columns and hierarchies are stored as individual .json files).

Additionally, Tabular Editor's [command-line syntax](/Command-line-Options) supports loading a model from this folder structure and deploying it directly to a database, making it easy for you to automate builds for continuous integration workflows.

If you want to customize the granularity at which metadata is saved to individual files, go to File > Preferences and click the "Save to folder"-tab. Here, it's possible to toggle some serialization options which are passed to the TOM when serializing into JSON. Furthermore, you can check/uncheck the types of objects for which individual files will be generated. In some Version Control scenarios, you might want to store everything related to one table in a file on its own, where as in other scenarios you may need individual files for columns and measures.

These settings are saved in an annotation on the model, the first time you use the Save to Folder function, so that the settings are reused when the model is loaded and the "Save"-button is subsequently clicked. If you want to apply new settings, use "File > Save to Folder..." again.

<img src="https://cloud.githubusercontent.com/assets/8976200/25333606/30578a78-28eb-11e7-9885-0fc66f5e4046.png" width="300" />

## User Settings Files

When Tabular Editor is executed, it writes some additional files to the disk at various locations. What follows is a description of these files and their content:

### In %ProgramData%\TabularEditor

- **BPARules.json** Best Practice Analyzer rules that are available to all users.
- **TOMWrapper.dll** This file is used when executing scripts inside Tabular Editor. You can also reference the .dll in your own .NET projects, to utilise the wrapper code. If you are having issues executing advanced scripts after upgrading Tabular Editor, please delete this file and restart Tabular Editor.
- **Preferences.json** This file stores all preferences set in the File > Preferences dialog.

### In %AppData%\Local\TabularEditor

- **BPARules.json** Best Practice Analyzer rules that are available only to the current user.
- **CustomActions.json** Custom script actions that can be invoked from the right-click menu or the Tools-menu of the Explorer Tree. These actions can be created on the Advanced Script Editor tab.
- **RecentFiles.json** Stores a list of recently opened .bim files. The last most 10 items in this list is displayed in the File > Recent Files menu.
- **RecentServers.json** Stores a list of recently accessed server names. These are displayed in the dropdown portion of the "Connect to Database" dialog box and in the Deployment Wizard.