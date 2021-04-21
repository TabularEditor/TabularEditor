# Command Line

Tabular Editor can be executed from the command-line to perform various tasks, which may be useful in Automated Build and Deployment scenarios, etc.

**Note:** Since TabularEditor.exe is a WinForms application, executing it directly from a windows command-prompt will cause the thread to return immediately to the prompt. This may cause issues in command scripts, etc. To wait for TabularEditor.exe to complete its command-line tasks, always execute it using: `start /wait TabularEditor ...`

To view the command-line options available in Tabular Editor, run the following command:

**Windows Command line:**
```shell
start /wait TabularEditor.exe /?
```

**PowerShell:**
```powershell
$p = Start-Process -filePath TabularEditor.exe -Wait -NoNewWindow -PassThru -ArgumentList "/?"
```

Output:
```
Tabular Editor 2.16.0 (build 2.16.7781.40242)
--------------------------------
Usage:

TABULAREDITOR ( file | server database ) [-S script1 [script2] [...]]
    [-SC] [-A [rules] | -AX rules] [(-B | -F) output [id]] [-V] [-T resultsfile]
    [-D [server database [-L user pass] [-O [-C [plch1 value1 [plch2 value2 [...]]]]
        [-P [-Y]] [-R [-M]]]
        [-X xmla_script]] [-W] [-E]]

file                Full path of the Model.bim file or database.json model folder to load.
server              Server\instance name or connection string from which to load the model
database            Database ID of the model to load
-S / -SCRIPT        Execute the specified script on the model after loading.
  scriptN             Full path of one or more files containing a C# script to execute or an inline
                      script.
-SC / -SCHEMACHECK  Attempts to connect to all Provider Data Sources in order to detect table schema
                    changes. Outputs...
                      ...warnings for mismatched data types and unmapped source columns
                      ...errors for unmapped model columns.
-A / -ANALYZE       Runs Best Practice Analyzer and outputs the result to the console.
  rules               Optional path of file or URL of additional BPA rules to be analyzed. If
                      specified, model is not analyzed against local user/local machine rules,
                      but rules defined within the model are still applied.
-AX / -ANALYZEX     Same as -A / -ANALYZE but excludes rules specified in the model annotations.
-B / -BIM / -BUILD  Saves the model (after optional script execution) as a Model.bim file.
  output              Full path of the Model.bim file to save to.
  id                  Optional id/name to assign to the Database object when saving.
-F / -FOLDER        Saves the model (after optional script execution) as a Folder structure.
  output              Full path of the folder to save to. Folder is created if it does not exist.
  id                  Optional id/name to assign to the Database object when saving.
-V / -VSTS          Output Visual Studio Team Services logging commands.
-T / -TRX         Produces a VSTEST (trx) file with details on the execution.
  resultsfile       File name of the VSTEST XML file.
-D / -DEPLOY        Command-line deployment
                      If no additional parameters are specified, this switch will save model metadata
                      back to the source (file or database).
  server              Name of server to deploy to or connection string to Analysis Services.
  database            ID of the database to deploy (create/overwrite).
  -L / -LOGIN         Disables integrated security when connecting to the server. Specify:
    user                Username (must be a user with admin rights on the server)
    pass                Password
  -O / -OVERWRITE     Allow deploy (overwrite) of an existing database.
    -C / -CONNECTIONS   Deploy (overwrite) existing data sources in the model. After the -C switch, you
                        can (optionally) specify any number of placeholder-value pairs. Doing so, will
                        replace any occurrence of the specified placeholders (plch1, plch2, ...) in the
                        connection strings of every data source in the model, with the specified values
                        (value1, value2, ...).
    -P / -PARTITIONS    Deploy (overwrite) existing table partitions in the model.
      -Y / -SKIPPOLICY    Do not overwrite partitions that have Incremental Refresh Policies defined.
    -R / -ROLES         Deploy roles.
      -M / -MEMBERS       Deploy role members.
  -X / -XMLA        No deployment. Generate XMLA/TMSL script for later deployment instead.
    xmla_script       File name of the new XMLA/TMSL script output.
  -W / -WARN        Outputs information about unprocessed objects as warnings.
  -E / -ERR         Returns a non-zero exit code if Analysis Services returns any error messages after
                      the metadata was deployed / updated.
```

