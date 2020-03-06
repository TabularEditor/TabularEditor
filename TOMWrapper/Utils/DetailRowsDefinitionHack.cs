using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.Utils
{
    public static class DetailRowsDefinitionHack
    {
        public static void SetToNull(Table table)
        {
            Type type = typeof(MetadataObject);

            var orgObj = table.DefaultDetailRowsDefinition;

            // Set DetailRowsDefinition.ObjectID = null (Microsoft.AnalysisServices.Tabular.dll~ChildLink`2.cs line 49):
            Type tableType = typeof(Table);
            FieldInfo tableBodyFieldInfo = GetFieldInfo(tableType, "body");
            object tableBody = tableBodyFieldInfo.GetValue(table);
            FieldInfo tableProp = GetFieldInfo(tableBody.GetType(), "DefaultDetailRowsDefinitionID");
            object tablePropValue = tableProp.GetValue(tableBody);
            PropertyInfo tablePropObj = tableProp.FieldType.BaseType.GetProperty("Object");
            try
            {
                tablePropObj.SetMethod.Invoke(tablePropValue, new object[] { null });
            }
            catch
            {
                tablePropObj.SetMethod.Invoke(tablePropValue, new object[] { null });
            }

            // Call UpdateMetadataObjectParent (Microsoft.AnalysisServices.Tabular.dll~DetailRowsDefinition.cs line 82):
            string methodName = "UpdateMetadataObjectParent";
            MethodInfo info = type.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            MethodInfo concrete = info.MakeGenericMethod(typeof(DetailRowsDefinition), typeof(MetadataObject));

            Type objType = typeof(DetailRowsDefinition);
            FieldInfo bodyFieldInfo = GetFieldInfo(objType, "body");
            object body = bodyFieldInfo.GetValue(orgObj);
            Type bodyType = bodyFieldInfo.FieldType;
            FieldInfo objectIdInfo = GetFieldInfo(bodyType, "ObjectID");
            object objectID = objectIdInfo.GetValue(body);

            concrete.Invoke(null, new object[] { objectID, null, null, null });
        }

        public static void SetToNull(Measure measure)
        {
            Type type = typeof(MetadataObject);

            var orgObj = measure.DetailRowsDefinition;

            // Set DetailRowsDefinition.ObjectID = null (Microsoft.AnalysisServices.Tabular.dll~ChildLink`2.cs line 49):
            Type measureType = typeof(Measure);
            FieldInfo measureBodyFieldInfo = GetFieldInfo(measureType, "body");
            object measureBody = measureBodyFieldInfo.GetValue(measure);
            FieldInfo measureProp = GetFieldInfo(measureBody.GetType(), "DetailRowsDefinitionID");
            object measurePropValue = measureProp.GetValue(measureBody);
            PropertyInfo measurePropObj = measureProp.FieldType.BaseType.GetProperty("Object");
            try
            {
                measurePropObj.SetMethod.Invoke(measurePropValue, new object[] { null });
            }
            catch
            {
                measurePropObj.SetMethod.Invoke(measurePropValue, new object[] { null });
            }

            // Call UpdateMetadataObjectParent (Microsoft.AnalysisServices.Tabular.dll~DetailRowsDefinition.cs line 82):
            string methodName = "UpdateMetadataObjectParent";
            MethodInfo info = type.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            MethodInfo concrete = info.MakeGenericMethod(typeof(DetailRowsDefinition), typeof(MetadataObject));

            Type objType = typeof(DetailRowsDefinition);
            FieldInfo bodyFieldInfo = GetFieldInfo(objType, "body");
            object body = bodyFieldInfo.GetValue(orgObj);
            Type bodyType = bodyFieldInfo.FieldType;
            FieldInfo objectIdInfo = GetFieldInfo(bodyType, "ObjectID");
            object objectID = objectIdInfo.GetValue(body);

            concrete.Invoke(null, new object[] { objectID, null, null, null });
        }


        private static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            FieldInfo fieldInfo;
            do
            {
                fieldInfo = type.GetField(fieldName,
                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            }
            while (fieldInfo == null && type != null);
            return fieldInfo;
        }
    }
}
