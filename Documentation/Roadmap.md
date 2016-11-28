# Roadmap

* Scripting objects into TMSL or DAX (compatible with DAX Editor)
* Create plug-in for Visual Studio, to launch Tabular Editor
* IntelliSense in DAX expression editor
* Formula fix-up (i.e. automatically fixing DAX expressions when renaming objects)
* UI for showing object dependencies
* Scripting changes from the command-line
* Possibility to read/edit more object types (tables, partitions, data columns, relationships, roles)
* Split a Model.bim into multiple json files (for example, one file per table) for better integration into Source Control workflows.
* Power BI compatibility


### Scripting objects into TMSL or DAX

It should be possible, when selecting one or more objects in the explorer tree, to generate a script for these objects. In fact, this is already possible by dragging and dropping the objects into another text editor (or SSMS), but there should be a similar right-click option to more clearly communicate to end-users what's going on. It should be possible to generate both TMSL scripts (for SSMS) or DAX-style code, usable in [DAX Editor](https://github.com/DaxEditor/).

### Create plug-in for Visual Studio, to launch Tabular Editor

A simple context menu extension to Visual Studio, that will simply ensure the Model.bim file is closed and then launch TabularEditor.exe with the Model.bim file loaded.

### IntelliSense in DAX expression editor

When writing DAX code in the expression editor, an autocompletebox should pop-up to help complete table names, column names, measure names or functions (and their arguments).

### Formula fix-up

When any model object is renamed, all DAX expressions refering that object should be updated to reflect the changed name.

### UI for showing object dependencies

Right-clicking a measure or calculated column should display a dependency tree in a pop-up dialog. It should be possible to show either objects that depend on the chosen object, or objects on which the chosen object depend.