## Connecting to Azure Analysis Services
You can use any valid SSAS connection string in place of a server name in the command. The following command loads a model from Azure Analysis Services and saves it locally as a Model.bim file:

**Windows Command Line:**
```shell
start /wait TabularEditor.exe "Provider=MSOLAP;Data Source=asazure://northeurope.asazure.windows.net/MyAASServer;User ID=xxxx;Password=xxxx;Persist Security Info=True;Impersonation Level=Impersonate" MyModelDB -B "C:\Projects\FromAzure\Model.bim"
```

**PowerShell:**
```powershell
$p = Start-Process -filePath TabularEditor.exe -Wait -NoNewWindow -PassThru `
       -ArgumentList "`"Provider=MSOLAP;Data Source=asazure://northeurope.asazure.windows.net/MyAASServer;User ID=xxxx;Password=xxxx;Persist Security Info=True;Impersonation Level=Impersonate`" MyModelDB -B C:\Projects\FromAzure\Model.bim"
```

If you prefer to connect using a Service Principal (Application ID and Key) instead of Azure Active Directory authentication, you can use the following connection string:

```
Provider=MSOLAP;Data Source=asazure://northeurope.asazure.windows.net/MyAASServer;User ID=app:<APPLICATION ID>@<TENANT ID>;Password=<APPLICATION KEY>;Persist Security Info=True;Impersonation Level=Impersonate
```

## Automating script changes
If you have created a script inside Tabular Editor, and you want to apply this script to a Model.bim file prior to deployment, you can use the command-line option "-S" (Script):

**Windows Command Line:**
```shell
start /wait TabularEditor.exe "C:\Projects\MyModel\Model.bim" -S "C:\Projects\MyModel\MyScript.cs" -D localhost\tabular MyModel
```

**PowerShell:**
```powershell
$p = Start-Process -filePath TabularEditor.exe -Wait -NoNewWindow -PassThru `
       -ArgumentList "`"C:\Projects\MyModel\Model.bim`" -S `"C:\Projects\MyModel\MyScript.cs`" -D `"localhost\tabular`" `"MyModel`""
```

This command will load the Model.bim file in Tabular Editor, apply the specified script and deploy the modified model to the "localhost\tabular" server as a new database "MyModel". Use the "-O" (Overwrite) switch if you want to overwrite an existing database on the server with the same name.

You can use the "-B" (Build) switch instead of the "-D" (Deploy) switch, to output the modified model as a new Model.bim file, instead of deploying it directly to a server. This is useful if you want to deploy the model using another deployment tool, or if you want to inspect the model in Visual Studio or Tabular Editor prior to deployment. It could also be useful for automated build scenarios, where you want to store the modified model as an artifact of the release, before deploying.

## Modifying connection strings during deployment

Let's assume you have a model containing a Data Source with the following connection string:

```
Provider=SQLOLEDB.1;Data Source=sqldwdev;Persist Security Info=False;Integrated Security=SSPI;Initial Catalog=DW
```

During deployment, you want to modify the string, to point to a UAT or production database. The best way to do this, is to first use a script, that changes the entire connection string into a placeholder value, and then use the -C switch to swap the placeholder with the actual connection string.

Put the following script into a file called "ClearConnectionStrings.cs" or similar:

```csharp
// This will replace the connection string of all Provider (legacy) data sources in the model
// with a placeholder based on the name of the data source. E.g., if your data source is called
// "SQLDW", the connection string after running this script would be "SQLDW":

foreach(var ds in Model.DataSources.OfType<ProviderDataSource>())
    ds.ConnectionString = ds.Name;
```

We can instruct Tabular Editor to execute the script, and then perform placeholder swapping using the following command:

**Windows Command Line:**
```shell
start /wait TabularEditor.exe "Model.bim" -S "ClearConnectionStrings.cs" -D localhost\tabular MyModel -C "SQLDW" "Provider=SQLOLEDB.1;Data Source=sqldwprod;Persist Security Info=False;Integrated Security=SSPI;Initial Catalog=DW"
```

**PowerShell:**
```powershell
$p = Start-Process -filePath TabularEditor.exe -Wait -NoNewWindow -PassThru `
       -ArgumentList "Model.bim -S ClearConnectionStrings.cs -D localhost\tabular MyModel -C SQLDW `"Provider=SQLOLEDB.1;Data Source=sqldwprod;Persist Security Info=False;Integrated Security=SSPI;Initial Catalog=DW`""
