# Master Model Pattern

It is not uncommon to have several Tabular models in an organisation, with a substantial amount of functional overlap. For the development team, keeping these models up to date with shared features can be a pain point. In this article, we'll see an alternate approach that may be suitable in situations where it makes sense to combine all these models into a single "Master" model, that is then deployed partially into several different subset models. Tabular Editor enables this approach by utilising perspectives in a special way (while still allowing perspectives to work the usual way).

**Disclaimer:** While this technique works, it is not supported by Microsoft, and there is a fair amount of learning, scripting and hacking involved. Decide for yourself whether you think it's the right approach for your team.

For simplicity, consider the AdventureWorks sample model:

![image](https://user-images.githubusercontent.com/8976200/43959290-895c1c96-9cae-11e8-8112-008f54cb400a.png)

Let's say that for some reason, these is a need to deploy everything relating to Internet Sales as one model, and everything relating to Reseller Sales as another. This could be for security reasons, performance, scalability, or maybe even because your team is servicing a number of external clients, where each client needs their own copy of the model, containing both shared and specific functionality.

Instead of actually maintaining one development branch for each of the different versions, the technique presented here, lets you maintain just one model using metadata to indicate how the model should be split upon deployment.

## (Ab)using perspectives
The idea is quite simple. Start by adding a number of new perspectives to your model, corresponding to the number of target models you need to deploy to. Make sure to prefix these perspectives in a consistent way, to separate them from user-oriented perspectives:

![image](https://user-images.githubusercontent.com/8976200/43960154-6b637042-9cb1-11e8-906b-6671bbb9558e.png)

Here, we use a ``$``-sign as the prefix on the perspective names. Later on we will see how these perspectives are stripped from the model, so that end users will not see them. They are only used by the model developers.

Now, simply add all objects needed in the individual models to these perspectives. Use the Perspective dropdown in Tabular Editor to confirm that a model contains the necessary objects. Here's a handy script that can be used to ensure that all dependencies are included in the perspective as well:

```csharp
// Look through all hierarchies in the current perspective:
foreach(var h in Model.AllHierarchies.Where(h => h.InPerspective[Selected.Perspective]))
{
    // Make sure columns used in hierarchy levels are included in the perspective:
    foreach(var level in h.Levels) {
        level.Column.InPerspective[Selected.Perspective] = true;
    }
}

// Loop through all measures and columns in the current perspective:
foreach(var obj in Model.AllMeasures.Cast<ITabularPerspectiveObject>()
    .Concat(Model.AllColumns).Where(m => m.InPerspective[Selected.Perspective])
    .OfType<IDaxDependantObject>().ToList())
{
    // Loop through all objects that the current object depends on:
    foreach(var dep in obj.DependsOn.Deep())
    {
        // Include columns, measure and table dependencies:
        var columnDep = dep as Column; if(columnDep != null) columnDep.InPerspective[Selected.Perspective] = true;
        var measureDep = dep as Measure; if(measureDep != null) measureDep.InPerspective[Selected.Perspective] = true;
        var tableDep = dep as Table; if(tableDep != null) tableDep.InPerspective[Selected.Perspective] = true;
    }    
}

// Look through all columns that have a SortByColumn in the current perspective:
foreach(var c in Model.AllColumns.Where(c => c.InPerspective[Selected.Perspective] && c.SortByColumn != null))
{
    c.SortByColumn.InPerspective[Selected.Perspective] = true;   
}
```
**Explanation:** First, the script loops through all hierarchies in the current perspective (the perspective currently selected in the dropdown at the top of the screen). For every such hierarchy, it ensures that all columns used as hierarchy levels appear in the perspective. Next, the script loops through all columns and measures of the current perspective. For each of these objects, all DAX dependencies in the form of measure-, column- or table references are also included in the perspective. Please note that expressions such as `DISTINCTCOUNT('Customer'[CustomerId])` will result in all columns of the 'Customer' table being included in the perspective, as Tabular Editor treats such an expression as having a dependency both on the [CustomerId] column itself, and on the 'Customer' table. Lastly, the script ensures that any columns that are used as a "Sort By"-column, are also included in the perspective.

I recommend saving this script as a Custom Action at the Model level, to make it easy to invoke it going forward.

By the way, if you want to make a copy of a perspective, you can already do that through the UI. Click on the "Perspectives" node in the explorer tree, and then click the ellipsis button in the property grid:

![image](https://user-images.githubusercontent.com/8976200/44028910-c7ffab80-9efb-11e8-813a-5b0f5c137bab.png)

This will open a dialog that lets you create and delete perspectives, as well as clone existing perspectives:

![image](https://user-images.githubusercontent.com/8976200/44028953-f13c91ca-9efb-11e8-936a-1f0e1d4eb93f.png)

To supplement this, here's a script that removes all invisible and unused objects from a perspective, in case you need to clean up a bit:

```csharp
// Loop through all columns of the current perspective:
foreach(var c in Model.AllColumns.Where(c => c.InPerspective[Selected.Perspective])) {
    if(
        // If the column is hidden (or the parent table is hidden):
        (c.IsHidden || c.Table.IsHidden) 

        // And not used in any relationships:
        && !c.UsedInRelationships.Any()
        
        // And not used as the SortByColumn for any other columns in the perspective:
        && !c.UsedInSortBy.Any(sb => !sb.IsHidden && sb.InPerspective[Selected.Perspective])
        
        // And not used in any hierarchies in the perspective:
        && !c.UsedInHierarchies.Any(h => h.InPerspective[Selected.Perspective])
        
        // And not referenced in any DAX expression for other visible objects in the perspective:
        && !c.ReferencedBy.Deep().OfType<ITabularPerspectiveObject>()
            .Any(obj => obj.InPerspective[Selected.Perspective] && !(obj as IHideableObject).IsHidden)
            
        // And not referenced by any roles:
        && !c.ReferencedBy.Roles.Any()    )
    {
        // If all of the above, then the column can be removed from the current perspective:
        c.InPerspective[Selected.Perspective] = false; 
    }
}

// Loop through all measures of the current perspective:
foreach(var m in Model.AllMeasures.Where(m => m.InPerspective[Selected.Perspective])) {
    if(
        // If the measure is hidden (or the parent table is hidden):
        (m.IsHidden || m.Table.IsHidden) 

        // And not referenced in any DAX expression for other visible objects in the perspective:
        && !m.ReferencedBy.Deep().OfType<ITabularPerspectiveObject>()
            .Any(obj => obj.InPerspective[Selected.Perspective] && !(obj as IHideableObject).IsHidden)
    )
    {
        // If all of the above, then the column can be removed from the current perspective:
        m.InPerspective[Selected.Perspective] = false; 
    }
}
```
**Explanation:** The script first loops through all columns of the currently selected perspective. It removes a column from the perspective only if all of the following are true:
* The column is hidden (or the table in which the column resides is hidden)
* The column does not participate in any relationships
* The column is not used as the SortByColumn of any other visible column in the perspective
* The column is not used as a level in any hierarchies in the perspective
* The column is not directly or indirectly referenced in any DAX expressions on other visible objects in the perspective
* The column is not used in any row level filter expressions

For measures, we do the same thing, but simplified to only remove measures that meet the following criteria:
* The measure is hidden (or the table in which the measure resides is hidden)
* The measure is not directly or indirectly referenced in any DAX expressions on other visible objects in the perspective

If you're a team of developers working on the model, you should already be using Tabular Editors ["Save to Folder" functionality](/Advanced-features#folder-serialization) together with a source control environment such as Git. Make sure to check the "Serialize perspectives per-object" option under "File" > "Preferences" > "Save to Folder", to avoid getting heaps of merge conflicts on your perspective definitions.

![image](https://user-images.githubusercontent.com/8976200/44029969-935e0efe-9eff-11e8-93de-c1223f7ebe7f.png)

## Adding more fine-grained control
By now, you've probably guessed that we're going to use scripting to create one version of the model for every of our prefixed developer perspectives. The script will simply remove all objects from the model, that are not included in a given developer perspective. However, before we do that, there are a couple more situations we need to handle.

### Controlling non-perspective objects
Some objects, such as perspectives, data sources and roles, are not included nor excluded from perspectives themselves, but we may still need a way to specify which of our model versions they should belong to. For this, we're going to use annotations. So going back to our Adventure Works model, we may want the "Inventory" and "Internet Operation" perspectives to appear in "$InternetModel" and "$ManagementModel", while "Reseller Operation" should appear in "$ResellerModel" and "$ManagementModel".

So let's add a new annotation called "DevPerspectives" on each of the 3 original perspectives, and let's just supply the names of the developer perspectives as a comma-separated string:

![image](https://user-images.githubusercontent.com/8976200/44032304-01bdcc70-9f07-11e8-9b28-db0912ea1ade.png)

When adding new *user* perspectives to the model, remember to add the same annotation and provide the names of the developer perspectives that you want the *user* perspective included in. When scripting the final model versions later on, we will use the information in these annotations to include the perspectives needed. We can do the same thing for data sources and roles.

### Controlling object metadata
There may also be situations where the same measure should have slightly different expressions or format strings across the different model versions. Again, we can use annotation to provide the metadata per developer perspective, and then apply the metadata when we script out the final model.

The easiest way to get all object properties serialized into text, would probably be the [ExportProperties](/Useful-script-snippets#export-object-properties-to-a-file) script function. However, that’s a little overkill for our use case, so let’s just specify directly which properties we want to store as annotations. Create the following script:
```csharp
foreach(var m in Selected.Measures) { 
    m.SetAnnotation(Selected.Perspective.Name + "_Expression", m.Expression);
    m.SetAnnotation(Selected.Perspective.Name + "_FormatString", m.FormatString);
    m.SetAnnotation(Selected.Perspective.Name + "_Description", m.Description);
}
```
And save it as a custom action named "Save Metadata as Annotations":

![image](https://user-images.githubusercontent.com/8976200/44033695-7a754482-9f0b-11e8-937b-0bc0987ce7cb.png)

Similarly, save the following script as a custom action called "Load Metadata from Annotations":
```csharp
foreach(Measure m in Selected.Measures) { 
    var expr = m.GetAnnotation(Selected.Perspective.Name + "_Expression"); if(expr == null) continue;
    m.Expression = expr;
    m.FormatString = m.GetAnnotation(Selected.Perspective.Name + "_FormatString");
    m.Description = m.GetAnnotation(Selected.Perspective.Name + "_Description");
}
```

The idea is that we create one annotation for each of the properties we would like to maintain different versions of, per developer perspective. If you need to maintain other properties than those shown in the script (Expression, FormatString, Description) separately, just add them to the script. You can do the same thing for other object types, but it probably won't make sense for much other than measures and perhaps calculated columns and partitions (to maintain different query expressions per model version, for example).

Use your new custom actions to apply model version specific changes to the developer perspectives (or add the annotations by hand). For example, in our Adventure Works sample, we want the [Day Count] measure to have a different expression in the $ResellerModel perspective, so we apply the changes to the measure, and invoke the "Save Metadata as Annotations" action while having selected the "$ResellerModel" perspective in the dropdown:

![image](https://user-images.githubusercontent.com/8976200/44033944-3104e414-9f0c-11e8-9f06-396bf85a0e4f.png)

In the screenshot above, we have 3 annotations for each of the developer perspectives. In reality, though, we would only need to create these annotations for those developer perspectives where the properties should differ from their native values.

## Altering partition queries
We can use a similar technique to apply changes to partition queries between the different versions. For example, we may want different SQL `WHERE` criterias on some partition queries depending on the version. Let's start by creating a set of new annotations on our *table* objects, to specify the base SQL query we want our partitions to use for each version. Here, for example, we want to restrict which records are included in the Product table on two of our three versions:

![image](https://user-images.githubusercontent.com/8976200/44736562-69221580-aaa4-11e8-82ee-88388015d30d.png)

For tables that have multiple partitions, we specify the WHERE criteria using "placeholders", that will be replaced later on:

![image](https://user-images.githubusercontent.com/8976200/44737015-b3f05d00-aaa5-11e8-9bad-cadd5b4dae35.png)

Define the placeholder values within each partition (note, you must be using [Tabular Editor v. 2.7.3](https://github.com/otykier/TabularEditor/releases/tag/2.7.3) or newer to edit partition annotations through the UI):

![image](https://user-images.githubusercontent.com/8976200/44737199-2a8d5a80-aaa6-11e8-8813-8189b593da98.png)

In dynamic partitioning scenarios, don't forget to include these annotations in the script you're using when creating the new partitions. In the next section, we'll see how to apply these placeholder values during deployment.

## Deploying different versions
Finally, we are ready to deploy our model as 3 different versions. Unfortunately, the Deployment Wizard UI in Tabular Editor cannot split up the model for us based on the perspectives and annotations we created, so we'd have to create an additional script, that strips down our model to a specific version. This script can then be executed as part of a command-line deployment, so that the whole deployment process can be packaged in a command file, a PowerShell executable or maybe even integrated in your build/automated deployment process?

The script we need looks like the following. The idea is that we create one script per developer perspective. Save the script as a text file and name it something like `ResellerModel.cs`:

```csharp
var version = "`$`ResellerModel"; // TODO: Replace this with the name of your developer perspective

// Remove tables, measures, columns and hierarchies that are not part of the perspective:
foreach(var t in Model.Tables.ToList()) {
    if(!t.InPerspective[version]) t.Delete();
    else {
        foreach(var m in t.Measures.ToList()) if(!m.InPerspective[version]) m.Delete();   
        foreach(var c in t.Columns.ToList()) if(!c.InPerspective[version]) c.Delete();
        foreach(var h in t.Hierarchies.ToList()) if(!h.InPerspective[version]) h.Delete();
    }
}

// Remove user perspectives based on annotations and all developer perspectives:
foreach(var p in Model.Perspectives.ToList()) {
    if(p.Name.StartsWith("`$`")) p.Delete();

    // Keep all other perspectives that do not have the "DevPerspectives" annotation, while removing
    // those that have the annotation, if <version> is not specified in the annotation:
    if(p.GetAnnotation("DevPerspectives") != null && !p.GetAnnotation("DevPerspectives").Contains(version)) 
        p.Delete();
}

// Remove data sources based on annotations:
foreach(var ds in Model.DataSources.ToList()) {
    if(ds.GetAnnotation("DevPerspectives") == null) continue;
    if(!ds.GetAnnotation("DevPerspectives").Contains(version)) ds.Delete();
}

// Remove roles based on annotations:
foreach(var r in Model.Roles.ToList()) {
    if(r.GetAnnotation("DevPerspectives") == null) continue;
    if(!r.GetAnnotation("DevPerspectives").Contains(version)) r.Delete();
}

// Modify measures based on annotations:
foreach(Measure m in Model.AllMeasures) {
    var expr = m.GetAnnotation(version + "_Expression"); if(expr == null) continue;
    m.Expression = expr;
    m.FormatString = m.GetAnnotation(version + "_FormatString");
    m.Description = m.GetAnnotation(version + "_Description");    
}

// Set partition queries according to annotations:
foreach(Table t in Model.Tables) {
    var queryWithPlaceholders = t.GetAnnotation(version + "_PartitionQuery"); if(queryWithPlaceholders == null) continue;
    
    // Loop through all partitions in this table:
    foreach(Partition p in t.Partitions) {
        
        var finalQuery = queryWithPlaceholders;

        // Replace all placeholder values:
        foreach(var placeholder in p.Annotations.Keys) {
            finalQuery = finalQuery.Replace("%" + placeholder + "%", p.GetAnnotation(placeholder));
        }

        p.Query = finalQuery;
    }
}

// TODO: Modify other objects based on annotations, if applicable...
```

**Explanation:** First, we remove all tables, columns, measures and hierarchies, that are not part of the perspective defined in line 1 of the script. Then, we remove any additional objects where we may have applied the "DevPerspectives" annotation as described previously, along with all the developer perspectives themselves. Afterwards, we apply any changes to measure expressions, format strings or descriptions based on the annotations, if any. Finally, we apply partition queries as defined in annotations (if any), while also replacing placeholder values with the annotated values (if any).

Note that we could also just add additional specific model changes directly to this script, if we wanted to, but the whole point of this exercise was how we can maintain several models directly from within Tabular Editor. The script above is the same, regardless of which version we want to deploy (except, of course, for line 1).

Finally, we can load our Model.bim file, execute the script, and deploy the modified model in one go, using the following [command line syntax](/Command-line-Options):

```sh
start /wait /d "c:\Program Files (x86)\Tabular Editor" TabularEditor.exe Model.bim -S ResellerModel.cs -D localhost AdventureWorksReseller -O -R
```
To deploy the Internet or Management versions, we would need to do the same, providing the corresponding scripts:
```sh
start /wait /d "c:\Program Files (x86)\Tabular Editor" TabularEditor.exe Model.bim -S InternetModel.cs -D localhost AdventureWorksInternet -O -R
start /wait /d "c:\Program Files (x86)\Tabular Editor" TabularEditor.exe Model.bim -S ManagementModel.cs -D localhost AdventureWorksManagement -O -R
```

This assumes that you are executing the command line within the directory of your Model.bim file (or Database.json file if using the "Save to Folder"-functionality). The -S switch instructs Tabular Editor to apply the supplied script to the model, and the -D switch performs the deployment. The -O switch allows overwriting an existing database with the same name, and the -R switch indicates that we also want to overwrite roles of the target database.

## Master model processing
If you have a dedicated processing server and large amounts of data overlap between the individual models, it may make sense for you to process the data into the master model first, before splitting it up. This way, you can avoid processing the same data several times, into individual models. **This assumes, however, that you are not processing any tables where the partition query has been changed between versions, as shown in [this section](/Master-model-pattern#altering-partition-queries).** The recipe for this is outlined below:

1. (Optional - in case there were metadata changes) Deploy your master model to your processing server
2. Perform the processing you need on your master model (do not process tables that have version-specific partition queries).
3. Synchronise the master model into every individual model and use the command above to strip down the individual models after synchronisation, followed by a ProcessRecalc if necessary.
4. (Optional) Process any tables on the individual models, that have version-specific partition queries.

## Tips and tricks
When you're starting to use custom annotations a lot, there may be situations where you want to list all objects with a specific annotation. This is where the Dynamic LINQ expressions of the Filter-box comes in handy.

First off, let's say we wanted to find all objects where we added an annotation with the name "$InternetModel_Expression". Type the following into the filter textbox and hit ENTER:

```
:GetAnnotation("`$`InternetModel_Expression")<>null
```

Or, if you want to find all objects, that have an annotation ending with the word "_Expression", use:
```
:GetAnnotations().Any(EndsWith("_Expression"))
```

Note that these functions are case-sensitive, so if your annotation was written in lowercase, the above filter would not catch it.

You could also search for objects where the annotation had a specific value:

```
:GetAnnotation(`$`InternetModel_Description).Contains("TODO")
```

## Conclusion
The technique described here can be very helpful when maintaining many similar models with a lots of shared functionality, such as Calendar tables and other common dimensions. The scripts used can be neatly reused as Custom Actions within Tabular Editor, while the actual deployment can be automated in various ways.