using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper.PowerBI
{
    /// <summary>
    /// Contains methods that governs the rules when editing Power BI data models (such as
    /// when connected to a Power BI Desktop instance, or when a .pbit file has been loaded).
    /// </summary>
    internal class PowerBIGovernance
    {
        // Defines a list of object types that can be removed/added from/to a Power BI data model:
        private HashSet<Type> Manipulatable = new HashSet<Type>() {
            typeof(Measure),
            typeof(KPI),
            typeof(ModelRole),
            typeof(ModelRoleMember),
            typeof(ExternalModelRoleMember),
            typeof(WindowsModelRoleMember),
            typeof(CalculationGroup),
            typeof(CalculationGroupTable),
            typeof(CalculationItem),
            typeof(Perspective),
            typeof(Culture)
        };

        public void UpdateGovernanceMode(TabularModelHandler handler)
        {
            if (handler.Database == null) return;

            if (handler.Settings.PBIFeaturesOnly && (handler.SourceType == ModelSourceType.Pbit || IsPBIDesktop(handler.Database)))
            {
                if (handler.Database.Model.DefaultPowerBIDataSourceVersion == Microsoft.AnalysisServices.Tabular.PowerBIDataSourceVersion.PowerBI_V3)
                    GovernanceMode = PowerBIGovernanceMode.V3Restricted;
                else
                    GovernanceMode = PowerBIGovernanceMode.ReadOnly;
            }
            else
                GovernanceMode = PowerBIGovernanceMode.Unrestricted;
        }

        internal bool IsPBIDesktop(TOM.Database database)
        {
            var server = database.Server;
            if (server == null) return false;

            if (server.CompatibilityMode == Microsoft.AnalysisServices.CompatibilityMode.PowerBI &&
                server.ServerLocation == Microsoft.AnalysisServices.ServerLocation.OnPremise)
                return true;

            return false; // throw new NotImplementedException();
        }

        public PowerBIGovernanceMode GovernanceMode { get; private set; } = PowerBIGovernanceMode.Unrestricted;

        public bool AllowCreate(Type type)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else return Manipulatable.Contains(type);
        }

        public bool AllowCreate(IEnumerable<ITabularObject> objects)
        {
            return objects.Select(obj => obj.GetType()).Distinct().All(t => AllowCreate(t));
        }

        public bool AllowDelete(Type type)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else return Manipulatable.Contains(type);
        }

        public bool AllowDelete(IEnumerable<ITabularObject> objects)
        {
            return objects.Select(obj => obj.GetType()).Distinct().All(t => AllowDelete(t));
        }

        public bool AllowGroup(string groupName)
        {
            return true; // Show all groups - but some properties will remain read-only
        }

        public bool AllowEditProperty(ObjectType type, string property)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else
            {
                // Some properties can be edited regardless of object type in restricted mode:
                switch (property)
                {
                    case Properties.TRANSLATEDDESCRIPTIONS:
                    case Properties.TRANSLATEDDISPLAYFOLDERS:
                    case Properties.TRANSLATEDNAMES:
                    case Properties.ISHIDDEN:
                    case Properties.DETAILROWSEXPRESSION:
                    case Properties.DEFAULTDETAILROWSEXPRESSION:
                    case Properties.DESCRIPTION:
                    case Properties.FORMATSTRING:
                    case Properties.SUMMARIZEBY:
                    case Properties.DATACATEGORY:
                    case Properties.DISPLAYFOLDER:
                    case Properties.KPI:
                    case Properties.DISCOURAGEIMPLICITMEASURES:
                        return true;
                }

                switch (type)
                {
                    case ObjectType.Measure: return AllowRestrictedEditMeasureProperty(property);
                    case ObjectType.KPI: return AllowRestrictedEditKpiProperty(property);
                    case ObjectType.CalculationGroupTable:
                    case ObjectType.CalculationGroup:
                    case ObjectType.CalculationItem:
                        return AllowRestrictedEditCalculationGroupItemProperty(property);
                    default:
                        return false;
                }
            }
        }
        public bool AllowEditName(TabularNamedObject obj)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else
                switch(obj.ObjectType)
                {
                    // Only allow column renames when parent table is a calc group table in restricted mode:
                    case ObjectType.Column: return (obj as Column).Table is CalculationGroupTable;
                    case ObjectType.CalculationGroupTable:
                    case ObjectType.CalculationItem:
                    case ObjectType.Measure:
                    case ObjectType.Perspective:
                    case ObjectType.Culture:
                        return true;
                    default:
                        return false;
                }
        }

        private bool AllowRestrictedEditMeasureProperty(string property)
        {
            switch(property)
            {
                case Properties.EXPRESSION:
                case Properties.DESCRIPTION:
                case Properties.DATACATEGORY:
                case Properties.FORMATSTRING:
                    return true;
                default:
                    return false;
            }
        }
        private bool AllowRestrictedEditKpiProperty(string property)
        {
            switch(property)
            {
                // Anything but annotations / ext properties:
                case Properties.ANNOTATIONS:
                case Properties.EXTENDEDPROPERTIES:
                    return false;
                default:
                    return true;
            }
        }
        private bool AllowRestrictedEditCalculationGroupItemProperty(string property)
        {
            switch(property)
            {
                case Properties.NAME:
                case Properties.EXPRESSION:
                case Properties.ORDINAL:
                case Properties.FORMATSTRINGEXPRESSION:
                case Properties.ISHIDDEN:
                case Properties.CALCULATIONGROUPPRECEDENCE:
                case Properties.PRECEDENCE:
                    return true;
                default:
                    return false;
            }
        }

        public bool AllowEditProperty(IEnumerable<ObjectType> types, string property)
        {
            return types.All(t => AllowEditProperty(t, property));
        }

        public bool AllowEditProperty(IEnumerable<ITabularObject> objects, string property)
        {
            return AllowEditProperty(objects.Select(obj => obj.ObjectType).Distinct(), property);
        }

        public bool VisibleProperty(ObjectType type, string property)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            
            switch(property)
            {
                case Properties.NAME:
                case Properties.OBJECTTYPENAME:
                case Properties.ISHIDDEN:
                case Properties.DISPLAYFOLDER:
                case Properties.DESCRIPTION:
                case Properties.FORMATSTRING:
                case Properties.EXPRESSION:
                case Properties.FORMATSTRINGEXPRESSION:
                case Properties.DEFAULTDETAILROWSEXPRESSION:
                case Properties.DETAILROWSEXPRESSION:
                case Properties.DATATYPE:
                case Properties.DATACATEGORY:
                case Properties.CALCULATIONGROUPPRECEDENCE:
                case Properties.ORDINAL:
                case Properties.FROMCOLUMN:
                case Properties.TOCOLUMN:
                case Properties.MEMBERS:
                case Properties.FROMCARDINALITY:
                case Properties.TOCARDINALITY:
                case Properties.SECURITYFILTERINGBEHAVIOR:
                case Properties.CROSSFILTERINGBEHAVIOR:
                case Properties.ISACTIVE:
                case Properties.ISKEY:
                case Properties.KPI:
                case Properties.SUMMARIZEBY:
                case Properties.TRANSLATEDDESCRIPTIONS:
                case Properties.TRANSLATEDDISPLAYFOLDERS:
                case Properties.TRANSLATEDNAMES:
                case Properties.SOURCECOLUMN:
                case Properties.STATUSDESCRIPTION:
                case Properties.STATUSEXPRESSION:
                case Properties.STATUSGRAPHIC:
                case Properties.TARGETDESCRIPTION:
                case Properties.TARGETEXPRESSION:
                case Properties.TARGETFORMATSTRING:
                case Properties.TRENDDESCRIPTION:
                case Properties.TRENDEXPRESSION:
                case Properties.TRENDGRAPHIC:
                case Properties.MEMBERID:
                case Properties.MEMBERNAME:
                case Properties.MEMBERTYPE:
                case Properties.IDENTITYPROVIDER:
                case Properties.DISCOURAGEIMPLICITMEASURES:
                case Properties.DATABASE:
                    return true;
            }
            return false;
        }
    }


    public enum PowerBIGovernanceMode
    {
        Unrestricted,
        V3Restricted,
        ReadOnly
    }

    public class PowerBIGovernanceException: Exception
    {
        public PowerBIGovernanceException(string message): base(message)
        {

        }
    }

}
