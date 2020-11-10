# Basic Features

The following article will give you a quick overview of the most important features of Tabular Editor.

## Load/save Model.bim files
Hitting CTRL+O shows an Open File dialog, which lets you select a Model.bim file to load in Tabular Editor. The file must be of Compatibility Level 1200 or newer (JSON format). CTRL+S saves any changes you make in Tabular Editor back to the file (we recommend backing up your Model.bim files before using Tabular Editor). If you want to deploy the loaded model to an Analysis Services server instance, see [Deployment](/Features-at-a-glance#deployment) below.

## Connect/deploy to SSAS Tabular Databases
Hitting CTRL+SHIFT+O lets you open a Tabular Model directly from a Tabular Database that has already been deployed. Enter the server address and (optionally) provide a username and password. After hitting "OK", you will be prompted with a list of databases and the server. Select the one you want to load, and click "OK" again. 

![](https://github.com/otykier/TabularEditor/blob/master/Documentation/Connect.png)

The dialog shown also lets you connect to Azure Analysis Services instances, if you provide the full name of the Azure AS instance, starting with "azureas://". The "Local Instance" dropdown, may be used to browse and connect to any running instances of Power BI Desktop or Visual Studio Integrated Workspaces. **Note that although Tabular Editor can make changes to a Power BI model through the TOM, this is not supported by Microsoft and may corrupt your .pbix file. Proceed at your own risk!**

Any time you press CTRL+S after the database has been loaded, the database will be updated with any changes you've made in Tabular Editor. Client tools (Excel, Power BI, DAX Studio, etc.) should be able to immediately view the changes in the database after this. Note that you may need to manually recalculate objects in the model, depending on the changes made, to successfully query the model.

If you want to save the connected model to a Model.bim file, choose "Save As..." from the "File" menu.

## Deployment
If you want to deploy the currently loaded model to a new database, or overwrite an existing database with the model changes (for example when loading from a Model.bim file), use the Deployment Wizard under "Model" > "Deploy...". The wizard will guide you through the deployment process, and allow you to choose which areas of the model to deploy. More information can be found [here](/Advanced-features#deployment-wizard).

## Hierarchical display
Objects of the loaded model are shown in the Explorer Tree, on the left side of the screen. By default, all object types (visible tables, roles, relationships, etc.) are shown. If you only want to see tables, measures, columns and hierarchies, go to the "View" menu and toggle off "Show all object types".

![](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/AllObjectTypes.png)

Expanding a table in the "Tables" group, you will find the measures, columns and hierarchies contained in the table presented in their respective display folders by default. This way, objects are arranged similar to how end-users would see them in client tools:

![](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/DisplayFolders.png)

Use the buttons immediately above the Explorer Tree, to toggle invisible objects, display folders, measures, columns and hierarchies, or to filter objects by name. You can rename an object by selecting it in then hitting F2. This also works for display folders. If you double-click a measure or calculated column, you may edit its [DAX expression](/Advanced-features#dax-expression-editor). Right-clicking will show a context menu, providing a range of handy shortcuts for operations such as setting visibility, perspective inclusion, adding columns to a hierarchy, etc.

## Editing properties
The Property Grid on the lower right side of the screen, shows most of the properties for the object(s) selected in the Explorer Tree. If you select multiple objects at once, the Property Grid lets you simultaneously edit properties for the selected objects. This is useful for example when setting the Format String property. Examples of properties you can set through the Property Grid:

* Name (you can rename objects directly in the Explorer Tree by hitting F2)
* Description
* Display Folder (can also be renamed directly in the Explorer Tree, also [drag/drop](/Features-at-a-glance#drag-and-drop-objects))
* Hidden (can be set for multiple objects through the right-click context menu in the Explorer Tree)
* Format String

Different properties exist, depending on what kind of object was selected.

## Duplicate objects and batch renamings
The right-click context menu in the Explorer Tree lets you duplicate measures and columns. The duplicated objects will have their names suffixed by "copy". Furthermore, you can perform batch renames by selecting multiple objects and right-clicking in the Explorer Tree.

![](https://github.com/otykier/TabularEditor/blob/master/Documentation/BatchRename.png)

You may use RegEx for your renamings, and optionally choose whether translations should be renamed as well.

## Drag and drop objects
By far the most useful feature of Tabular Editor, when working on models with many measures/columns organised in display folders. Check out the animation below:

![](https://github.com/otykier/TabularEditor/blob/master/Documentation/DragDropFolders.gif)

Notice how the display folder property of every single object below the folder is changed, when the entire folder is dragged. No more going over measures/columns one-by-one, to change the display folder structure. What you see is what you get.

(This works with translations too!)

## Working with Perspectives and Translations
You can add/edit existing perspectives and translations (cultures), by clicking the Model node in the Explorer Tree, and locating the relevant properties at the bottom of the property grid. Alternatively, when your Explorer Tree is [showing all object types](/Features-at-a-glance#hierarchical-display), you can view and edit perspectives, cultures and roles directly in the tree.

![](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/RolesPerspectivesTranslations.png)

You can duplicate an existing perspective, role or translation by opening the right-click menu and choose "Duplicate". This will create an exact copy of the object, which you can then modify to your needs.

To view perspectives and/or translations "in action", use the two dropdown lists in the toolbar near the top of the screen. Choosing a perspective will hide all objects that are not included in that perspective, while choosing a translation will show all objects in the tree using the translated names and display folders. When hitting F2 to change the names of objects/display folders or when dragging objects around in the tree, the changes will only apply to the selected translation.

## Perspectives/Translations within object context
When one or more objects are selected in the tree, you will find 4 special property collections within the Property Grid:

* **Captions**, **Descriptions** and **Display Folders** shows a list of all cultures in the model, with the translated names, descripions and display folders respectively of the selected objects for each culture.
* **Perspectives** shows a list of all perspectives in the model, with an indication of whether or nor the selected objects belong to each perspective.

You can use these collections in the Property Grid to change the translations and perspective inclusions for one or more objects at at time.

## Undo/Redo support
Any change you make in Tabular Editor can be undone using CTRL+Z and subsequently redone using CTRL+Y. There is no limit to the number of operations that can be undone, but the stack is reset when you open a Model.bim file or load a model from a database.

When deleting objects from the model, all translations, perspectives and relationships that reference the deleted objects are also automatically deleted (where as Visual Studio normally shows an error message that the object cannot be deleted). If you make a mistake, you can use the Undo functionality to restore the deleted object, which will also restore any translations, perspectives or relationships that were deleted. Note that even though Tabular Editor can detect [DAX formula dependencies](), Tabular Editor will not warn you in case you delete a measure or column which is used in the DAX expression of another measure or calculated column.