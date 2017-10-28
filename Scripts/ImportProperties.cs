// BEFORE RUNNING THIS SCRIPT, SAVE A COPY OF YOUR MODEL.
//
// This script showcases how we can import a set of properties on model objects, from a TSV file.
// The script assumes that the first row contains headers, and the first column contains the
// object path identifying the object.
//
// Lambda methods defined below will be added as methods in the TOMWrapper namespace in a future
// build of Tabular Editor, so that using this functionality only requires the execution of one
// line of code (see the bottom of this script).

Func<string, TabularObject> ResolveObjectPath =
(path) => {
    var parts = path.Split('.');
    TabularObject obj = Model;
    foreach(var part in parts) {
        if(part == "Model") continue;
        if(obj is Model) {
            obj = Model.Tables[part];
            continue;
        }
        if(obj is Table) {
            obj = (obj as Table).GetChildren().OfType<TabularNamedObject>().FirstOrDefault(c => c.Name == part);
            continue;
        }
        if(obj is Hierarchy) {
            obj = (obj as Hierarchy).Levels.FirstOrDefault(l => l.Name == part);
            continue;
        }
        obj = null;
        break;
    }
    return obj;
};

// public static void AssignTsvToObject(string propertyValues, string properties)
// ------------------------------------------------------------------------------
// Arguments:
//     * propertyValues : A tabulator-separated string of property values to assign to the object
//                        specified as the first entry of the string.
//     * properties     : A comma-separated string of property names to assign the values to.
//                        Read-only properties, or properties not found on the object, are ignored.
//                        Properties referencing other objects are resolved based on the string
//                        value.
Action<string, string> AssignTsvToObject = 
(propertyValues,properties) => { 
    var props = properties.Split(',');
    var values = propertyValues.Split('\t').Select(v => v.Replace("\\n","\n").Replace("\\t","\t")).ToArray();
    var obj = ResolveObjectPath(values[0]);
    if(obj == null) return;
    
    for(int i = 0; i < props.Length; i++) {
        var pInfo = obj.GetType().GetProperty(props[i]);
        
        // Consider only properties that exist, and have a public setter:
        if(pInfo == null || !pInfo.CanWrite || !pInfo.GetSetMethod(true).IsPublic) continue;
        
        var pValue = values[i + 1]; // This is shifted by 1 since the first column is the Object path
        if(typeof(TabularObject).IsAssignableFrom(pInfo.PropertyType))
        {
            // Object references need to be resolved:
            var pValueObj = ResolveObjectPath(pValue);
            pInfo.SetValue(obj, pValueObj);
        }
        else {
            // Value is converted from string to the type of the property:
            pInfo.SetValue(obj, Convert.ChangeType(pValue, pInfo.PropertyType));    
        }
    }
};

// public static void SetPropertyValues(string tsv)
// -------------------------------------------------------------------------------
// Arguments:
//     * tsvData : A string containing a tsv-formatted dataset (with headers) that represents the
//                 values of any number of properties, of any number of objects. The first column
//                 of the dataset must uniquely identify the object, as obtained from the
//                 "GetObjectPath()" method. Subsequent columns hold property values. Header names
//                 for these columns are the corresponding property names.
Action<string> ImportProperties =
(tsvData) => {
    var rows = tsvData.Split('\n');
    var properties = string.Join(",", rows[0].Split('\t').Skip(1).ToArray());
    foreach(var row in rows.Skip(1)) {
        if(!string.IsNullOrWhiteSpace(row))
            AssignTsvToObject(row, properties);
    }
};

// public static string ReadFile(string filePath)
// --------------------------------------------------------------------------------
// Allows reading a file's text content, even if the file is exclusively locked by
// another process, such as Excel. This is an alternative to the standard .NET
// System.IO.File.ReadAllText() method.
Func<string, string> ReadFile =
(filePath) => {
    using (var fileStream = new System.IO.FileStream(filePath, 
        System.IO.FileMode.Open,
        System.IO.FileAccess.Read,
        System.IO.FileShare.ReadWrite))
    using (var textReader = new System.IO.StreamReader(fileStream))
    {
        return textReader.ReadToEnd();
    }    
};

var tsv = ReadFile(@"c:\Temp\AllMeasures.tsv");
ImportProperties(tsv);