```

The command above, will deploy the Model.bim file as a new SSAS database "MyModel" on the "localhost\tabular" SSAS instance. Before deployment, the script is used to replace all connection strings on provider (legacy) data sources, with the name of the data source, to be used as a placeholder. Assuming we only have a single data source called "SQLDW", the -C switch will then update the connection string, replacing "SQLDW" with the entire string specified.

This technique is useful for scenarios, where you want to deploy the same model to multiple environments that should process data from different (identical) sources - for example, a production, pre-prod or UAT database. If using Azure DevOps (see below), consider using a variable to store the actual connection string to be used, instead of hardcoding it in the command.

## Integration with Azure DevOps
If you want to use the Tabular Editor CLI inside an Azure DevOps pipeline, you should use the "-V" switch on any TabularEditor.exe command executed by your script. This switch will cause Tabular Editor to output logging commands in a [format readable by Azure DevOps](https://github.com/Microsoft/vsts-tasks/blob/master/docs/authoring/commands.md). These allow Azure DevOps to react properly to errors, etc.

When performing deployment through the command-line, information about unprocessed objects will be outputted to the prompt. In automated deployment scenarios, you may want your build agent to react to situations where objects become unprocessed, for example when adding new columns, changing the DAX expression of a calculated table, etc. In this case, you can use the "-W" switch in addition to the "-V" switch mentioned above, to output this information as warnings. Doing so, will cause the deployment to return the "SucceededWithIssues" status to Azure DevOps, after deployment is completed. You may also use the "-E" switch if you want the deployment to return status "Failed" in case the server reports any DAX errors back after successful deployment.

`start /wait` is not necessary when executing TabularEditor.exe within a Command Line Task in an Azure DevOps pipeline. This is because the Command Line Task will not complete, until all threads spawned by the task have terminated. In other words, you need only use `start /wait` if you have additional commands following the call to TabularEditor.exe, and in this case, make sure to use `start /B /wait`. The `/B` switch is required in order for the output from TabularEditor.exe to be correctly piped back to the pipeline log.

```shell
TabularEditor.exe "C:\Projects\My Model\Model.bim" -D ssasserver databasename -O -C -P -V -E -W
```

Or with multiple commands:

```shell
start /B /wait TabularEditor.exe "C:\Projects\Finance\Model.bim" -D ssasserver Finance -O -C -P -V -E -W
start /B /wait TabularEditor.exe "C:\Projects\Sales\Model.bim" -D ssasserver Sales -O -C -P -V -E -W
```

The figure below shows what such a build looks like in Azure DevOps:

![image](https://user-images.githubusercontent.com/8976200/27128146-bc044356-50fd-11e7-9a67-b893fc48ea50.png)

If the deployment fails for any reason, Tabular Editor returns the "Failed" status to Azure DevOps, regardless of whether or not you are using the "-W" switch.

For more information on Azure DevOps and Tabular Editor, [take a look at this blog series](https://tabulareditor.github.io/2019/02/20/DevOps1.html) (especially [chapter 3](https://tabulareditor.github.io/2019/10/08/DevOps3.html) and onward).

### Azure DevOps PowerShell Task
If you prefer to use a PowerShell task instead of a command line task, you must execute TabularEditor.exe using the `Start-Process` cmdlet, as demonstrated above. In addition, make sure to pass the process exit code as the exit parameter in your PowerShell script, so that errors occurring in Tabular Editor will cause the PowerShell task to fail:

```powershell
$p = Start-Process -filePath TabularEditor.exe -Wait -NoNewWindow -PassThru `
       -argumentList "`"C:\Projects\My Model\Model.bim`" -D ssasserver databasename -O -C -P -V -E -W"
