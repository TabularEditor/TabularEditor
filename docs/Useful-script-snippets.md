# Useful Script Snippets

Here's a collection of small script snippets to get you started using the [Advanced Scripting functionality](/Advanced-Scripting) of Tabular Editor. Many of these scripts are useful to save as [Custom Actions](/Custom-Actions), so that you can easily reuse them from the context menu.'

If you want to explore other scripts or want to contribute your own, please go to the [Tabular Editor Scripts repository](https://github.com/TabularEditor/Scripts).

***

## Create measures from columns
```csharp
// Creates a SUM measure for every currently selected column and hide the column.
foreach(var c in Selected.Columns)
{
    var newMeasure = c.Table.AddMeasure(
        "Sum of " + c.Name,                    // Name
        "SUM(" + c.DaxObjectFullName + ")",    // DAX expression
        c.DisplayFolder                        // Display Folder
    );
    
    // Set the format string on the new measure:
    newMeasure.FormatString = "0.00";

    // Provide some documentation:
    newMeasure.Description = "This measure is the sum of column " + c.DaxObjectFullName;

    // Hide the base column:
    c.IsHidden = true;
}
```
This snippet uses the `<Table>.AddMeasure(<name>, <expression>, <displayFolder>)` function to create a new measure on the table. We use the `DaxObjectFullName` property to get the fully qualified name of the column for use in the DAX expression: `'TableName'[ColumnName]`.

***

## Generate Time Intelligence measures
First, create custom actions for individual Time Intelligence aggregations. For example:
```csharp
// Creates a TOTALYTD measure for every selected measure.
foreach(var m in Selected.Measures) {
    m.Table.AddMeasure(
        m.Name + " YTD",                                       // Name
        "TOTALYTD(" + m.DaxObjectName + ", 'Date'[Date])",     // DAX expression
        m.DisplayFolder                                        // Display Folder
    );
}
```
Here, we use the `DaxObjectName` property, to generate an unqualified reference for use in the DAX expression, as this is a measure: `[MeasureName]`. Save this as a Custom Action called "Time Intelligence\Create YTD measure" that applies to measures. Create similar actions for MTD, LY, and whatever else you need. Then, create the following as a new action:

```csharp
// Invoke all Time Intelligence Custom Actions:
CustomAction(@"Time Intelligence\Create YTD measure");
CustomAction(@"Time Intelligence\Create MTD measure");
CustomAction(@"Time Intelligence\Create LY measure");
```
This illustrates how you can execute one (or more) Custom Actions from within another action (beware of circular references - that will cause Tabular Editor to crash). Save this as a new Custom Action "Time Intelligence\All of the above", and you will have an easy way to generate all your Time Intelligence measures with a single click:

![image](https://user-images.githubusercontent.com/8976200/36632257-5565c8ca-197c-11e8-8498-82667b6e1049.png)

Of course, you may also put all your time intelligence calculations into a single script such as the following:

```csharp
var dateColumn = "'Date'[Date]";

// Creates time intelligence measures for every selected measure:
foreach(var m in Selected.Measures) {
    // Year-to-date:
    m.Table.AddMeasure(
        m.Name + " YTD",                                       // Name
        "TOTALYTD(" + m.DaxObjectName + ", " + dateColumn + ")",     // DAX expression
        m.DisplayFolder                                        // Display Folder
    );
    
    // Previous year:
    m.Table.AddMeasure(
        m.Name + " PY",                                       // Name
        "CALCULATE(" + m.DaxObjectName + ", SAMEPERIODLASTYEAR(" + dateColumn + "))",     // DAX expression
        m.DisplayFolder                                        // Display Folder
    );    
    
    // Year-over-year
    m.Table.AddMeasure(
        m.Name + " YoY",                                       // Name
        m.DaxObjectName + " - [" + m.Name + " PY]",            // DAX expression
        m.DisplayFolder                                        // Display Folder
    );
    
    // Year-over-year %:
    m.Table.AddMeasure(
        m.Name + " YoY%",                                           // Name
        "DIVIDE(" + m.DaxObjectName + ", [" + m.Name + " YoY])",    // DAX expression
        m.DisplayFolder                                             // Display Folder
    ).FormatString = "0.0 %";  // Set format string as percentage
    
    // Quarter-to-date:
    m.Table.AddMeasure(
        m.Name + " QTD",                                            // Name
        "TOTALQTD(" + m.DaxObjectName + ", " + dateColumn + ")",    // DAX expression
        m.DisplayFolder                                             // Display Folder
    );
    
    // Month-to-date:
    m.Table.AddMeasure(
        m.Name + " MTD",                                       // Name
        "TOTALMTD(" + m.DaxObjectName + ", " + dateColumn + ")",     // DAX expression
        m.DisplayFolder                                        // Display Folder
    );
}
```

### Including additional properties

If you want to set additional properties on the newly created measure, the above script can be modified like so:

```csharp
// Creates a TOTALYTD measure for every selected measure.
foreach(var m in Selected.Measures) {
    var newMeasure = m.Table.AddMeasure(
        m.Name + " YTD",                                       // Name
        "TOTALYTD(" + m.DaxObjectName + ", 'Date'[Date])",     // DAX expression
        m.DisplayFolder                                        // Display Folder
    );
    newMeasure.FormatString = m.FormatString;               // Copy format string from original measure
    foreach(var c in Model.Cultures) {
        newMeasure.TranslatedNames[c] = m.TranslatedNames[c] + " YTD"; // Copy translated names for every culture
        newMeasure.TranslatedDisplayFolders[c] = m.TranslatedDisplayFolders[c]; // Copy translated display folders
    }
}
```

***

## Handling perspectives
Measures, columns, hierarchies and tables all expose the `InPerspective` property, which holds a True/False value for every perspective in the model, that indicates if the given object is a member of that perspective or not. So for example:

```csharp
foreach(var measure in Selected.Measures)
{
    measure.InPerspective["Inventory"] = true;
    measure.InPerspective["Reseller Operation"] = false;
}
```

The script above ensures that all selected measures are visible in the "Inventory" perspective and hidden in the "Reseller Operation" perspective.

In addition to getting/setting the membership in an individual perspective, the `InPerspective` property also supports the following methods:

* `<<object>>.InPerspective.None()` - removes the object from all perspectives.
* `<<object>>.InPerspective.All()` - includes the object in all perspectives.
* `<<object>>.CopyFrom(string[] perspectives)` - includes the object in all specified perspectives (array of string containing names of the perspectives).
* `<<object>>.CopyFrom(perspectiveIndexer perspectives)` - copies perspective inclusions from another `InPerspective` property.

The latter may be used to copy perspective memberships from one object to another. For example, say have a base measure [Reseller Total Sales], and you want to make sure that all currently selected measures are visible in the same perspectives as this base measure. The following script does the trick:

```csharp
var baseMeasure = Model.Tables["Reseller Sales"].Measures["Reseller Total Sales"];

foreach(var measure in Selected.Measures)
{
    /* Uncomment the line below, if you want 'measure' to be hidden
       from perspectives that 'baseMeasure' is hidden in: */
    // measure.InPerspective.None();

    measure.InPerspective.CopyFrom(baseMeasure.InPerspective);
}
```

This technique can be used also when generating new objects from code. For example, if we want to ensure that auto-generated Time Intelligence measures are only visible in the same perspectives as their base measure, we can extend the script from the previous section as:

```csharp
// Creates a TOTALYTD measure for every selected measure.
foreach(var m in Selected.Measures) {
    var newMeasure = m.Table.AddMeasure(
        m.Name + " YTD",                                       // Name
        "TOTALYTD(" + m.DaxObjectName + ", 'Date'[Date])",     // DAX expression
        m.DisplayFolder                                        // Display Folder
    );
    newMeasure.InPerspective.CopyFrom(m.InPerspective);        // Apply perspectives from the base measure
}
```

***

## Export object properties to a file
For some workflows, it may be useful to edit multiple object properties in bulk using Excel. Use the following snippet to export a standard set of properties to a .TSV file, which can then be subsequently imported (see below).
```csharp
// Export properties for the currently selected objects:
var tsv = ExportProperties(Selected);
SaveFile("Exported Properties 1.tsv", tsv);
```
The resulting .TSV file looks like this, when opened in Excel:
![image](https://user-images.githubusercontent.com/8976200/36632472-e8e96ef6-197e-11e8-8285-6816b09ad036.png)
The contents of the first column (Object) is a reference to the object. If the contents of this column is changed, subsequent import of the properties might not work correctly. To change the name of an object, only change the value in the second column (Name).

By default, the file is saved to the same folder as TabularEditor.exe is located. By default, only the following properties are exported (where applicable, depending on the type of object exported):

* Name
* Description
* SourceColumn
* Expression
* FormatString
* DataType

To export different properties, supply a comma-separated list of property names to be exported as the 2nd argument to `ExportProperties`:
```csharp
// Export the names and Detail Rows Expressions for all measures on the currently selected table:
var tsv = ExportProperties(Selected.Table.Measures, "Name,DetailRowsExpression");
SaveFile("Exported Properties 2.tsv", tsv);
```
The available property names can be found in the [TOM API documentation](https://msdn.microsoft.com/en-us/library/microsoft.analysisservices.tabular.aspx). These are mostly identical to the names shown in the Tabular Editor property grid in CamelCase and with spaces removed (with a few exceptions, for example, the "Hidden" property is called `IsHidden` in the TOM API).

To import properties, use the following snippet:
```csharp
// Imports and applies the properties in the specified file:
var tsv = ReadFile("Exported Properties 1.tsv");
ImportProperties(tsv);
```

### Exporting indexed properties

As of Tabular Editor 2.11.0, the `ExportProperties` and `ImportProperties` methods support indexed properties. Indexed properties are properties that take a key in addition to the property name. One example is `myMeasure.TranslatedNames`. This property represents the collection of all strings applied as name translations for `myMeasure`. In C#, you can access the translated caption of a specific culture using the indexing operator: `myMeasure.TranslatedNames["da-DK"]`.

Long story short, you can now export all translations, perspective information, annotations, extended properties, row-level- and object-level security information on objects in your Tabular model.

For example, the following script will produce a TSV file of all model measures and information about which perspectives each is visible in:

```csharp
var tsv = ExportProperties(Model.AllMeasures, "Name,InPerspective");
SaveFile(@"c:\Project\MeasurePerspectives.tsv", tsv);
```

The TSV file looks like this, when opened in Excel:

![image](https://user-images.githubusercontent.com/8976200/85208532-956dec80-b331-11ea-8568-32dbd4cc5516.png)

And just as shown above, you can make changes in Excel, hit save, and then load the updated values back into Tabular Editor using `ImportProperties`.

If you want to list only a specific or a few specific perspectives, you can specify those in the 2nd argument in the call to `ExportProperties`:

```csharp
var tsv = ExportProperties(Model.AllMeasures, "Name,InPerspective[Inventory]");
SaveFile(@"c:\Project\MeasurePerspectiveInventory.tsv", tsv);
```

Similarly, for translations, annotations, etc. For example, if you wanted to see all danish translations applied to tables, columns, hierarchies, levells and measures:

```csharp
// Construct a list of objects:
var objects = new List<TabularNamedObject>();
objects.AddRange(Model.Tables);
objects.AddRange(Model.AllColumns);
objects.AddRange(Model.AllHierarchies);
objects.AddRange(Model.AllLevels);
objects.AddRange(Model.AllMeasures);

var tsv = ExportProperties(objects, "Name,TranslatedNames[da-DK],TranslatedDescriptions[da-DK],TranslatedDisplayFolders[da-DK]");
SaveFile(@"c:\Project\ObjectTranslations.tsv", tsv);
```

***

## Generating documentation
The `ExportProperties` method shown above, can also be used if you want to document all or parts of your model. The following snippet will extract a set of properties from all visible measures or columns in a Tabular Model, and save it as a TSV file:

```csharp
// Construct a list of all visible columns and measures:
var objects = Model.AllMeasures.Where(m => !m.IsHidden && !m.Table.IsHidden).Cast<ITabularNamedObject>()
      .Concat(Model.AllColumns.Where(c => !c.IsHidden && !c.Table.IsHidden));

// Get their properties in TSV format (tabulator-separated):
var tsv = ExportProperties(objects,"Name,ObjectType,Parent,Description,FormatString,DataType,Expression");

// (Optional) Output to screen (can then be copy-pasted into Excel):
// tsv.Output();

// ...or save the TSV to a file:
SaveFile("documentation.tsv", tsv);
```
***

## Generating measures from a file

The above techniques of exporting/importing properties, is useful if you want to edit object properties in bulk of *existing* objects in your model. What if you want to import a list of measures that do not already exist?

Let's say you have a TSV (tab-separated values) file that contains Names, Descriptions and DAX Expressions of measures you'd like to import into an existing Tabular Model. You can use the following script to read in the file, split it out into rows and columns, and generate the measures. The script also assigns a special annotation to each measure, so that it can delete measures that were previously created using the same script.

```csharp
var targetTable = Model.Tables["Program"];  // Name of the table that should hold the measures
var measureMetadata = ReadFile(@"c:\Test\MyMeasures.tsv");   // c:\Test\MyMeasures.tsv is a tab-separated file with a header row and 3 columns: Name, Description, Expression

// Delete all measures from the target table that have an "AUTOGEN" annotation with the value "1":
foreach(var m in targetTable.Measures.Where(m => m.GetAnnotation("AUTOGEN") == "1").ToList())
{
    m.Delete();
}

// Split the file into rows by CR and LF characters:
var tsvRows = measureMetadata.Split(new[] {'\r','\n'},StringSplitOptions.RemoveEmptyEntries);

// Loop through all rows but skip the first one:
foreach(var row in tsvRows.Skip(1))
{
    var tsvColumns = row.Split('\t');     // Assume file uses tabs as column separator
    var name = tsvColumns[0];             // 1st column contains measure name
    var description = tsvColumns[1];      // 2nd column contains measure description
    var expression = tsvColumns[2];       // 3rd column contains measure expression

    // This assumes that the model does not already contain a measure with the same name (if it does, the new measure will get a numeric suffix):
    var measure = targetTable.AddMeasure(name);
    measure.Description = description;
    measure.Expression = expression;
    measure.SetAnnotation("AUTOGEN", "1");  // Set a special annotation on the measure, so we can find it and delete it the next time the script is executed.
}
```

If you need to automate this process, save the above script into a file and use the [Tabular Editor CLI](/Command-line-Options) as follows:

```
start /wait TabularEditor.exe "<path to bim file>" -S "<path to script file>" -B "<path to modified bim file>"
```

for example:

```
start /wait TabularEditor.exe "c:\Projects\AdventureWorks\Model.bim" -S "c:\Projects\AutogenMeasures.cs" -B "c:\Projects\AdventureWorks\Build\Model.bim"
```

...or, if you prefer to run the script against an already deployed database:

```
start /wait TabularEditor.exe "localhost" "AdventureWorks" -S "c:\Projects\AutogenMeasures.cs" -D "localhost" "AdventureWorks" -O
```

***

## Creating Data Columns from Partition Source metadata
**Note:** If you're using version 2.7.2 or newer, make sure to try the new "Import Table..." feature.

If a table uses a Query partition based on an OLE DB provider data source, we can automatically refresh the column metadata of that table by executing the following snippet:
```csharp
Model.Tables["Reseller Sales"].RefreshDataColumns();
```
This is useful when adding new tables to a model, to avoid having to create every Data Column on the table manually. The snippet above assumes that the partition source can be accessed locally, using the existing connection string of the Partition Source for the 'Reseller Sales' table. The snippet above will extract the schema from the partition query, and add a Data Column to the table for every column in the source query.

If you need to supply a different connection string for this operation, you can do that in the snippet as well:
```csharp
var source = Model.DataSources["DWH"] as ProviderDataSource;
var oldConnectionString = source.ConnectionString;
source.ConnectionString = "...";   // Enter the connection string you want to use for metadata refresh
Model.Tables["Reseller Sales"].RefreshDataColumns();
source.ConnectionString = oldConnectionString;
```
This assumes that the partitions of the 'Reseller Sales' table is using a Provider Data Source with the name "DWH".

***

## Format DAX expressions
Please see [FormatDax](/FormatDax) for more information.

```csharp
// Works in Tabular Editor version 2.13.0 or newer:
Selected.Measures.FormatDax();
```

Alternate syntax:

```csharp
// Works in Tabular Editor version 2.13.0 or newer:
foreach(var m in Selected.Measures)
    m.FormatDax();
```

***

## Generate list of source columns for a table
The following script outputs a nicely formatted list of source columns for the currently selected table. This may be useful if you want to replace partition queries that use `SELECT *` with explicit columns.

```csharp
string.Join(",\r\n", 
    Selected.Table.DataColumns
        .OrderBy(c => c.SourceColumn)
        .Select(c => "[" + c.SourceColumn + "]")
    ).Output();
```

***

## Auto-creating relationships
If you’re consistently using a certain set of naming conventions within your team, you’ll quickly find that scripts can be even more powerful.

The following script, when executed on one or more fact tables, will automatically create relationships to all relevant dimension tables, based on column names. The script will search for fact table columns having the name pattern `xxxyyyKey` where the xxx is an optional qualifier for role-playing use, and the yyy is the dimension table name. On the dimension table, a column named `yyyKey` must exist and have the same data type as the column on the fact table. For example, a column named “ProductKey” will be related to the “ProductKey” column on the Product table. You can specify a different column name suffix to use in place of "Key".

If a relationship already exists between the fact and dimension table, the script will create the new relationship as inactive.

```csharp
var keySuffix = "Key";

// Loop through all currently selected tables (assumed to be fact tables):
foreach(var fact in Selected.Tables)
{
    // Loop through all SK columns on the current table:
    foreach(var factColumn in fact.Columns.Where(c => c.Name.EndsWith(keySuffix)))
    {
        // Find the dimension table corresponding to the current SK column:
        var dim = Model.Tables.FirstOrDefault(t => factColumn.Name.EndsWith(t.Name + keySuffix));
        if(dim != null)
        {
            // Find the key column on the dimension table:
            var dimColumn = dim.Columns.FirstOrDefault(c => factColumn.Name.EndsWith(c.Name));
            if(dimColumn != null)
            {
                // Check whether a relationship already exists between the two columns:
                if(!Model.Relationships.Any(r => r.FromColumn == factColumn && r.ToColumn == dimColumn))
                {
                    // If relationships already exists between the two tables, new relationships will be created as inactive:
                    var makeInactive = Model.Relationships.Any(r => r.FromTable == fact && r.ToTable == dim);

                    // Add the new relationship:
                    var rel = Model.AddRelationship();
                    rel.FromColumn = factColumn;
                    rel.ToColumn = dimColumn;
                    factColumn.IsHidden = true;
                    if(makeInactive) rel.IsActive = false;
                }
            }
        }
    }
}
```

***

## Create DumpFilters measure
Inspired by [this article](https://www.sqlbi.com/articles/displaying-filter-context-in-power-bi-tooltips/), here's a script that will create a [DumpFilters] measure on the currently selected table:

```csharp
var dax = "VAR MaxFilters = 3 RETURN ";
var dumpFilterDax = @"IF (
    ISFILTERED ( {0} ), 
    VAR ___f = FILTERS ( {0} )
    VAR ___r = COUNTROWS ( ___f )
    VAR ___t = TOPN ( MaxFilters, ___f, {0} )
    VAR ___d = CONCATENATEX ( ___t, {0}, "", "" )
    VAR ___x = ""{0} = "" & ___d 
        & IF(___r > MaxFilters, "", ... ["" & ___r & "" items selected]"") & "" ""
    RETURN ___x & UNICHAR(13) & UNICHAR(10)
)";

// Loop through all columns of the model to construct the complete DAX expression:
bool first = true;
foreach(var column in Model.AllColumns)
{
    if(!first) dax += " & ";
    dax += string.Format(dumpFilterDax, column.DaxObjectFullName);
    if(first) first = false;
}

// Add the measure to the currently selected table:
Selected.Table.AddMeasure("DumpFilters", dax);
```

***

## CamelCase to Proper Case

A common naming scheme for columns and tables on a relation database, is CamelCase. That is, names do not contain any spaces and individual words start with a capital letter. In a Tabular model, tables and columns that are not hidden, will be visible to business users, and so it would often be preferable to use a "prettier" naming scheme. The following script will convert CamelCased names to Proper Case. Sequences of uppercase letters are kept as-is (acronyms). For example, the script will convert the following:

* `CustomerWorkZipcode` to `Customer Work Zipcode`
* `CustomerAccountID` to `Customer Account ID`
* `NSASecurityID` to `NSA Security ID`

I highly recommend saving this script as a Custom Action that applies to all object types (except Relationships, KPIs, Table Permissions and Translations, as these do not have an editable "Name" property):

```csharp
foreach(var obj in Selected.OfType<ITabularNamedObject>()) {
    var oldName = obj.Name;
    var newName = new System.Text.StringBuilder();
    for(int i = 0; i < oldName.Length; i++) {
        // First letter should always be capitalized:
        if(i == 0) newName.Append(Char.ToUpper(oldName[i]));

        // A sequence of two uppercase letters followed by a lowercase letter should have a space inserted
        // after the first letter:
        else if(i + 2 < oldName.Length && char.IsLower(oldName[i + 2]) && char.IsUpper(oldName[i + 1]) && char.IsUpper(oldName[i]))
        {
            newName.Append(oldName[i]);
            newName.Append(" ");
        }

        // All other sequences of a lowercase letter followed by an uppercase letter, should have a space
        // inserted after the first letter:
        else if(i + 1 < oldName.Length && char.IsLower(oldName[i]) && char.IsUpper(oldName[i+1]))
        {
            newName.Append(oldName[i]);
            newName.Append(" ");
        }
        else
        {
            newName.Append(oldName[i]);
        }
    }
    obj.Name = newName.ToString();
}
```

***

## Exporting dependencies between tables and measures

Let's say you have a large, complex model, and you want to know which measures are potentially affected by changes to the underlying data.

The following script loops through all the measures of your model, and for each measure, it outputs a list of tables that measure depends on - both directly and indirectly. The list is outputted as a Tab-separated file.

```csharp
string tsv = "Measure\tDependsOnTable"; // TSV file header row

// Loop through all measures:
foreach(var m in Model.AllMeasures) {

    // Get a list of ALL objects referenced by this measure (both directly and indirectly through other measures):
    var allReferences = m.DependsOn.Deep();

    // Filter the previous list of references to table references only. For column references, let's get th
    // table that each column belongs to. Finally, keep only distinct tables:
    var allTableReferences = allReferences.OfType<Table>()
        .Concat(allReferences.OfType<Column>().Select(c => c.Table)).Distinct();

    // Output TSV rows - one for each table reference:
    foreach(var t in allTableReferences)
        tsv += string.Format("\r\n{0}\t{1}", m.Name, t.Name);
}
    
tsv.Output();   
// SaveFile("c:\\MyProjects\\SSAS\\MeasureTableDependencies.tsv", tsv); // Uncomment this line to save output to a file
```
***

## Setting up Aggregations (Power BI Dataset only)
As of [Tabular Editor 2.11.3](https://github.com/otykier/TabularEditor/releases/tag/2.11.3), you can now set the `AlternateOf` property on a column, enabling you to define aggregation tables on your model. This feature is enabled for Power BI Datasets (Compatibility Level 1460 or higher) through the Power BI Service XMLA endpoint.

Select a range of columns and run the following script to initiate the `AlternateOf` property on them:

```csharp
foreach(var col in Selected.Columns) col.AddAlternateOf();
```

Work your way through the columns one by one, to map them to the base column and set the summarization accordingly (Sum/Min/Max/GroupBy). Alternatively, if you want to automate this process, and your aggregation table columns have identical names as the base table columns, you can use the following script, which will map the columns for you:

```csharp
// Select two tables in the tree (ctrl+click). The aggregation table is assumed to be the one with fewest columns.
// This script will set up the AlternateOf property on all columns on the aggregation table. Agg table columns must
// have the same name as the base table columns for this script to work.
var aggTable = Selected.Tables.OrderBy(t => t.Columns.Count).First();
var baseTable = Selected.Tables.OrderByDescending(t => t.Columns.Count).First();

foreach(var col in aggTable.Columns)
{
    // The script will set the summarization type to "Group By", unless the column uses data type decimal/double:
    var summarization = SummarizationType.GroupBy;
    if(col.DataType == DataType.Double || col.DataType == DataType.Decimal)
        summarization = SummarizationType.Sum;
    
    col.AddAlternateOf(baseTable.Columns[col.Name], summarization);
}
```

After running the script, you should see that the `AlternateOf` property has been assigned on all columns on your agg table (see screenshot below). Keep in mind, that the base table partition must use DirectQuery for aggregations to work.

![image](https://user-images.githubusercontent.com/8976200/85851134-6ed70800-b7ae-11ea-82eb-37fcaa2ca9c4.png)

***

## Querying Analysis Services

As of version [2.12.1](https://github.com/otykier/TabularEditor/releases/tag/2.12.1), Tabular Editor now provides a number of helper methods for executing DAX queries and evaluating DAX expressions against your model. These methods work only when model metadata have been loaded directly from an instance of Analysis Services, such as when using the "File > Open > From DB..." option, or when using the Power BI external tools integration of Tabular Editor.

The following methods are available:

| Method | Description |
| ------ | ----------- |
| `void ExecuteCommand(string tmsl)` | This methods passes the specified TMSL script to the connected instance of Analysis Services. This is useful when you want to refresh data in a table on the AS instance. Note that if you use this method to perform metadata changes to your model, your local model metadata will become out-of-sync with the metadata on the AS instance, and you may receive a version conflict warning the next time you try to save the model metadata. |
| `IDataReader ExecuteReader(string dax)` | Executes the specified DAX *query* against the connected AS database and returns the resulting [AmoDataReader](https://docs.microsoft.com/en-us/dotnet/api/microsoft.analysisservices.amodatareader?view=analysisservices-dotnet) object. Note that you can not have multiple open data readers at once. Tabular Editor will automatically close them in case you forget to explicitly close or dispose the reader. |
| `DataSet ExecuteDax(string dax)` | Executes the specified DAX *query* against the connected AS database and returns a [DataSet](https://docs.microsoft.com/en-us/dotnet/api/system.data.dataset?view=netframework-4.6) object containing the data returned from the query. Returning very large data tables is not recommended as they may cause out-of-memory or other stability errors. |
| `object EvaluateDax(string dax)` | Executes the specified DAX *expression* against the connected AS database and returns an object representing the result. If the DAX expression is scalar, an object of the relevant type is returned (string, long, decimal, double, DateTime). If the DAX expression is table-valued, a [DataTable](https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable?view=netframework-4.6) is returned. |

The methods are scoped to the `Model.Database` object, but they can also be executed directly without any prefix.

Darren Gosbell presents an interesting use-case of generating data-driven measures using the `ExecuteDax` method [here](https://darren.gosbell.com/2020/08/the-best-way-to-generate-data-driven-measures-in-power-bi-using-tabular-editor/).

Another option is to create a reusable script for refreshing a table. For example, to perform a recalculation, use this:

```csharp
var type = "calculate";
var database = Model.Database.Name;
var table = Selected.Table.Name;
var tmsl = "{ \"refresh\": { \"type\": \"%type%\", \"objects\": [ { \"database\": \"%db%\", \"table\": \"%table%\" } ] } }"
    .Replace("%type%", type)
    .Replace("%db%", database)
    .Replace("%table%", table);

ExecuteCommand(tmsl);
```

You can also use the `Output` helper method to visualize the result of a DAX expression returned from `EvaluateDax` directly:

```csharp
EvaluateDax("1 + 2").Output(); // An integer
EvaluateDax("\"Hello from AS\"").Output(); // A string
EvaluateDax("{ (1, 2, 3) }").Output(); // A table
```

![image](https://user-images.githubusercontent.com/8976200/91638299-bbd59580-ea0e-11ea-882b-55bff73c30fb.png)

...or, if you want to return the value of the currently selected measure:

```csharp
EvaluateDax(Selected.Measure.DaxObjectFullName).Output();
```

![image](https://user-images.githubusercontent.com/8976200/91638367-6f3e8a00-ea0f-11ea-90cd-7d2e4cff6e31.png)

And here's a more advanced example that allows you to select and evaluate multiple measures at once:

```csharp
var dax = "ROW(" + string.Join(",", Selected.Measures.Select(m => "\"" + m.Name + "\", " + m.DaxObjectFullName).ToArray()) + ")";
EvaluateDax(dax).Output();
```
![image](https://user-images.githubusercontent.com/8976200/91638356-546c1580-ea0f-11ea-8302-3e40829e00dd.png)

If you're really advanced, you could use SUMMARIZECOLUMNS or some other DAX function to visualize the selected measure sliced by some column:

```csharp
var dax = "SUMMARIZECOLUMNS('Product'[Color], " + string.Join(",", Selected.Measures.Select(m => "\"" + m.Name + "\", " + m.DaxObjectFullName).ToArray()) + ")";
EvaluateDax(dax).Output();
```

![image](https://user-images.githubusercontent.com/8976200/91638389-9b5a0b00-ea0f-11ea-819f-d3eee3ddfa71.png)

Remember you can save these scripts as Custom Actions by clicking the "+" icon just above the script editor. This way, you get an easily reusable collection of DAX queries that you can execute and visualize directly from inside the Tabular Editor context menu:

![image](https://user-images.githubusercontent.com/8976200/91638790-305e0380-ea12-11ea-9d84-313f4388496f.png)

If you come up with some other interesting uses of these methods, please consider sharing them in the [community scripts repository](https://github.com/TabularEditor/Scripts). Thanks!

***

## Replace Power Query server and database names

Power BI Dataset that import data from SQL Server-based datasources, often contain M expressions that look like the following. Tabular Editor does unfortunately not have any mechanism for "parsing" such an expression, but if we wanted to replace the server and database names in this expression with something else, without knowing the original values, we can exploit the fact that the values are enclosed in double quotes:

```M
let
    Source = Sql.Databases("devsql.database.windows.net"),
    AdventureWorksDW2017 = Source{[Name="AdventureWorks"]}[Data],
    dbo_DimProduct = AdventureWorksDW2017{[Schema="dbo",Item="DimProduct"]}[Data]
in
    dbo_DimProduct
```

The following script will replace the first occurrence of a value in double quotes with a server name, and the second occurrence of a value in double quotes with a database name. Both replacement values are read from environment variables:

```csharp
// This script is used to replace the server and database names across
// all power query partitions, with the ones provided through environment
// variables:
var server = "\"" + Environment.GetEnvironmentVariable("SQLServerName") + "\"";
var database = "\"" + Environment.GetEnvironmentVariable("SQLDatabaseName") + "\"";

// This function will extract all quoted values from the M expression, returning a list of strings
// with the values extracted (in order), but ignoring any quoted values where a hashtag (#) precedes
// the quotation mark:
var split = new Func<string, List<string>>(m => { 
    var result = new List<string>();
    var i = 0;
    foreach(var s in m.Split('"')) {
        if(s.EndsWith("#") && i % 2 == 0) i = -2;
        if(i >= 0 && i % 2 == 1) result.Add(s);
        i++;
    }
    return result;
});
var GetServer = new Func<string, string>(m => split(m)[0]);    // Server name is usually the 1st encountered string
var GetDatabase = new Func<string, string>(m => split(m)[1]);  // Database name is usually the 2nd encountered string

// Loop through all partitions on the model, replacing the server and database names from the partitions
// with the ones specified in environment variables:
foreach(var p in Model.AllPartitions.OfType<MPartition>())
{
    var oldServer = "\"" + GetServer(p.Expression) + "\"";
    var oldDatabase = "\"" + GetDatabase(p.Expression) + "\"";
    p.Expression = p.Expression.Replace(oldServer, server).Replace(oldDatabase, database);
}
```


***

## Replace Power Query data sources and partitions with Legacy

If you are working with a Power BI-based model that uses Power Query (M) expressions for partitions against a SQL Server-based data source, you will unfortunately not be able to use Tabular Editor's Data Import wizard or perform a schema check (i.e. comparing imported columns with columns in the data source).

To solve this issue, you can run the following script on your model, to replace the power query partitions with corresponding native SQL query partitions, and to create a legacy (provider) data source on the model, which will work with Tabular Editor's Import Data wizard:

There are two versions of the script: The first one uses the MSOLEDBSQL provider for the created legacy data source, and hardcoded credentials. This is useful for local development. The second one uses the SQLNCLI provider, which is available on Microsoft-hosted build agents on Azure DevOps, and reads credentials and server/database names from environment variables, making the script useful for integration in Azure Pipeliens.

MSOLEDBSQL version, which reads connection information from M partitions and prompts for user name and password through Azure AD:
```csharp
#r "Microsoft.VisualBasic"

// This script replaces all Power Query partitions on this model with a
// legacy partition using the provided connection string with INTERACTIVE
// AAD authentication. The script assumes that all Power Query partitions
// load data from the same SQL Server-based data source.

// Provide the following information:
var authMode = "ActiveDirectoryInteractive";
var userId = Microsoft.VisualBasic.Interaction.InputBox("Type your AAD user name", "User name", "name@domain.com", 0, 0);
if(userId == "") return;
var password = ""; // Leave blank when using ActiveDirectoryInteractive authentication

// This function will extract all quoted values from the M expression, returning a list of strings
// with the values extracted (in order), but ignoring any quoted values where a hashtag (#) precedes
// the quotation mark:
var split = new Func<string, List<string>>(m => { 
    var result = new List<string>();
    var i = 0;
    foreach(var s in m.Split('"')) {
        if(s.EndsWith("#") && i % 2 == 0) i = -2;
        if(i >= 0 && i % 2 == 1) result.Add(s);
        i++;
    }
    return result;
});
var GetServer = new Func<string, string>(m => split(m)[0]);    // Server name is usually the 1st encountered string
var GetDatabase = new Func<string, string>(m => split(m)[1]);  // Database name is usually the 2nd encountered string
var GetSchema = new Func<string, string>(m => split(m)[2]);    // Schema name is usually the 3rd encountered string
var GetTable = new Func<string, string>(m => split(m)[3]);     // Table name is usually the 4th encountered string

var server = GetServer(Model.AllPartitions.OfType<MPartition>().First().Expression);
var database = GetDatabase(Model.AllPartitions.OfType<MPartition>().First().Expression);

// Add a legacy data source to the model:
var ds = Model.AddDataSource("AzureSQL");
ds.Provider = "System.Data.OleDb";
ds.ConnectionString = string.Format(
    "Provider=MSOLEDBSQL;Data Source={0};Initial Catalog={1};Authentication={2};User ID={3};Password={4}",
    server,
    database,
    authMode,
    userId,
    password);

// Remove Power Query partitions from all tables and replace them with a single Legacy partition:
foreach(var t in Model.Tables)
{
    var mPartitions = t.Partitions.OfType<MPartition>();
    if(!mPartitions.Any()) continue;
    var schema = GetSchema(mPartitions.First().Expression);
    var table = GetTable(mPartitions.First().Expression);
    t.AddPartition(t.Name, string.Format("SELECT * FROM [{0}].[{1}]", schema, table));
    foreach(var p in mPartitions.ToList()) p.Delete();
}
```

SQLNCLI version reading connection info from environment variables:
```csharp
// This script replaces all Power Query partitions on this model with a
// legacy partition, reading the SQL server name, database name, user name
// and password from corresponding environment variables. The script assumes
// that all Power Query partitions load data from the same SQL Server-based
// data source.

var server = Environment.GetEnvironmentVariable("SQLServerName");
var database = Environment.GetEnvironmentVariable("SQLDatabaseName");
var userId = Environment.GetEnvironmentVariable("SQLUserName");
var password = Environment.GetEnvironmentVariable("SQLUserPassword");

// This function will extract all quoted values from the M expression, returning a list of strings
// with the values extracted (in order), but ignoring any quoted values where a hashtag (#) precedes
// the quotation mark:
var split = new Func<string, List<string>>(m => { 
    var result = new List<string>();
    var i = 0;
    foreach(var s in m.Split('"')) {
        if(s.EndsWith("#") && i % 2 == 0) i = -2;
        if(i >= 0 && i % 2 == 1) result.Add(s);
        i++;
    }
    return result;
});
var GetServer = new Func<string, string>(m => split(m)[0]);    // Server name is usually the 1st encountered string
var GetDatabase = new Func<string, string>(m => split(m)[1]);  // Database name is usually the 2nd encountered string
var GetSchema = new Func<string, string>(m => split(m)[2]);    // Schema name is usually the 3rd encountered string
var GetTable = new Func<string, string>(m => split(m)[3]);     // Table name is usually the 4th encountered string

// Add a legacy data source to the model:
var ds = Model.AddDataSource("AzureSQL");
ds.Provider = "System.Data.SqlClient";
ds.ConnectionString = string.Format(
    "Server={0};Initial Catalog={1};Persist Security Info=False;User ID={2};Password={3}",
    server,
    database,
    userId,
    password);

// Remove Power Query partitions from all tables and replace them with a single Legacy partition:
foreach(var t in Model.Tables)
{
    var mPartitions = t.Partitions.OfType<MPartition>();
    if(!mPartitions.Any()) continue;
    var schema = GetSchema(mPartitions.First().Expression);
    var table = GetTable(mPartitions.First().Expression);
    t.AddPartition(t.Name, string.Format("SELECT * FROM [{0}].[{1}]", schema, table));
    foreach(var p in mPartitions.ToList()) p.Delete();
}
```



Generating a dynamic measure selector:
```csharp
// For certain kinds of reports, it sometimes makes sense to be able to select which measures should be
// displayed by checking off members on a dimension, rather than including individual measures from
// the field list


// (1) Name of disconnected selector table:
var selectorTableName = "Measure Selector";

// (2) Name of column on selector table:
var selectorTableColumnName = "Measure";

// (3) Name of dynamic switch measure:
var dynamicMeasureName = "Dynamic Measure";

// (4) Name of dynamic switch measure's parent table:
var dynamicMeasureTableName = "Measure Selector";

// (5) Fallback DAX expression:
var fallbackDax = "BLANK()";

// ----- Do not modify script below this line -----

if(Selected.Measures.Count == 0) {
    Error("Select one or more measures");
    return;
}

// Get or create selector table:
CalculatedTable selectorTable;
if(!Model.Tables.Contains(selectorTableName)) Model.AddCalculatedTable(selectorTableName);
selectorTable = Model.Tables[selectorTableName] as CalculatedTable;

// Get or create dynamic measure:
Measure dynamicMeasure;
if(!Model.Tables[dynamicMeasureTableName].Measures.Contains(dynamicMeasureName))
    Model.Tables[dynamicMeasureTableName].AddMeasure(dynamicMeasureName);
dynamicMeasure = Model.Tables[dynamicMeasureTableName].Measures[dynamicMeasureName];

// Generate DAX for disconnected table:
// SELECTCOLUMNS({"Measure 1", "Measure 2", ...}, "Measure", [Value])
var selectorTableDax = "SELECTCOLUMNS(\n    {\n        " +
    string.Join(",\n        ", Selected.Measures.Select(m => "\"" + m.Name + "\"").ToArray()) +
    "\n    },\n    \"" + selectorTableColumnName + "\", [Value]\n)";

// Generate DAX for dynamic metric:
// VAR _s = SELECTEDVALUE('Metric Selection'[Value]) RETURN SWITCH(_s, ...)
var dynamicMeasureDax = 
    "VAR _s =\n    SELECTEDVALUE('" + selectorTableName + "'[" + selectorTableColumnName + "])\n" +
    "RETURN\n    SWITCH(\n        _s,\n        " +
    string.Join(",\n        ", Selected.Measures.Select(m => "\"" + m.Name + "\", " + m.DaxObjectFullName).ToArray()) +
    ",\n        " + fallbackDax + "\n    )";

// Assign DAX expressions:
selectorTable.Expression = selectorTableDax;
dynamicMeasure.Expression = dynamicMeasureDax;

```
