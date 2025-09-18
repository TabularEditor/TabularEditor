using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly Version Base = new Version(1, 0, 0, 0);
        private static readonly Version OLSSupport = new Version(2, 88, 0, 0);
        private static readonly Version RoleModdingSupport = new Version(2, 90, 0, 0);
        private static readonly Version DevMode = new Version(2, 118, 0, 0);
        private static readonly Version DevModePbip = new Version(9992, 118, 999, 999);

        // Defines a list of object types that can be removed/added from/to a Power BI data model:
        private Dictionary<Type, Version> Manipulatable = new Dictionary<Type,Version>() {
            { typeof(Measure), Base },
            { typeof(KPI), Base },
            { typeof(CalculationGroup), Base },
            { typeof(CalculationGroupTable), Base },
            { typeof(CalculationItem), Base },
            { typeof(Perspective), Base },
            { typeof(Culture), Base },
            { typeof(TablePermission), OLSSupport },
            { typeof(ModelRole), RoleModdingSupport },

            { typeof(CalculatedColumn), DevMode },
            { typeof(CalculatedTableColumn), DevMode },
            { typeof(CalculatedTable), DevMode },
            { typeof(Hierarchy), DevMode },
            { typeof(Relationship), DevMode },
            { typeof(SingleColumnRelationship), DevMode },
            { typeof(Function), DevMode },

            { typeof(Column), DevModePbip },
            { typeof(DataColumn), DevModePbip },
            { typeof(Table), DevModePbip },
            { typeof(NamedExpression), DevModePbip }
        };

        private readonly TabularModelHandler handler;

        public PowerBIGovernance(TabularModelHandler handler)
        {
            this.handler = handler;
        }

        public void UpdateGovernanceMode(string fileSourcePath = null)
        {
            string pbipFileName = null;
            if (handler.SourceType != ModelSourceType.Database && fileSourcePath != null)
            {
                var fi = new FileInfo(fileSourcePath);
                if(fi.Exists && fi.Directory.Name.EndsWith("Dataset", StringComparison.OrdinalIgnoreCase))
                {
                    pbipFileName = Path.Combine(fi.Directory.Parent.FullName, fi.Directory.Name.Replace(".Dataset", ".pbip", StringComparison.OrdinalIgnoreCase));
                }
            }
            PBIP = pbipFileName != null && File.Exists(pbipFileName);

            PBIDesktopVersion = new Version();
            PBIDesktopVersionSimple = 0.0M;
            if (handler.Database == null) return;

            if (handler.Settings.PBIFeaturesOnly && (PBIP || handler.SourceType == ModelSourceType.Pbit || IsPBIDesktop(handler.Database)))
            {
                if (handler.Database.Model.DefaultPowerBIDataSourceVersion == Microsoft.AnalysisServices.Tabular.PowerBIDataSourceVersion.PowerBI_V3)
                    GovernanceMode = PowerBIGovernanceMode.V3Restricted;
                else
                    GovernanceMode = PowerBIGovernanceMode.ReadOnly;
            }
            else
                GovernanceMode = PowerBIGovernanceMode.Unrestricted;

            if (handler.Model.HasAnnotation("PBIDesktopVersion"))
            {
                var versionStringSplit = handler.Model.GetAnnotation("PBIDesktopVersion").Split(' ');
                if (versionStringSplit.Length > 0 && Version.TryParse(versionStringSplit[0], out Version version))
                    PBIDesktopVersion = version;
                if (versionStringSplit.Length > 1 && System.Text.RegularExpressions.Regex.IsMatch(versionStringSplit[1], "^\\(\\d\\d\\.\\d\\d\\)$"))
                {
                    var _versions = versionStringSplit[1].Replace("(", "").Replace(")", "").Split('.');
                    PBIDesktopVersionSimple = int.Parse(_versions[0]) + (int.Parse(_versions[1]) / 100.0M);
                }
            }
            else
            {
                // If the annotation is not present (seems not to be the case since September 2025 version of PBI Desktop), assume everything is possible:
                GovernanceMode = PowerBIGovernanceMode.Unrestricted;
            }
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

        private PowerBIGovernanceMode internalGovernanceMode = PowerBIGovernanceMode.Unrestricted;
        public PowerBIGovernanceMode GovernanceMode
        {
            get => GovernanceEffective ? internalGovernanceMode : PowerBIGovernanceMode.Unrestricted;
            set => internalGovernanceMode = value;
        }
        public bool PBIP { get; private set; }
        public Version PBIDesktopVersion { get; private set; }
        public decimal PBIDesktopVersionSimple { get; private set; }

        private int governanceSuspension = 0;
        private bool GovernanceEffective => governanceSuspension == 0;
        public void SuspendGovernance()
        {
            governanceSuspension++;
        }
        public void ResumeGovernance()
        {
            governanceSuspension--;
        }

        public bool AllowCreate(Type type)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else return IsManipulatable(type);
        }

        private bool IsManipulatable(Type type)
        {
            if(Manipulatable.TryGetValue(type, out Version minVersion))
            {
                if (minVersion <= PBIDesktopVersion) return true;
                if (minVersion == DevModePbip && PBIP) return true;
            }
            return false;
        }

        public bool AllowCreate(IEnumerable<ITabularObject> objects)
        {
            return objects.Select(obj => obj.GetType()).Distinct().All(t => AllowCreate(t));
        }

        public bool AllowDelete(Type type)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else return IsManipulatable(type);
        }

        public bool AllowDelete(IEnumerable<ITabularObject> objects)
        {
            return objects.Select(obj => obj.GetType()).Distinct().All(t => AllowDelete(t));
        }

        public bool AllowGroup(string groupName)
        {
            return true; // Show all groups - but some properties will remain read-only
        }

        public bool AllowEditProperty(ObjectType type, string property, TabularObject objInstance = null)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else
            {
                if (type == ObjectType.Database) return false;
                if (type == ObjectType.Folder) return true;

                // Some properties can be edited regardless of object type in restricted mode:
                switch (property)
                {
                    case Properties.TRANSLATEDDESCRIPTIONS:
                    case Properties.TRANSLATEDDISPLAYFOLDERS:
                    case Properties.TRANSLATEDNAMES:
                    case nameof(ISynonymObject.Synonyms):
                    case Properties.ISHIDDEN:
                    case Properties.DETAILROWSEXPRESSION:
                    case Properties.DEFAULTDETAILROWSEXPRESSION:
                    case Properties.DESCRIPTION:
                    case Properties.FORMATSTRING:
                    case Properties.FORMATSTRINGEXPRESSION:
                    case Properties.SUMMARIZEBY:
                    case Properties.DATACATEGORY:
                    case Properties.DISPLAYFOLDER:
                    case Properties.KPI:
                    case Properties.DISCOURAGEIMPLICITMEASURES:
                    case Properties.INPERSPECTIVE:
                    case nameof(Culture.Content):
                    case nameof(Culture.ContentType):
                        return true;

                    case Properties.OBJECTLEVELSECURITY:
                    case Properties.METADATAPERMISSION:
                    case Properties.FILTEREXPRESSION:
                    case Properties.MODELPERMISSION:
                    case Properties.COLUMNPERMISSIONS:
                    case Properties.TABLEPERMISSIONS:
                    case Properties.ROWLEVELSECURITY:
                        return PBIDesktopVersion >= OLSSupport ; // Conditional on Power BI version (must be December 2020 release or newer)
                }

                switch (type)
                {
                    case ObjectType.Measure: 
                        return PBIDesktopVersion >= DevMode ? true : AllowRestrictedEditMeasureProperty(property);
                    case ObjectType.KPI: 
                        return PBIDesktopVersion >= DevMode ? true : AllowRestrictedEditKpiProperty(property);
                    case ObjectType.CalculationGroupTable:
                    case ObjectType.CalculationGroup:
                    case ObjectType.CalculationItem:
                        return PBIDesktopVersion >= DevMode ? true : AllowRestrictedEditCalculationGroupItemProperty(property);
                    case ObjectType.Function:
                        return PBIDesktopVersion >= DevMode;
                    default:
                        switch (type)
                        {
                            case ObjectType.Partition:
                                if (property == Properties.EXPRESSION && objInstance is Partition p && p.SourceType == PartitionSourceType.Calculated) return PBIDesktopVersion >= DevMode;
                                return false;
                            case ObjectType.Expression:
                                return PBIP;
                            case ObjectType.Column:
                                if (property == Properties.SOURCECOLUMN) return PBIP;
                                return PBIDesktopVersion >= DevMode;
                            case ObjectType.Table:
                                if (property == Properties.EXPRESSION) return PBIDesktopVersion >= DevMode;
                                return PBIP;
                        }

                        return false;
                }
            }
        }
        public bool AllowEditName(TabularNamedObject obj)
        {
            if (GovernanceMode == PowerBIGovernanceMode.Unrestricted) return true;
            else if (GovernanceMode == PowerBIGovernanceMode.ReadOnly) return false;
            else
                if (obj is ITranslatableObject && this.handler.Tree.Culture != null) return true;
                switch(obj.ObjectType)
                {
                    // Only allow column renames when parent table is a calc group table in restricted mode:
                    case ObjectType.CalculationGroupTable:
                    case ObjectType.CalculationItem:
                    case ObjectType.Hierarchy:    
                    case ObjectType.Measure:
                    case ObjectType.Perspective:
                    case ObjectType.Culture:
                    case ObjectType.Role:
                    case ObjectType.Function:
                    return true;
                    default:
                        if (obj is CalculatedTable) return true;
                        if (obj is Table) return PBIP;
                        if (obj is NamedExpression) return PBIP;
                        if (obj is DataColumn) return PBIP || (obj as Column).Table is CalculationGroupTable;
                        if (obj is CalculatedColumn) return true;
                        if (obj is CalculatedTableColumn) return true;
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
                case Properties.FORMATSTRINGEXPRESSION:
                case Properties.LINEAGETAG:
                case Properties.NAME:
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

            if (PBIP)
            {
                if (property == nameof(Table.Partitions)) return false;
                return true;
            }
            if (PBIDesktopVersion >= DevMode)
            {
                if (type != ObjectType.Table) return true;
            }

            switch(property)
            {
                case Properties.ANNOTATIONS:
                case Properties.EXTENDEDPROPERTIES:
                    return PBIDesktopVersion >= DevMode;
                case Properties.STATE:
                case nameof(EntityPartition.EntityName):
                case nameof(EntityPartition.ExpressionSource):
                case nameof(MPartition.MExpression):
                case nameof(Culture.Content):
                case nameof(Culture.ContentType):
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
                case Properties.DEFAULTDETAILROWSDEFINITION:
                case Properties.DETAILROWSDEFINITION:
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
                case Properties.SOURCELINEAGETAG:
                case Properties.LINEAGETAG:
                case Properties.SUMMARIZEBY:
                case Properties.TRANSLATEDDESCRIPTIONS:
                case Properties.TRANSLATEDDISPLAYFOLDERS:
                case Properties.TRANSLATEDNAMES:
                case nameof(ISynonymObject.Synonyms):
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
                case Properties.SERVER:
                case Properties.COMPATIBILITYLEVEL:
                case Properties.COMPATIBILITYMODE:
                case Properties.INPERSPECTIVE:
                case Properties.CULTURE:
                case Properties.OBJECTLEVELSECURITY:
                case Properties.ROWLEVELSECURITY:
                case Properties.METADATAPERMISSION:
                case Properties.COLUMNPERMISSIONS:
                case Properties.MODELPERMISSION:
                case Properties.TABLEPERMISSIONS:
                case Properties.ROLE:
                case Properties.TABLE:
                case Properties.ERRORMESSAGE:
                case Properties.FILTEREXPRESSION:
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
