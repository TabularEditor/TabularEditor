# Tabular Editor 2.0 Advanced Scripting
This is an introduction to the Advanced Scripting capabilities of Tabular Editor. Information in this document is subject to change.

## What is Advanced Scripting?
The goal of the UI of Tabular Editor, is to make it easy to perform most tasks commonly needed when building Tabular Models. For example, changing the Display Folder of multiple measures at once is just a matter of selecting the objects in the explorer tree and then dragging and dropping them around. The right-click context menu of the explorer tree provides a convenient way to perform many of these tasks, such as adding/removing objects from perspectives, renaming multiple objects, etc.

There may be many other common workflow tasks, which are not as easily performed through the UI however. For this reason, Tabular Editor introduces Advanced Scripting, which lets advanced users write a script using C# syntax, to more directly manipulate the objects in the loaded Tabular Model.

## Objects
The scripting API provides access to two top-level objects, `Model` and `Selected`. The former contains methods and properties that allow you to manipulate all objects in the Tabular Model, whereas the latter exposes only objects that are currently selected in the explorer tree.

The `Model` object is a wrapper of the [Microsoft.AnalysisServices.Tabular.Model](https://msdn.microsoft.com/en-us/library/microsoft.analysisservices.tabular.model.aspx) class, exposing a subset of it’s properties, with some additional methods and properties for easier operations on translations, perspectives and object collections. The same applies to any descendant objects, such as Table, Measure, Column, etc. which all have corresponding wrapper objects. Please see ***Appendix A*** for a complete listing of objects, properties and methods in the Tabular Editor wrapper library.

The main advantage of working through this wrapper is, that all changes will be undoable from the Tabular Editor UI. Simply press CTRL+Z after executing a script, and you will see that all changes made by the script are immediately undone. Furthermore, the wrapper provides convenient methods that turn many common tasks into simple one-liners. We will provide some examples below. It is assumed that the reader is already somewhat familiar with C# and LINQ, as these aspects of Tabular Editors scripting capabilities will not be covered here. Users unfamiliar with C# and LINQ should still be able to follow the examples given below.

## Setting object properties
If you want to change a property of one object in particular, obviously the easiest way to do so would be directly through the UI. But as an example, let us see how we could achieve the same thing through scripting.

Say you want to change the Format String of your [Sales Amount] measure in the 'FactInternetSales' table. If you locate the measure in the explorer tree, you can simply drag it onto the script editor. Tabular Editor will then generate the following code, which represents this particular measure in the Tabular Object Model:

`Model.Tables["FactInternetSales"].Measure["Sales Amount"]`

Adding an extra dot (.) after the right-most bracket, should make the autocomplete menu pop up, showing you which properties and methods exist on this particular measure. Simply choose “FormatString” in the menu, or write the first few letters and hit Tab. Then, enter an equal sign followed by “0.0%”. Let us also change the Display Folder of this measure. Your final code should look like this:

`Model.Tables["FactInternetSales"].Measure["Sales Amount"].FormatString = "0.0%";
Model.Tables["FactInternetSales"].Measure["Sales Amount"].DisplayFolder = "New Folder";`

**Note:** Remember to put the semicolon (;) at the end of each line. This is a requirement of C#. If you forget it, you will get a syntax error message when trying to execute the script.

Hit F5 or the ”Play” button above the script editor to execute the script. Immediately, you should see the measure moving around in the explorer tree, reflecting the changed Display Folder. If you examine the measure in the Property Grid, you should also see that the Format String property has changed accordingly.

### Working with multiple objects at once
Many objects in the object model, are actually collections of multiple objects. For example, each Table object has a Measures collection. The wrapper exposes a series of convenient properties and methods on these collections, to make it easy to set the same property on multiple objects at once. This is described in detail below. Additionally, you may use all the standard LINQ extension methods to filter and browse the objects of a collection.

Below is a few examples of the most commonly used LINQ extension methods:

`Collection.First([predicate])` Returns the first object in the collection satisfying the optional [predicate] condition.
`Collection.Any([predicate])` Returns true if the collection contains any objects (optionally satisfying the [predicate] condition).
`Collection.Where(predicate)` Returns a collection that is the original collection filtered by the predicate condition.
`Collection.Select(map)` Projects each object in the collection into another object according to the specified map.
`Collection.ForEach(action)` Performs the specified action on each element in the collection.

Use the IntelliSense functionality of the Advanced Script editor to see what other LINQ methods exist, or refer to the [LINQ-to-Objects documentation](https://msdn.microsoft.com/en-us/library/9eekhta0.aspx).

## Working with **Model**
To quickly reference any object in the currently loaded Tabular Model, you can drag and drop the object from the explorer tree and into the Advanced Scripting editor:

![Dragging and dropping an object into the Advanced Scripting editor](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/DragDropTOM.gif)

Please refer to the [TOM documentation](https://msdn.microsoft.com/en-us/library/microsoft.analysisservices.tabular.model.aspx) for an overview of which properties exist on the Model and its descendant objects. Additionally, refer to ***Appendix A*** for a complete listing of the properties and methods exposed by the wrapper object.

## Working with **Selected**
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

```Selected.Measures
        .Rename("Amount", "Value");
```

This would replace any occurence of the word ”Amount” with the word ”Value” in the names of all currently selected measures.
Alternatively, we may use the LINQ ForEach()-method, as described above, to include more advanced logic:

```Selected.Measures
        .ForEach(m => if(m.Name.Contains("Reseller")) m.Name += " DEPRECATED");
```

This example will prepend the text " DEPRECATED" to the names of all selected measures where the names contain the word "Reseller". Alternatively, we could use the LINQ extension method `Where()` to filter the collection before applying the `ForEach()` operation, which would yield exactly the same result:

```Selected.Measures
        .Where(m => m.Name.Contains("Reseller"))
        .ForEach(m => m.Name += " DEPRECATED");
```
