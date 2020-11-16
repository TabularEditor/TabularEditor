# Advanced Scripting

This is an introduction to the Advanced Scripting capabilities of Tabular Editor. Information in this document is subject to change. Also, make sure to check out the article on [Useful script snippets](/Useful-script-snippets), for some more real-life examples of what you can do with the scripting capabilities of Tabular Editor.

## What is Advanced Scripting?
The goal of the UI of Tabular Editor is to make it easy to perform most tasks commonly needed when building Tabular Models. For example, changing the Display Folder of multiple measures at once is just a matter of selecting the objects in the explorer tree and then dragging and dropping them around. The right-click context menu of the explorer tree provides a convenient way to perform many of these tasks, such as adding/removing objects from perspectives, renaming multiple objects, etc.

There may be many other common workflow tasks, which are not as easily performed through the UI however. For this reason, Tabular Editor introduces Advanced Scripting, which lets advanced users write a script using C# syntax, to more directly manipulate the objects in the loaded Tabular Model.

## Objects
The scripting API provides access to two top-level objects, `Model` and `Selected`. The former contains methods and properties that allow you to manipulate all objects in the Tabular Model, whereas the latter exposes only objects that are currently selected in the explorer tree.

The `Model` object is a wrapper of the [Microsoft.AnalysisServices.Tabular.Model](https://msdn.microsoft.com/en-us/library/microsoft.analysisservices.tabular.model.aspx) class, exposing a subset of it’s properties, with some additional methods and properties for easier operations on translations, perspectives and object collections. The same applies to any descendant objects, such as Table, Measure, Column, etc. which all have corresponding wrapper objects. Please see ***Appendix A*** for a complete listing of objects, properties and methods in the Tabular Editor wrapper library.

The main advantage of working through this wrapper is, that all changes will be undoable from the Tabular Editor UI. Simply press CTRL+Z after executing a script, and you will see that all changes made by the script are immediately undone. Furthermore, the wrapper provides convenient methods that turn many common tasks into simple one-liners. We will provide some examples below. It is assumed that the reader is already somewhat familiar with C# and LINQ, as these aspects of Tabular Editors scripting capabilities will not be covered here. Users unfamiliar with C# and LINQ should still be able to follow the examples given below.

## Setting object properties
If you want to change a property of one object in particular, obviously the easiest way to do so would be directly through the UI. But as an example, let us see how we could achieve the same thing through scripting.

Say you want to change the Format String of your [Sales Amount] measure in the 'FactInternetSales' table. If you locate the measure in the explorer tree, you can simply drag it onto the script editor. Tabular Editor will then generate the following code, which represents this particular measure in the Tabular Object Model:

```csharp
Model.Tables["FactInternetSales"].Measures["Sales Amount"]
```

Adding an extra dot (.) after the right-most bracket, should make the autocomplete menu pop up, showing you which properties and methods exist on this particular measure. Simply choose “FormatString” in the menu, or write the first few letters and hit Tab. Then, enter an equal sign followed by “0.0%”. Let us also change the Display Folder of this measure. Your final code should look like this:

```csharp
Model.Tables["FactInternetSales"].Measures["Sales Amount"].FormatString = "0.0%";
Model.Tables["FactInternetSales"].Measures["Sales Amount"].DisplayFolder = "New Folder";
```

**Note:** Remember to put the semicolon (;) at the end of each line. This is a requirement of C#. If you forget it, you will get a syntax error message when trying to execute the script.

Hit F5 or the ”Play” button above the script editor to execute the script. Immediately, you should see the measure moving around in the explorer tree, reflecting the changed Display Folder. If you examine the measure in the Property Grid, you should also see that the Format String property has changed accordingly.

### Working with multiple objects at once
Many objects in the object model, are actually collections of multiple objects. For example, each Table object has a Measures collection. The wrapper exposes a series of convenient properties and methods on these collections, to make it easy to set the same property on multiple objects at once. This is described in detail below. Additionally, you may use all the standard LINQ extension methods to filter and browse the objects of a collection.

Below is a few examples of the most commonly used LINQ extension methods:

* `Collection.First([predicate])` Returns the first object in the collection satisfying the optional [predicate] condition.
* `Collection.Any([predicate])` Returns true if the collection contains any objects (optionally satisfying the [predicate] condition).
* `Collection.Where(predicate)` Returns a collection that is the original collection filtered by the predicate condition.
* `Collection.Select(map)` Projects each object in the collection into another object according to the specified map.
* `Collection.ForEach(action)` Performs the specified action on each element in the collection.

In the above examples, `predicate` is a lambda expression that takes a single object as input, and returns a boolean value as output. For example, if `Collection` is a collection of measures, a typical `predicate` could look like:

`m => m.Name.Contains("Reseller")`

This predicate would return true only if the Name of the measure contains the character string "Reseller". Wrap the expression in curly braces and use the `return` keyword, if you need more advanced logic:

```csharp
.Where(obj => {
    if(obj is Column) {
        return false;
    }
    return obj.Name.Contains("test");
})
```

Going back to the examples above, `map` is a lambda expression that takes a single object as input, and returns any single object as output. `action` is a lambda expression that takes a single object as input, but does not return any value.

Use the IntelliSense functionality of the Advanced Script editor to see what other LINQ methods exist, or refer to the [LINQ-to-Objects documentation](https://msdn.microsoft.com/en-us/library/9eekhta0.aspx).

## Working with the **Model** object
To quickly reference any object in the currently loaded Tabular Model, you can drag and drop the object from the explorer tree and into the Advanced Scripting editor:

![Dragging and dropping an object into the Advanced Scripting editor](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/DragDropTOM.gif)

Please refer to the [TOM documentation](https://msdn.microsoft.com/en-us/library/microsoft.analysisservices.tabular.model.aspx) for an overview of which properties exist on the Model and its descendant objects. Additionally, refer to ***Appendix A*** for a complete listing of the properties and methods exposed by the wrapper object.

## Working with the **Selected** object
Being able to explicitly refer to any object in the Tabular Model is great for some workflows, but sometimes you want to cherry pick objects from the explorer tree, and then execute a script against only the selected objects. This is where the `Selected` object comes in handy.

The `Selected` object provides a range of properties that make it easy to identify what is currently selected, as well as limiting the selection to objects of a particular type. When browsing with Display Folders, and one or more folders are selected in the explorer tree, all their child items are considered to be selected as well.
For single selections, use the singular name for the type of object you want to access. For example,

`Selected.Hierarchy`

refers to the currently selected hierarchy in the tree, provided that one and only one hierarchy is selected. Use the plural type name, in case you want to work with multiselections:

`Selected.Hierarchies`

All properties that exist on the singular object, also exist on its plural form, with a few exceptions. This means that you can set the value of these properties for multiple objects at once, with just one line of code and without using the LINQ extension methods mentioned above. For example, say you wanted to move all currently selected measures into a new Display Folder called “Test”:

`Selected.Measures.DisplayFolder = "Test";`

If no measures are currently selected in the tree, the above code does nothing, and no error is raised. Otherwise, the DisplayFolder property will be set to "Test" on all selected measures (even measures residing within folders, as the `Selected` object also includes objects in selected folders). If you use the singular form `Measure` instead of `Measures`, you will get an error unless the current selection contains exactly one measure.

Although we cannot set the Name property of multiple objects at once, we still have some options available. If we just want to replace all occurrences of some character string with another, we can use the provided “Rename” method, like so:

```csharp
Selected.Measures
        .Rename("Amount", "Value");
```

This would replace any occurence of the word ”Amount” with the word ”Value” in the names of all currently selected measures.
Alternatively, we may use the LINQ ForEach()-method, as described above, to include more advanced logic:

```csharp
Selected.Measures
        .ForEach(m => if(m.Name.Contains("Reseller")) m.Name += " DEPRECATED");
```

This example will append the text " DEPRECATED" to the names of all selected measures where the names contain the word "Reseller". Alternatively, we could use the LINQ extension method `Where()` to filter the collection before applying the `ForEach()` operation, which would yield exactly the same result:

```csharp
Selected.Measures
        .Where(m => m.Name.Contains("Reseller"))
        .ForEach(m => m.Name += " DEPRECATED");
```

## Helper methods
To make debugging scripts easier, Tabular Editor provides a set of special helper methods. Internally, these are static methods decorated with the `[ScriptMethod]`-attribute. This attribute allows scripts to call the methods directly, without the need to specify a namespace or class name. Plugins may also use the `[ScriptMethod]` attribute to expose public static methods for scripting in a similar way.

As of 2.7.4, Tabular Editor provides the following script methods. Note that some of these may be invoked as extension methods. For example, `object.Output();` and `Output(object);` are equivalent.

* `Output(object);` - halts script execution and displays information about the provided object. When the script is running as part of a command line execution, this will write a string representation of the object to the console.
* `SaveFile(filePath, content);` - convenient way to save text data to a file.
* `ReadFile(filePath);` - convenient way to load text data from a file.
* `ExportProperties(objects, properties);` - convenient way to export a set of properties from multiple objects as a TSV string.
* `ImportProperties(tsvData);` - convenient way to load properties into multiple objects from a TSV string.
* `CustomAction(name);` - invoke a Custom Action by name.
* `CustomAction(objects, name);` - invoke a Custom Action on the specified objects.
* `ConvertDax(dax, useSemicolons);` - converts a DAX expression between US/UK and non-US/UK locales. If `useSemicolons` is true (default) the `dax` string is converted from the native US/UK format to non-US/UK. That is, commas (list separators) will be converted to semicolons and periods (decimal separators) will be converted to commas. Vice versa if `useSemicolons` is set to false.
* ~~`FormatDax(string dax);`~~ - formats a DAX expression using www.daxformatter.com (Deprecated, please don't use!)
* `FormatDax(IEnumerable<IDaxDependantObject> objects, bool shortFormat, bool? skipSpace)` - formats DAX expressions on all objects in the provided collection
* `FormatDax(IDaxDependantObject obj)` - queues an object for DAX expression formatting when script execution is complete, or when the `CallDaxFormatter` method is called.
* `CallDaxFormatter(bool shortFormat, bool? skipSpace)` - formats all DAX expressions on objects enqueued so far
* `Info(string);` - Writes an informational message to the console (only when the script is executed as part of a command line execution).
* `Warning(string);` - Writes a warning message to the console (only when the script is executed as part of a command line execution).
* `Error(string);` - Writes an error message to the console (only when the script is executed as part of a command line execution).

### Debugging scripts
As mentioned above, you can use the `Output(object);` method to pause script execution, and open a dialog box with information about the passed object. You can also use this method as an extension method, invoking it as `object.Output();`. The script is resumed when the dialog is closed.

The dialog will appear in one of four different ways, depending on the kind of object being output:

- Singular objects (such as strings, ints and DateTimes, except any object that derives from TabularNamedObject) will be displayed as a simple message dialog, by invoking the `.ToString()` method on the object:

![image](https://user-images.githubusercontent.com/8976200/29941982-9917d0cc-8e94-11e7-9e78-24aaf11fd311.png)

- Singular TabularNamedObjects (such as Tables, Measures or any other TOM NamedMetadataObject available in Tabular Editor) will be shown in a Property Grid, similar to when an object has been selected in the Tree Explorer. Properties on the object may be edited in the grid, but note that if an error is encountered at a later point in the script execution, the edit will be automatically undone, if "Rollback on error" is enabled:

![image](https://user-images.githubusercontent.com/8976200/29941852-2acc9846-8e94-11e7-9380-f84fef26a78c.png)

- Any IEnumerable of objects (except TabularNamedObjects) will be displayed in a list, where each list item shows the `.ToString()` value and type of the object in the IEnumerable:

![image](https://user-images.githubusercontent.com/8976200/29942113-02dad928-8e95-11e7-9c04-5bb87b396f3f.png)

- Any IEnumerable of TabularNamedObjects will cause the dialog to display a list of the objects on the left, and a Property Grid on the right. The Property Grid will be populated from whatever object is selected in the list, and properties may be edited just as when a single TabularNamedObject is being output:

![image](https://user-images.githubusercontent.com/8976200/29942190-498cbb5c-8e95-11e7-8455-32750767cf13.png)

You can tick the "Don't show more outputs" checkbox at the lower left-hand corner, to prevent the script from halting on any further `.Output()` invocations.

## .NET references

[Tabular Editor version 2.8.6](https://github.com/otykier/TabularEditor/tree/2.8.6) makes it a lot easier to write complex scripts. Thanks to the new pre-processor, you can now use the `using` keyword to shorten class names, etc. just like in regular C# source code. In addition, you can include external assemblies by using the syntax `#r "<assembly name or DLL path>"` similar to .csx scripts used in Azure Functions.

For example, the following script will now work as expected:

```csharp
// Assembly references must be at the very top of the file:
#r "System.IO.Compression"

// Using keywords must come before any other statements:
using System.IO.Compression;
using System.IO;

var xyz = 123;

// Using statements still work the way they're supposed to:
using(var data = new MemoryStream())
using(var zip = new ZipArchive(data, ZipArchiveMode.Create)) 
{
   // ...
}
```

By default, Tabular Editor applies the following `using` keyword (even though they are not specified in the script), to make common tasks easier:

```csharp
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI;
```

In addition, the following .NET Framework assemblies are loaded by default:

- System.Dll
- System.Core.Dll
- System.Data.Dll
- System.Windows.Forms.Dll
- Microsoft.Csharp.Dll
- Newtonsoft.Json.Dll
- TomWrapper.Dll
- TabularEditor.Exe
- Microsoft.AnalysisServices.Tabular.Dll

## Compiling with Roslyn

If you prefer to compile your scripts using the new Roslyn compiler introduced with Visual Studio 2017, you can set this up under File > Preferences > General, starting with Tabular Editor version 2.12.2. This allows you to use newer C# language features such as string interpolation. Simply specify the path to the directory that holds the compiler executable (`csc.exe`) and specify the language version as an option for the compiler:

![image](https://user-images.githubusercontent.com/8976200/92464140-0902f580-f1cd-11ea-998a-b6ecce57b399.png)

### Visual Studio 2017

For a typical Visual Studio 2017 Enterprise installation, the Roslyn compiler is located here:

```
c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\Roslyn
```

This includes the C# 6.0 language features by default.

![image](https://user-images.githubusercontent.com/8976200/92464584-a52cfc80-f1cd-11ea-9b66-3b47ac36f6c6.png)

### Visual Studio 2019

For a typical Visual Studio 2019 Community installation, the Roslyn compiler is located here:

```
c:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\Roslyn
```

The compiler that ships with VS2019 supports C# 8.0 language features, which can be enabled by specifying the following as compiler options:

```
-langversion:8.0
```
