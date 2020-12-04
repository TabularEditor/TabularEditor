# Roadmap

* Scripting objects into TMSL or DAX (compatible with DAX Editor)
* IntelliSense in DAX expression editor
* Create plug-in for Visual Studio, to launch Tabular Editor
* Tabular Editor plug-in architecture / public API for developers
* Automated build, test, publishing and documentation using VSTS
* [Done] Formula fix-up (i.e. automatically fixing DAX expressions when renaming objects)
* [Done] UI for showing object dependencies
* [Done] Scripting changes from the command-line
* [Done] Possibility to read/edit more object types (tables, partitions, data columns)
* [Done] Split a Model.bim into multiple json files (for example, one file per table) for better integration into Source Control workflows.
* [Done] Import/export translations

## Scripting objects into TMSL or DAX

It should be possible, when selecting one or more objects in the explorer tree, to generate a script for these objects. In fact, this is already possible by dragging and dropping the objects into another text editor (or SSMS), but there should be a similar right-click option to more clearly communicate to end-users what's going on. It should be possible to generate both TMSL scripts (for SSMS) or DAX-style code, usable in [DAX Editor](https://github.com/DaxEditor/).

Today, measures and calculated columns can be dragged between instances of Tabular Editor to copy them between models, but to better expose this functionality, there should be an UI option for importing a provided piece of TMSL, either from the clipboard or from a file. See [this issue](https://github.com/otykier/TabularEditor/issues/69). Lastly, the standard copy-paste shortcuts should be enabled.

## Create plug-in for Visual Studio, to launch Tabular Editor

A simple context menu extension to Visual Studio, that will simply ensure the Model.bim file is closed and then launch TabularEditor.exe with the Model.bim file loaded.

## IntelliSense in DAX expression editor

When writing DAX code in the expression editor, an autocompletebox should pop-up to help complete table names, column names, measure names or functions (and their arguments).

See also [this issue](https://github.com/otykier/TabularEditor/issues/64).

## Tabular Editor plug-in architecture / public API for developers

People who prefer to script tabular models using C# can already today use the TOMWrapper.dll instead of using the Analysis Services TOM API directly. This provides some benefits, for example, the TOMWrapper namespace makes it easier to work with perspectives and translations, thanks to the convenient methods and properties available.

Taking this one step further, it would be interesting to expose more Tabular Editor functionality to developers:

- Parsing of DAX objects
- Accessing Best Practice Analyzer results
- Tabular Editor UI (making it possible to create "plug-ins" for Tabular Editor with/without custom UI)

## Automated build, test, publishing and documentation using VSTS

DevOps using VSTS and general clean-up of Tabular Editor source code.

## Formula fix-up

When any model object is renamed, all DAX expressions refering that object should be updated to reflect the changed name.

**Update**: As of 2.2, this feature can now be toggled on under "File" > "Preferences".

## UI for showing object dependencies

Right-clicking a measure or calculated column should display a dependency tree in a pop-up dialog. It should be possible to show either objects that depend on the chosen object, or objects on which the chosen object depend.

**Update**: As of 2.2, this feature is available. Simply right-click an object and choose "Show dependencies...".

## Scripting changes from the command-line

Today, it is possible to deploy a model directly from the command-line. Similarly, it should be possible to pipe in a .cs file, containing a C# script to be executed on the model. After script execution, it should be possible to save or deploy the updated model. This requires a few changes to the current command-line options.

**Update**: As of 2.3, scripts can be executed from the command-line, by using the "-S" switch. Deployment works as usual, but if you want to save the modified model as a .bim, you can use the "-B" switch.

## Possibility to read/edit more object types

Tabular Editor currently only lets end-users read and edit a subset of the objects in the Tabular Object Model. It is desirable to allow all objects in the model tree, to be accessible in Tabular Editor: Relationships, KPIs, Calculated Tables and Roles should be directly editable. Data Sources, tables, data columns and table partitions should be editable with some constraints (for example, we should not expect Tabular Editor to be able to fetch data schemas from arbitrary data sources and queries).

**Update**: As of 2.1, many new object types are now visible directly in the Tree Explorer. Using the right-click menu, you can create, duplicate and delete many of these objects (roles, perspectives, translations). We're still lacking support for creating/deleting relationships and data sources, but this will come in a future release.

**Update**: As of 2.2, we can now create and delete relationships. More object types comming later.

**Update**: As of 2.3, tables, partitions and data columns can now be edited. Visual Studio is now only needed to create the blank model itself - everything else can be done in Tabular Editor.

**Update**: Previous update was a lie! I forgot about KPIs - but they can now be created/edited/deleted as of version 2.4.

## Split a Model.bim into multiple json files

The layout and structure of the Model.bim file, makes it horrible for purposes of source control and versioning. Not only is the entire Tabular Object Model written into just one file, the file also contains "ModifiedTime" information everywhere in the structure, making source control DIFF operations useless.

For better release management workflows with Tabular Models, it would be interesting if Tabular Editor could save/load a Model.bim file as a folder structure with individual files for measures, calculated columns, etc. There should be command-line options available for exporting/importing Model.bim files from/to this format, and it should be possible to deploy directly from this format (in cases where you don't need the Model.bim file itself). These individual files should contain the same JSON as the Model.bim file, but without the "ModifiedTime" information, so that they can easily be used in Version Control software, allowing multiple developers to work on the same model at once.

**Update**: [Available in 2.2](/Advanced-features#folder-serialization).

**Update**: As of 2.3, options exist to store Perspective and Translation metadata as annotations on the individual objects. This is useful for source control scenarios with multiple developers, to avoid having single files that gets lots of edits when developers change translations, perspective memberships, etc.

## Power BI Compatibility

Today, it is already possible to connect Tabular Editor to a model hosted by Power BI Desktop. The approach is similar to what is [described here for Excel and SSMS](http://biinsight.com/connect-to-power-bi-desktop-model-from-excel-and-ssms/). Doing this, it is actually possible to add Display Folders to the Power BI Desktop model, and they actually stay in Power BI, even after saving and reopening the .pbix file. However, it seems that there are some compatibility level issues, which should be looked into before proceeding.

**Update**: As of 2.1, Tabular Editor now detects running instances of Power BI Desktop and Visual Studio Integrated Workspaces. You can connect to these instances and make changes as you would normal instances, although this approach of changing Power BI and Integrated Workspace models is not supported by Microsoft.

## Import/Export translations

This is a standard feature in SSDT, which would be useful to have in Tabular Editor as well.

**Update**: [Available in 2.2](/Advanced-features#import-export-translations).
