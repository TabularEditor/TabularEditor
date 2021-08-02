# Importing Tables

If you already have a Legacy Data Source in your model, you can right click it, and choose "Import Tables...". Tabular Editor will attempt to connect using the data provider and credentials specified in the Data Source. If successful, you should get a list of all the databases, tables and views accessible through the Data Source:

![image](https://user-images.githubusercontent.com/8976200/49701892-35ea3900-fbf2-11e8-951a-8858179426c6.png)

Clicking a table or view on the left-hand side will display a preview of the data on the right. You can deselect columns that you do not want to include, although [the data import best practice](https://www.sqlbi.com/articles/data-import-best-practices-in-power-bi/) suggests to always use views, and include only columns in those views that are needed in the Tabular Model. The UI will show you the resulting SQL query. By default, Tabular Editor will import a table/view using `SELECT * FROM ...`, but if you toggle any column in the preview, the resulting query will include an explicit list of columns. To switch back to `SELECT * FROM ...`, toggle the "Select all columns" checkbox in the upper right corner.

You can select multiple tables/views to import at once. When you click "Import", all selected tables/views will be imported as new tables with all columns populated from the metadata. A single partition will be created on each table, holding the resulting SQL query from the UI.

That's it! No more going back and forth between Tabular Editor and SSDT.

## A note on Legacy vs. Structured Data Sources
As there is currently no way for Tabular Editor to infer the metadata returned from M (Power Query) expressions, this UI only supports Legacy (aka. Provider) Data Sources. If you must use Structured Data Sources, you can still use a temporary Legacy connection to import the table schema initially (assuming your data source can be accessed through SQL, OLE DB or ODBC), and then manually switch the partitions on the imported tables, to use the Structured Data Sources. If you are importing data from "exotic" data sources, such as web services, Azure Data Lake Storage, etc. schema metadata can not be imported automatically, but [there is an option for providing the metadata information through the clipboard](/Importing-Tables#power-query-data-sources).

In general, though, it is recommended to always use a Legacy connection for the following types of sources:

* SQL Server databases
* Azure SQL Databases
* Azure SQL Data Warehouse
* Azure Databricks (through ODBC)
* Any relational OLEDB data source
* Any relational ODBC source

For authentication using Azure Active Directory with MFA, please see here.

## Importing without a pre-existing Data Source

If your model does not yet contain any data sources, you can import tables by going to the "Model" menu and clicking "Import Tables...". The resulting UI looks like this:

![image](https://user-images.githubusercontent.com/8976200/49702141-74cdbe00-fbf5-11e8-8a88-5bc2a0a6c80d.png)

Leaving the selection at "Create a new Data Source and add it to the model" will display the Connection Dialog UI when clicking "Next". This dialog lets you specify the connection details:

![image](https://user-images.githubusercontent.com/8976200/49702167-a5adf300-fbf5-11e8-8d06-d6670ad456d4.png)

When clicking "OK", a (Legacy) Data Source using the specified connection will be created in your model, and you will be taken to the import page shown above.

The next option on the list, "Use a temporary connection", will not cause a new Data Source to be added to the model. This means that you are responsible for assigning a Data Source to the partitions of the newly imported table, before deploying the model.

The last option, "Manually import metadata from another application", is used when you want to import a new table based on a list of column metadata. This is useful for Structured (Power Query) Data Sources, [see below](/Importing-Tables#power-query-data-sources).

## SQL capabilities
For non-SQL Server data sources (or more precisely, data sources that do not use the Native SQL Client driver), please pay attention to the two dropdown-boxes near the bottom of the screen:

![image](https://user-images.githubusercontent.com/8976200/51613859-b952b600-1f24-11e9-8fd7-7c5269aaab26.png)

The "Reduce rows using"-dropdown lets you specify which row reduction clause to use, when querying the source for preview data, since the Table Import Wizard will only retrieve 200 rows of data from the source table or view. You can choose between the most common row reduction clauses, such as "TOP", "LIMIT", "FETCH FIRST", etc.

The "Identifier quotes"-dropdown lets you specify how object names (column, tables) should be quoted in the generated SQL statements. This applies to both the data preview, as well as the SQL statement used in the table partition query, when the table is imported to the tabular model. By default, square brackets are used, but this can be changed to other common types of identifier quotes.

## Changing the source of a table

Another way to bring up the import page, is to right-click on an existing table (that uses a Legacy Data Source), and choose "Select Columns...". If that table was previously imported using the UI, the import page should show up with the source table/view and imported columns pre-selected. You may add/remove columns or even choose an entirely different table to be imported in place of the table you selected in your model. Keep in mind that any columns in your table, that were deselected or no longer exists in your source table/view will be removed from your model. You can always undo operations such as this using CTRL+Z.

## Refreshing Table Metadata

As of version 2.8, Tabular Editor has a new UI feature that lets you easily check for schema drift. That is, detecting columns that had their data type changed, or were added or removed to source tables and views. This check may be invoked at the Model level (again, this only applies to Legacy Data Sources), at the Data Source level, at the Table level or at the Partition level. This is done by right-clicking the object and choosing "Refresh Table Metadata..."

![image](https://user-images.githubusercontent.com/8976200/49702346-7e582580-fbf7-11e8-9a62-04c6963179e5.png)

Changes are detected based on the "Source Column" and "Data Type" properties of all data columns on the respective tables. If any changes are detected, Tabular Editor will display the above UI, detailing the changes. You may deselect changes that you do not want to apply to your model, although keep in mind that some changes may cause processing errors (for example, source columns that do not exist in the source table/view/query).

This mechanism (as well as the Import Table UI) uses the FormatOnly-flag, when querying the metadata from the source. This means that you can have table partitions that use Stored Procedures. The FormatOnly-flag ensures that the Stored Proc is never executed directly. Instead, static analysis is performed by the server, in order to return only metadata describing the result set that would be returned from the Stored Proc upon execution. Depending on your RDBMS, there may be some limitations of the FormatOnly-flag when used with Stored Procedures. For more information on this topic when using SQL Server as a data source, please see [this article](https://docs.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-describe-first-result-set-transact-sql?view=sql-server-2017#remarks).

### CLI support

You can perform a schema check at the model level from the command line by using the `-SC` flag. Note that the schema check, when executed through the CLI, will only report mapping issues. It will not make any changes to your model. This is useful if you're using Tabular Editor within CI/CD pipelines, as mapping issues could potentially cause problems after deploying your model to a test/production environment.

### Ignoring objects

As of Tabular Editor 2.9.8, you can exclude objects from schema checks / metadata refresh. This is controlled by setting an annotation on the objects that you wish to leave out. As the annotation name, use the codes listed below. You can leave the annotation value blank or set it to "1", "true" or "yes". Setting the annotation value to "0", "false" or "no" will effectively disable the annotation, as if it didn't exist:

**Table flags:**
- `TabularEditor_SkipSchemaCheck`: Causes Tabular Editor to completely skip a schema check on this table.
- `TabularEditor_IgnoreSourceColumnAdded`: Tabular Editor will ignore additional columns that are not mapped to any table columns on this table.
- `TabularEditor_IgnoreDataTypeChange`: Tabular Editor will ignore mismatched data types on any column of the table.
- `TabularEditor_IgnoreMissingSourceColumn`: Tabular Editor will ignore imported columns where the source column apparently does not exist in the source.

**Column flags:**
- `TabularEditor_IgnoreDataTypeChange`: Tabular Editor will ignore mismatched data type on this specific column.
- `TabularEditor_IgnoreMissingSourceColumn`: Tabular Editor will ignore an apparently missing source column for this specific column.

The flags impact schema checking through both the UI and the CLI.

### Treating warnings as errors

By default, the CLI will report an error when a partition query could not be executed, or when the imported table contains a column that does not match any column in the source query. The CLI will report a warning when a column's data type does not match the column in the source query, or if the source query contains columns that are not mapped to any columns in the imported table. The CLI will also report a warning when source queries of different partitions on the same table, do not return the same columns.

Starting with Tabular Editor version 2.14.1, you can change the behaviour of the CLI such that all warnings as listed above are reported as errors. To do this, add the following annotation at the **model** level:

- `TabularEditor_SchemaCheckNoWarnings`: Causes Tabular Editor to treat all schema check warnings as errors.

## Azure Active Directory with MFA

If you want to import tables from an Azure SQL Database or Azure Synapse SQL pool, you will likely need Azure Active Directory multi-factor authentication. Unfortunately, this is not supported by the SQL Native Client provider used in .NET Framework. Instead, use the MSOLEDBSQL provider (which also has the benefit that it is generally faster than the native client, when Analysis Services reads data from the table). Make sure you have the [latest (x86) version](https://docs.microsoft.com/en-us/sql/connect/oledb/download-oledb-driver-for-sql-server?view=sql-server-ver15) of this driver installed, to make this work on your local machine.

Here are step by step instructions to set up the data source to work with MFA:

1. Create a new legacy data source and add it to your model. Model > New Data Source (Legacy)
2. Specify `System.Data.OleDb` as the Provider property and use a connection string that looks as follows, substituting the correct server, database and user names:

### For Synapse SQL pools:
```
Provider=MSOLEDBSQL;Data Source=<synapse workspace name>-ondemand.sql.azuresynapse.net;User ID=daniel@adventureworks.com;Database=<database name>;Authentication=ActiveDirectoryInteractive
```
### For Azure SQL databases:
```
Provider=MSOLEDBSQL;Data Source=<sql server name>.database.windows.net;User ID=daniel@adventureworks.com;Database=<database name>;Authentication=ActiveDirectoryInteractive
```

3. To import tables from this source, right-click on the data source and choose "Import Tables...", the Import Table Wizard UI should appear showing a list of tables/views from the source. Note, that for Synapse SQL pools, you may have to specify "TOP (without NOLOCK)" as a row clause, in order for the data preview to work.
4. When deploying your model to Analysis Services, you will most likely need to specify other credentials, such as a Service Principal application ID and secret or a SQL account, in order for Analysis Services to authenticate itself against the source when refreshing table data. This can be specified using TMSL or SSMS post-deployment, or you can set this up as [part of your CI/CD deployment pipeline](https://tabulareditor.com/2020/06/20/DevOps5.html#creating-your-first-release-pipeline).

## Manually importing schema/metadata

If you're using a data source not supported by the Import Tables Wizard, you have the option of manually importing metadata. This option provides a UI where you can enter or paste in a table schema on the left hand side, which will be automatically parsed for column name and data type information. Alternatively, you can manually type each column name on the right hand side and choose a data type in the drop down. Either way, this is faster than manually creating a table and adding individual data columns through the main UI. When you're done, hit "Import!", adjust the table name and partition expression.

When parsing the text on the left hand side, Tabular Editor searches for certain keywords, in order to determine how the information is structured. It's pretty liberal in the way it interprets data, so you can, for example, paste in a list of columns from a CREATE TABLE SQL script, or the output of the Power Query `Table.Schema(...)` function as described below. The only requirements is that each line of text represents one column of source data.

![image](https://user-images.githubusercontent.com/8976200/70419758-6f07f400-1a66-11ea-838d-9a587c8021ca.png)

## Power Query data sources

Since there is no officially supported way to execute or validate a Power Query/M expression, Tabular Editor only has limited support for Power Query data sources. As of 2.9.0, you may use the "Manually import metadata from another application"-option of the Import Table Wizard, as described above, to import a schema from a Power Query query in Excel or Power BI Desktop. The workflow is the following:

- First, make sure your model contains a Power Query Data Source. Right-click Data Sources > New Data Source (Power Query). If you're going to load data from a SQL Server, specify "tds" as the protocol and fill out the Database, Server and AuthenticationKind properties.
![image](https://user-images.githubusercontent.com/8976200/70418811-6dd5c780-1a64-11ea-8332-d074c6b2d5c2.png)
- For other types of data sources, it may be easier to create the initial model and first few tables in SSDT, to figure out how the Data Source should be configured, and then use the technique below only when adding additional tables.
- Use Power Query within Excel or Power BI Desktop to connect to your source data and apply any transformations needed.
- Using Power Query's Advanced Editor, add a step that uses the `Table.Schema(...)` [M function](https://docs.microsoft.com/en-us/powerquery-m/table-schema) on the previous output:
![image](https://user-images.githubusercontent.com/8976200/70416018-5562ae80-1a5e-11ea-8962-529304ce83f0.png)
- Select the full output preview, copy it into the clipboard (CTRL+A, CTRL+C) and paste it into the schema/metadata textbox in the Import Tables Wizard:
![image](https://user-images.githubusercontent.com/8976200/70416817-2e0ce100-1a60-11ea-9e2b-430cecf88d0a.png)
- Click "Import!" and provide a proper name for your table.
- Lastly, paste the original M expression you used in Excel/Power BI, from before you modified it with the `Table.Schema(...)` function, into the partition on the newly created table. Modify the M expression to point to the source you specified in the first step:
![image](https://user-images.githubusercontent.com/8976200/70418985-dae95d00-1a64-11ea-8bfb-8dda16c33742.png)