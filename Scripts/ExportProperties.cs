var outputPath = @"c:\Temp\";
// BEFORE RUNNING THIS SCRIPT, SAVE A COPY OF YOUR MODEL. ALSO, MAKE SURE THE FOLDER ABOVE EXISTS.
//
// This script showcases how we can export a set of properties on model objects, as a TSV file.
// The file can be loaded in Excel for bulk editing of properties, and subsequently loaded using
// the accompanying Import.cs script.
//
// Lambda methods defined below will be added as methods in the TOMWrapper namespace in a future
// build of Tabular Editor, so that using this functionality only requires the execution of one
// line of code (see the bottom of this script).

// public static string GetTsvForObject(this TabularObject obj, string properties)
// -------------------------------------------------------------------------------
// Arguments:
//     * obj        : A TabularObject (measure, column, table) to consider.
//     * properties : A comma-separated string of property names to output. Properties not found
//                    on the object, will be output as blank. Properties referencing other objects
//                    will be output as the full path of that object.
// Output: 
//     A tabulator-separated string of property values for the considered object.
Func<TabularObject, string, string> GetTsvForObject = 
(obj,properties) => { 
    var props = properties.Split(',');
    var sb = new System.Text.StringBuilder();
    sb.Append(obj.GetObjectPath());
    foreach(var prop in props) {
        sb.Append('\t');
        var pInfo = obj.GetType().GetProperty(prop);
        if(pInfo != null) {
            var pValue = pInfo.GetValue(obj);
            if(pValue == null) 
                continue;
            else if(pValue is TabularObject)
                sb.Append((pValue as TabularObject).GetObjectPath());
            else
                sb.Append(pValue.ToString().Replace("\n","\\n").Replace("\t","\\t"));
        }
    }
    return sb.ToString(); 
};

// public static string GetPropertyValues(this IEnumerable<ITabularNamedObject> objects, string properties)
// --------------------------------------------------------------------------------------------------------
// Arguments:
//     * objects    : A collection of ITabularNamedObjects to consider.
//     * properties : A comma-separated string of property names to output. Properties 
//                    not found on any individual object, will be output as blank.
// Output: 
//     A tsv-formatted dataset as a string. The first column references the individual objects.
//     Subsequent columns according to the 'properties' argument. The first row has headers.
Func<System.Collections.Generic.IEnumerable<ITabularNamedObject>, string, string> GetPropertyValues = 
(objects,properties) => {
    var sb = new System.Text.StringBuilder();
    sb.Append("Object\t");
    sb.Append(properties.Replace(",","\t"));
    foreach(var obj in objects.OfType<TabularObject>()) {
        sb.Append("\n");
        sb.Append(GetTsvForObject(obj, properties));
    }
    return sb.ToString();
};

// Sample usage 1: (outputs all selected objects as TSV)
var tsv1 = GetPropertyValues(Selected, "Table,ObjectType,Name,FormatString,DisplayFolder,Description,IsHidden,Expression");
System.IO.File.WriteAllText(outputPath + "SelectedObjects.tsv", tsv1);

// Sample usage 2: (outputs all model measures as TSV)
var tsv2 = GetPropertyValues(Model.AllMeasures, "Name,FormatString,DisplayFolder,Description,IsHidden,Expression,DetailRowsExpression");
System.IO.File.WriteAllText(outputPath + "AllMeasures.tsv", tsv2);
