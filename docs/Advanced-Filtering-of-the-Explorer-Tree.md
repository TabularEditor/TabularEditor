# Advanced Object Filtering

This article describes how to use the "Filter" textbox within Tabular Editor - an incredibly useful feature when navigating complex models.

## Filtering Mode
As of [2.7.4](https://github.com/otykier/TabularEditor/releases/tag/2.7.4), Tabular Editor now lets you decide how the filter should apply to objects in the hierarchy, and how search results are displayed. This is controlled using the three right-most toolbar buttons next to the Filter button:

![image](https://user-images.githubusercontent.com/8976200/46567931-08a4b480-c93d-11e8-96fd-e197e87a0587.png)

* ![image](https://user-images.githubusercontent.com/8976200/46567944-44d81500-c93d-11e8-91e2-d9822078dba7.png) **Hierarchical by parent**: The search will apply to _parent_ objects, that is Tables and Display Folders (if those are enabled). All child items will be displayed, when a parent item matches the search criteria.
* ![image](https://user-images.githubusercontent.com/8976200/46567940-2ffb8180-c93d-11e8-9fba-84fbb79b6bb3.png) **Hierarchical by children**: The search will apply to _child_ objects, that is Measures, Columns, Hierarchies, etc. Parent objects will only be displayed if they have at least one child object matching the search criteria.
* ![image](https://user-images.githubusercontent.com/8976200/46567941-37bb2600-c93d-11e8-9c02-86502f41bce8.png) **Flat**: The search will apply to all objects, and results will be displayed in a flat list. Objects that contain child items will still display these in a hierarchical manner.

## Simple search
Type anything into the Filter textbox and hit [Enter] to do a simple case-insensitive search within object names. For example, typing "sales" in the Filter textbox, using the "By Parent" filtering mode, will produce the following results:

![image](https://user-images.githubusercontent.com/8976200/46568002-5f5ebe00-c93e-11e8-997b-7f89dfd92076.png)

Expanding any of the tables will reveal all measures, columns, hierarchies and partitions of the table. If we change the filtering mode to "By Child", the results will look like this:

![image](https://user-images.githubusercontent.com/8976200/46568016-9f25a580-c93e-11e8-9bc2-c0a16a890256.png)

Notice how the "Employee" table now appears in the list, since it has a couple of child items (columns in this case), that contain the word "sales".

## Wildcard search
When typing in a string in the Filter textbox, you can use the wildcard `?` to denote any single character, and `*` to denote any sequence of characters (zero or more). So typing `*sales*` would produce exactly the same results as shown above, however typing `sales*` will only show objects whose name _starts_ with the word "sales" (again, this is case-insensitive).

Searching for `sales*` by parent:

![image](https://user-images.githubusercontent.com/8976200/46568043-19eec080-c93f-11e8-8d81-2a6214bfa572.png)

Searching for `sales*` by child:

![image](https://user-images.githubusercontent.com/8976200/46568117-f9733600-c93f-11e8-96ab-f87769b8097c.png)

Flat search for `sales*` (toggle info columns [Ctrl]+[F1] to show detailed information about each object):

![image](https://user-images.githubusercontent.com/8976200/46568118-042dcb00-c940-11e8-82d1-516207450559.png)

Wildcards can be placed anywhere in the string, and you can include as many as you need. If that's not complex enough, read on...

## Dynamic LINQ search
You can also use [Dynamic LINQ](https://github.com/kahanu/System.Linq.Dynamic/wiki/Dynamic-Expressions) to search for objects, which is the same thing you do when creating [Best Practice Analyzer rules](/Best-Practice-Analyzer). To enable Dynamic LINQ mode in the filter box, simply put a `:` (colon) in front of your search string. For example, to view all objects whose name end with "Key" (case-sensitive) write:

```
:Name.EndsWith("Key")
```

...and hit [Enter]. In "Flat" filtering mode, the result looks like this:

![image](https://user-images.githubusercontent.com/8976200/46568130-33dcd300-c940-11e8-903c-193e1acde0ad.png)

For case-insensitive search in Dynamic LINQ, you can either convert the input string using something like:

```
:Name.ToUpper().EndsWith("KEY")
```

or you can supply the [StringComparison](https://docs.microsoft.com/en-us/dotnet/api/system.string.endswith?view=netframework-4.7.2#System_String_EndsWith_System_String_System_StringComparison_) argument, like:

```
:Name.EndsWith("Key", StringComparison.InvariantCultureIgnoreCase)
```

You are not restricted to searching within the names of objects. Dynamic LINQ search strings can be as complex as you like, to evaluate any property (as well as sub-properties) of an object. So if you want to find all objects having an expression that contains the word "TODO", you would use the following search filter:

```
:Expression.ToUpper().Contains("TODO")
```

As another example, the following will display all hidden measures in the model that are not referenced by anything else:

```
:ObjectType="Measure" and (IsHidden or Table.IsHidden) and ReferencedBy.Count=0
````

You can also use Regular Expressions. The following will find all columns whose name contains the word "Number" OR "Amount":

```
:ObjectType="Column" and RegEx.IsMatch(Name,"(Number)|(Amount)")
```

Note, that the display options (the toolbar buttons directly above the tree), may affect the results when using "By Parent" and "By Child" filtering mode. For example, the above LINQ filter only returns columns, but if your display options are currently set to not show columns, nothing will be displayed.