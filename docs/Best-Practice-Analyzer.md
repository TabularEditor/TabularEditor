# Best Practice Analyzer

```eval_rst
.. note::
    Some of the information and screenshots in this article is outdated, as the Best Practice Analyzer has received a `complete overhaul in Tabular Editor v. 2.8.1 <https://docs.tabulareditor.com/en/latest/Best-Practice-Analyzer-Improvements.html>`_. Information on Dynamic LINQ (rule expressions) is still up-to-date.
```

Inspired by [this excellent suggestion](https://github.com/otykier/TabularEditor/issues/39), I am proud to present the Best Practice Analyzer (BPA) - a brand new feature of Tabular Editor. Go to the Tools-menu and click "Best Practice Analyzer...", this will open the following window (you can continue working on your model in the main window, while the BPA window stays open):

![image](https://cloud.githubusercontent.com/assets/8976200/25298153/07cb3ae0-26f3-11e7-84cb-1c27a5911560.png)

BPA lets you define rules on the metadata of your model, to encourage certain conventions and best practices while developing in SSAS Tabular.

Clicking one of the rules in the top list, will show you all objects that satisfy the conditions of the given rule in the bottom list:

![image](https://cloud.githubusercontent.com/assets/8976200/25298226/9c036214-26f3-11e7-97ea-03ef82366eb5.png)

Double-clicking an object in the list switches the focus back to the main Tabular Editor window, where the object will be selected in the Explorer Tree ("Go to object..."). It is also possible to specify that a rule should be ignored entirely (can also be done by removing the checkmarks from the rule list), or ignored only for a specific object. Ignores are stored in the metadata annotations of the Model.bim file.

To create a new rule, simply click "Add rule..." while you have a Tabular Model loaded in Tabular Editor. This opens a new window, where you can give your rule a name, a description and specify the conditions of the rule:

![image](https://cloud.githubusercontent.com/assets/8976200/25298330/4178cbe4-26f4-11e7-97ee-d80c1dbc54ed.png)

A visual rule builder is planned for a later release. For now, you specify the rule condition using a [Dynamic LINQ expression](https://github.com/kahanu/System.Linq.Dynamic/wiki/Dynamic-Expressions) that allows you to access all properties on the type(s) of object(s) specified in the dropdown. All objects that satisfy the condition will show up in the BPA UI when the rule is selected.

By default, a rule created this way will be added to the metadata annotations of the Model object and stored in the Model.bim file or the connected database, when you click "Save" in Tabular Editor. You can promote a rule stored locally in a model to a "global" rule. Global rules are stored in your %AppData%\Local\TabularEditor folder in a file called "BPARules.json". You can also put the BPARules.json in the %ProgramData%\TabularEditor folder, to make the rules available to all users on the machine.

Note that the rule ID's must always be unique. In case a rule within the model metadata has the same ID as a rule in the %AppData% or %ProgramData% folder, the order of precedence is:

- Rules stored locally in the model
- Rules stored in the %AppData%\Local folder
- Rules stored in the %ProgramData% folder

## Rule Expression Samples
In this section, you'll see some examples of Dynamic LINQ expressions that can be used to define rules. The expression that is entered in the Rule Expression Editor, will be evaluated whenever focus leaves the textbox, and any syntax errors will be shown on top of the screen:

![image](https://cloud.githubusercontent.com/assets/8976200/25380170/9f01634e-29af-11e7-952e-e10a1f28df32.png)

Your rule expressions may access any public properties on the objects in the TOM. If you try to access a property that does not exist on that type of object, an error will also be shown:

![image](https://cloud.githubusercontent.com/assets/8976200/25381302/798bab98-29b3-11e7-931e-789e5286fc45.png)

"Expression" does not exist on the "Column" object, but if we switch the dropdown to "Calculated Columns", the statement above works fine:

![image](https://cloud.githubusercontent.com/assets/8976200/25380451/87b160da-29b0-11e7-8e2e-c4e47593007d.png)

Dynamic LINQ supports all the standard arithmetic, logical and comparison operators, and using the "."-notation, you can access subproperties and -methods of all objects.

```
String.IsNullOrWhitespace(Expression) and not Name.StartsWith("Dummy")
```

The above statement, applied to Calculated Columns, Calculated Tables or Measures, flags those that have an empty DAX expression unless the object's name starts with the text "Dummy".

Using LINQ, we can also work with collections of objects. The following expression, applied to tables, will find those that have more than 10 columns which are not organized in Display Folders:

```
Columns.Count(DisplayFolder = "") > 10
```

Whenever we use a LINQ method to iterate over a collection, the expression used as an argument to the LINQ method is evaluated on the items in the collection. Indeed, DisplayFolder is a property on columns that does not exist at the Table level.

Here, we see this rule in action on the Adventure Works tabular model. Note how the "Reseller" table shows up as being in violation, while the "Reseller Sales" does not show up (columns in the latter have been organized in Display Folders):

![image](https://cloud.githubusercontent.com/assets/8976200/25380809/d9d1c3a4-29b1-11e7-839e-29450ad39c8a.png)

To refer to the parent object inside a LINQ method, use the special "outerIt" syntax. This rule, applied to tables, will find those that contain columns whose name does not start with the table name:

```
Columns.Any(not Name.StartsWith(outerIt.Name))
```

It would probably make more sense to apply this rule to Columns directly, in which case it should be written as:

```
not Name.StartsWith(Table.Name)
```

To compare against enumeration properties, simply pass the enumerated value as a string. This rule, will find all columns whose name end with the word "Key" or "ID", but where the SummarizeBy property has not been set to "None":

```
(Name.EndsWith("Key") or Name.EndsWith("ID")) and SummarizeBy <> "None"
```

## Finding unused objects
When building Tabular Models it is important to avoid high-cardinality columns at all costs. Typical culprits are system timestamps, technical keys, etc. that have been imported to the model by mistake. In general, we should make sure that the model only contains columns that are actually needed. Wouldn't it be nice if the Best Practice Analyzer could tell us which columns are likely not needed at all?

The following rule will report columns that:

- ...are hidden (or whose parent table is hidden)
- ...are not referenced by any DAX expressions (considers all DAX expressions in the model - even drillthrough and RLS filter expressions)
- ...do not participate in any relationships
- ...are not used as the "Sort By"-column of any other column
- ...are not used as levels of a hierarchy.

The Dynamic LINQ expression for this BPA rule is:

```
(IsHidden or Table.IsHidden)
and ReferencedBy.Count = 0 
and (not UsedInRelationships.Any())
and (not UsedInSortBy.Any())
and (not UsedInHierarchies.Any())
``` 

The same technique can be used to find unused measures. It's a little simpler, since measures can't participate in relationships, etc. So instead, let's spice things up a bit, by also considering whether any downstream objects that reference a given measure, are visible or not. That is, if measure [A] is referenced by measure [B], and both measure [A] and [B] are hidden, and no other DAX expressions refer to these two measures, we should let the developer know that it is safe to remove both of them:

```
(IsHidden or Table.IsHidden)
and not ReferencedBy.AllMeasures.Any(not IsHidden)
and not ReferencedBy.AllColumns.Any(not IsHidden)
and not ReferencedBy.AllTables.Any(not IsHidden)
and not ReferencedBy.Roles.Any()
```

## Fixing objects
In some cases, it is possible to automatically fix the issues on objects satisfying the criteria of a rule. For example when it's just a matter of setting a simple property on the object. Take a closer look at the JSON behind the following rule:

```json
{
    "ID": "FKCOLUMNS_HIDDEN",
    "Name": "Hide foreign key columns",
    "Category": null,
    "Description": "Columns used on the Many side of a relationship should be hidden.",
    "Severity": 1,
    "Scope": "Column",
    "Expression": "Model.Relationships.Any(FromColumn = outerIt) and not IsHidden and not Table.IsHidden",
    "FixExpression": "IsHidden = true",
    "Compatibility": [
      1200,
      1400
    ],
    "IsValid": false
}
```

This rule finds all columns that are used in a relationship (on the "Many"/"From" side), but where the column or its parent table are not hidden. It is recommended that such columns are never shown, as users should filter data using the related (dimension) table instead. So the fix in this case, would be to set the columns IsHidden property to true, which is exactly what the "FixExpression" string above does. To see this in action, right-click any objects that violate the rule, and choose "Generate Fix Script". This puts a small script into the clipboard, which can be pasted into the Advanced Script Editor, from where you can easily review the code and execute it:

![image](https://cloud.githubusercontent.com/assets/8976200/25298489/9035bab6-26f5-11e7-8134-8502daaf4132.png)

Remember that you can always undo (CTRL+Z) changes done to a model after script execution.

Feedback on this new tool is most welcome! In the future, we plan to provide a set of universal Best Practices that will ship with Tabular Editor to get you started. Furthermore, plans are in motion to make the Best Practice Analyzer available as a plug-in to Visual Studio, so those of you not using Tabular Editor can still benefit from it.

## Official Best Practice Rules

To provide Tabular Editor users with a set of standard Best Practices, a new GitHub [repository has been created here](https://github.com/TabularEditor/BestPracticeRules), that will serve as a public collection of Best Practice Rules that the community can contribute to. Any rules that are deemed to be generally viable for all kinds of Tabular Modelling, will be included in periodic "Releases" at that repository. At a later time, Tabular Editor will be able to automatically fetch these rules from the GitHub repository, eliminating the need to manually download the BPARules.json file from the repository.