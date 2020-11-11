# Custom Actions
```eval_rst
.. note::
    Please note that this functionality is unrelated to the Custom Actions feature available for Multidimensional models.
```

Say you have created a useful script using the `Selected` object, and you want to be able to execute the script several times on different objects in the explorer tree. Instead of hitting the "Play" button whenever you want to execute the script, Tabular Editor lets you save it as a Custom Action:

![image](https://user-images.githubusercontent.com/8976200/33581673-0db35ed0-d952-11e7-90cd-e3164e198865.png)

After saving the custom action, you will see that it is now available directly from the right-click context menu of the explorer tree, making it very easy to invoke the script on any objects selected in the tree. You can create as many custom actions as you want. Use backslashes (\\) in the names to create a submenu structure within the context menu.

![Custom Actions show up directly in the context menu](https://raw.githubusercontent.com/otykier/TabularEditor/master/Documentation/InvokeCustomAction.png)

Custom Actions are stored in the CustomActions.json file within %AppData%\Local\TabularEditor. In the above example, the contents of this file will look like this:

```json
{
  "Actions": [
    {
      "Name": "Custom Formatting\\Number with 1 decimal",
      "Enabled": "true",
      "Execute": "Selected.Measures.ForEach(m => m.FormatString = \"0.0\";",
      "Tooltip": "Sets the FormatString property to \"0.0\"",
      "ValidContexts": "Measure, Column"
    }
  ]
}
```

As you can see, `Name` and `Tooltip` gets their values from whatever was specified when the action was saved. `Execute` is the actual script to be executed when the action is invoked. Note that any syntax errors in the CustomActions.json file will cause Tabular Editor to skip loading all Custom Actions entirely, so make sure you can successfully execute a script inside the Advanced Scripting editor, before saving it as a Custom Action.

The `ValidContexts` property holds a list of object types for which the Action will be available. When selecting objects in the tree, a selection containing any objects different from the types listed in the `ValidContexts` property will hide the action from the context menu.

## Controlling Action Availability
If you need even more control on when an action can be invoked from the context menu, you can set the `Enabled` property to a custom expression that must return a boolean value, indicating whether the action will be available for the given selection. By default, the `Enabled` property has the value "true", which means that the action will always be enabled within the valid context. Keep this in mind, when using the singular object references on the `Selected` object, such as `Selected.Measure` or `Selected.Table`, as these will throw an error if the current selection does not contain exactly one of that type of object. In such a case, it is recommended to use the `Enabled` property to check that one and only one object of the required type, has been selected:

```json
{
  "Actions": [
    {
      "Name": "Reset measure name",
      "Enabled": "Selected.Measures.Count == 1",
      "Execute": "Selected.Measure.Name == \"New Measure\"",
      "ValidContexts": "Measure"
    }
  ]
}
```

This will disable the context menu item, unless exactly one measure has been selected in the tree.

## Reusing custom actions
Release 2.7 introduces a new script method `CustomAction(...)`, which may be called to invoke previously saved Custom Actions. You can use this method as a stand-alone method (similar to `Output(...)`), or you can use it as an extension method on any set of objects:

```csharp
// Executes "My custom action" against the current selection:
CustomAction("My custom action");                

// Executes "My custom action" against all tables in the model:
CustomAction(Model.Tables, "My custom action");

// Executes "My custom action" against every measure in the current selection whose name starts with "Sum":
Selected.Measures.Where(m => m.Name.StartsWith("Sum")).CustomAction("My custom action");
```

Note that you must specify the full name of the Custom Action, including any context menu folder names.

If no action with the given name is found, an error is raised when the script is executed.