exit $p.ExitCode
```

## Running the Best Practice Analyzer

You can use the "-A" switch to have Tabular Editor scan your model for all objects that are in violation of any Best Practice Rules defined on the local machine (in the %AppData%\..\Local\TabularEditor\BPARules.json file), or as annotations within the model itself. Alternatively, you can specify a path of a .json file containing Best Practice Rules after the "-A" switch, to scan the model using the rules defined in the file. Objects that are in violation will be outputted to the console.

If you're also using the "-V" switch, the severity level of each rule will determine how the rule violation is reported to the build pipeline:

* Severity = 1 will be informational only
* Severity = 2 will cause a WARNING
* Severity >= 3 will cause an ERROR

## Performing a data source schema check

As of [version 2.8](https://github.com/otykier/TabularEditor/releases/tag/2.8), you can use the -SC (-SCHEMACHECK) switch to validate table source queries. This is equivalent to invoking the [Refresh Table Metadata UI](/Importing-Tables#refreshing-table-metadata) except that no changes will be made to the model, but schema differences will be reported to the console. Changed Data Types and columns that were added to the source will be reported as warnings. Missing source columns will be reported as errors. If both the -SC (-SCHEMACHECK) and -S (-SCRIPT) switch are specified, the schema check will run AFTER the script has successfully executed, allowing you to modify Data Source properties before the schema check is performed, for example in order to specify a credential password.

You can also annotate tables and columns if you want the schema check to treat them in a specific way. [More information here](/Importing-Tables#ignoring-objects).

## Command Line output and Exit Codes
The command line provides various details, depending on the switches used and any events encountered during execution. Exit Codes were introduced in [version 2.7.4](https://github.com/otykier/TabularEditor/releases/tag/2.7.4).

|Level|Command|Message|Clarification|
|---|---|---|---|
|Error|(Any)|Invalid argument syntax|Invalid arguments were provided to the Tabular Editor CLI|
|Error|(Any)|File not found: ...||
|Error|(Any)|Error loading file: ...|The file is corrupt or does not contain valid TOM metadata in a JSON format|
|Error|(Any)|Error loading model: ...|Not able to connect to the provided Analysis Services instance, database not found, database metadata corrupt or database not of a supported compatibility level|
|Error|-SCRIPT|Specified script file not found||
|Error|-SCRIPT|Script compilation errors:|Script contained invalid C# syntax. Details will be outputted on the following lines.
|Error|-SCRIPT|Script execution error: ...|Unhandled exception when executing the script.|
|Information|-SCRIPT|Script line #: ...|Use of the `Info(string)` or `Output(string)` methods within the script.|
|Warning|-SCRIPT|Script warning: ...|Use of the `Warning(string)` method within the script.|
|Error|-SCRIPT|Script error: ...|Use of the `Error(string)` method within the script.|
|Error|-FOLDER, -BIM|-FOLDER and -BIM arguments are mutually exclusive.|Tabular Editor can not save the currently loaded model to a folder structure and a .bim file in a single execution.|
|Error|-ANALYZE|Rulefile not found: ...||
|Error|-ANALYZE|Invalid rulefile: ...|The specified BPA rulefile is corrupt or does not contain valid JSON.|
|Information|-ANALYZE|... violates rule ...|Best Practice Analyzer results for rules of severity level 1 or lower.|
|Warning|-ANALYZE|... violates rule ...|Best Practice Analyzer results for rules of severity level 2.|
|Error|-ANALYZE|... violates rule ...|Best Practice Analyzer results for rules of severity level 3 or higher.|
|Error|-DEPLOY|Deployment failed! ...|Failure reason returned directly from Analysis Service instance (for example: Database not found, Database override not allowed, etc.)|
|Information|-DEPLOY|Unprocessed object: ...|Objects that are in state "NoData" or "CalculationNeeded" after succesful deployment. Use the -W switch to treat these as Level=Warning.|
|Warning|-DEPLOY|Object not in "Ready" state: ...|Objects that are in state "DependencyError", "EvaluationError" or "SemanticError" after succesful deployment. If using the -W switch, also includes objects in state "NoData" or "CalculationNeeded".|
|Warning|-DEPLOY|Error on X:...|Objects containing invalid DAX after succesful deployment (measures, calculated columns, calculated tables, roles). Use the -E switch to treat these as Level=Error.|

If any of the "Error" level outputs are encountered, Tabular Editor will return Exit Code = 1. Otherwise 0.
