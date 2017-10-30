using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper.PowerBI
{
    /// <summary>
    /// Contains methods that governs the rules when editing Power BI data models (such as
    /// when connected to a Power BI Desktop instance, or when a .pbit file has been loaded).
    /// </summary>
    internal static class PowerBIGovernance
    {
        // Defines a list of object types that can be deleted from a Power BI data model:
        private static HashSet<Type> Manipulatable = new HashSet<Type>() {
            typeof(CalculatedColumn),
            typeof(Measure),
            typeof(CalculatedTable),
            typeof(Hierarchy),
            typeof(Variation),
            typeof(Relationship),
            typeof(SingleColumnRelationship),
            typeof(KPI),
            typeof(ModelRole),
            typeof(ModelRoleMember),
            typeof(ExternalModelRoleMember),
            typeof(WindowsModelRoleMember)
        };

        public static bool AllowCreate(Type type)
        {
            return Manipulatable.Contains(type);
        }

        public static bool AllowDelete(Type type)
        {
            return Manipulatable.Contains(type);
        }

        public static bool AllowGroup(string groupName)
        {
            switch(groupName)
            {
                case LogicalGroups.RELATIONSHIPS:
                case LogicalGroups.ROLES:
                case LogicalGroups.TABLES:
                case LogicalGroups.EXPRESSIONS:
                    return true;

                //case LogicalGroup.DATASOURCES:
                //case LogicalGroup.PERSPECTIVES:
                //case LogicalGroup.TABLEPARTITIONS:
                //case LogicalGroup.TRANSLATIONS:
                default:
                    return false;
            }
        }

        public static bool AllowProperty(ObjectType type, string property)
        {
            // Non-type specific unsupported properties:
            switch(property)
            {
                case Properties.DISPLAYFOLDER:
                case Properties.PARTITIONS:
                case Properties.PERSPECTIVES:
                case Properties.DATATYPE:
                case Properties.SOURCECOLUMN:
                case Properties.SOURCE:
                case Properties.SOURCETYPE:
                case Properties.SOURCEPROVIDERTYPE:
                case Properties.CULTURES:
                    return false;
                case Properties.EXPRESSION:
                    // Only allow editing Expressions for Measures, Calc Columns and Calc Tables:
                    return type == ObjectType.Measure || type == ObjectType.Table || type == ObjectType.Column;
            }

            return true;
        }
    }
}
