using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.TOMWrapper.PowerBI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    partial class Model: ITabularObjectContainer
    {
        #region Convenient methods
        [IntelliSense("Adds a new perspective to the model."), Tests.GenerateTest()]
        public Perspective AddPerspective(string name = null)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(Perspective)))
                throw new PowerBIGovernanceException("Adding perspectives to this Power BI model is not supported.");

            Handler.BeginUpdate("add perspective");
            var perspective = Perspective.CreateNew(this, name);
            Handler.EndUpdate();
            return perspective;
        }

        [IntelliSense("Adds a new Shared Expression to the model."), Tests.GenerateTest(), Tests.CompatibilityLevel(1400)]
        public NamedExpression AddExpression(string name = null, string expression = null)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(NamedExpression)))
                throw new PowerBIGovernanceException("Adding Shared Expressions to this Power BI model is not supported.");

            Handler.BeginUpdate("add shared expression");
            var expr = NamedExpression.CreateNew(this, name);
            Handler.EndUpdate();
            return expr;
        }

        [IntelliSense("Adds a new calculated table to the model."), Tests.GenerateTest()]
        public CalculatedTable AddCalculatedTable(string name = null, string expression = null)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(CalculatedTable)))
                throw new PowerBIGovernanceException("Adding Calculated Tables to this Power BI model is not supported.");

            Handler.BeginUpdate("add calculated table");
            var t = CalculatedTable.CreateNew(this, name, expression);
            Handler.EndUpdate();
            return t;
        }

        [IntelliSense("Adds a new calculation group to the model."), Tests.GenerateTest(), Tests.CompatibilityLevel(1500)]
        public CalculationGroupTable AddCalculationGroup(string name = null)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(CalculationGroupTable)))
                throw new PowerBIGovernanceException("Adding Calculation Groups to this Power BI model is not supported.");

            Handler.BeginUpdate("add calculation group");
            var maxPrecedence = Model.CalculationGroups.Select(c => c.CalculationGroupPrecedence).DefaultIfEmpty(-1).Max();
            var t = CalculationGroupTable.CreateNew(this, name);
            t.CalculationGroupPrecedence = maxPrecedence + 1;
            Handler.Tree.RebuildFolderCacheForTable(t);
            Handler.EndUpdate();
            return t;
        }

        internal static Model CreateFromMetadata(TOM.Model metadataObject)
        {
            var obj = new Model(metadataObject);
            obj.Init();
            return obj;
        }

        [IntelliSense("Adds a new table to the model."), Tests.GenerateTest()]
        public Table AddTable(string name = null)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(Table)))
                throw new PowerBIGovernanceException("Adding tables to this Power BI model is not supported.");

            Handler.BeginUpdate("add table");
            var t = Table.CreateNew(this, name);
            
            Handler.EndUpdate();
            return t;
        }

        [IntelliSense("Adds a new relationship table to the model."), Tests.GenerateTest()]
        public SingleColumnRelationship AddRelationship()
        {
            Handler.BeginUpdate("add relationship");
            var rel = SingleColumnRelationship.CreateNew(this);
            Handler.EndUpdate();
            return rel;
        }

        [IntelliSense("Adds a new translation to the model."), Tests.GenerateTest("da-DK")]
        public Culture AddTranslation(string cultureId)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(Culture)))
                throw new PowerBIGovernanceException("Adding metadata translations to this Power BI Model is not supported.");

            Handler.BeginUpdate("add translation");
            var culture = TOMWrapper.Culture.CreateNew(cultureId);
            Handler.EndUpdate();
            return culture;
        }

        [IntelliSense("Adds a new security role to the model."), Tests.GenerateTest()]
        public ModelRole AddRole(string name = null)
        {
            Handler.BeginUpdate("add role");
            var role = ModelRole.CreateNew(this);
            //role.InitRLSIndexer();
            if (!string.IsNullOrEmpty(name)) role.Name = name;
            Handler.EndUpdate();
            return role;
        }

        [IntelliSense("Adds a new data source to the model."), Tests.GenerateTest()]
        public ProviderDataSource AddDataSource(string name = null)
        {
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(ProviderDataSource)))
                throw new PowerBIGovernanceException("Adding Data Sources to this Power BI Model is not supported.");

            Handler.BeginUpdate("add data source");

            var ds = ProviderDataSource.CreateNew(this, name);
            Handler.EndUpdate();
            return ds;
        }

        [IntelliSense("Adds a new strucured data source to the model."), Tests.GenerateTest(), Tests.CompatibilityLevel(1400)]
        public StructuredDataSource AddStructuredDataSource(string name = null)
        {
            if (Handler.CompatibilityLevel < 1400) throw new InvalidOperationException(Messages.CompatibilityError_StructuredDataSource);
            if (!Handler.PowerBIGovernance.AllowCreate(typeof(StructuredDataSource)))
                throw new PowerBIGovernanceException("Adding Data Sources to this Power BI Model is not supported.");

            Handler.BeginUpdate("add data source");
            var ds = StructuredDataSource.CreateNew(this, name);
            Handler.EndUpdate();
            return ds;
        }
        #endregion
        #region Convenient Collections
        /// <summary>
        /// Iterates all hierarchies on all tables of the model.
        /// </summary>
        [Browsable(false),IntelliSense("A collection of every hierarchy across all tables in the model.")]
        public IEnumerable<Hierarchy> AllHierarchies { get { return Tables.SelectMany(t => t.Hierarchies); } }

        /// <summary>
        /// Iterates all columns on all tables of the model.
        /// </summary>
        [Browsable(false),IntelliSense("A collection of every column across all tables in the model.")]
        public IEnumerable<Column> AllColumns { get { return Tables.SelectMany(t => t.Columns); } }

        /// <summary>
        /// Iterates all calculation items on all calculation groups of the model.
        /// </summary>
        [Browsable(false), IntelliSense("Iterates all calculation items on all calculation groups of the model.")]
        public IEnumerable<CalculationItem> AllCalculationItems { get { return CalculationGroups.SelectMany(t => t.CalculationItems); } }
        /// <summary>
        /// Iterates all partitions on all tables of the model.
        /// </summary>
        [Browsable(false), IntelliSense("A collection of every partition across all tables in the model.")]
        public IEnumerable<Partition> AllPartitions { get { return Tables.SelectMany(t => t.Partitions); } }

        /// <summary>
        /// Iterates all measures on all tables of the model.
        /// </summary>
        [Browsable(false), IntelliSense("A collection of every measure across all tables in the model.")]
        public IEnumerable<Measure> AllMeasures { get { return Tables.SelectMany(t => t.Measures); } }

        /// <summary>
        /// Iterates all levels on all hierarchies on all tables of the model.
        /// </summary>
        [Browsable(false), IntelliSense("A collection of every level in every hierarchy across all tables in the model.")]
        public IEnumerable<Level> AllLevels { get { return Tables.SelectMany(t => t.Hierarchies).SelectMany(h => h.Levels); } }
        #endregion

        [IntelliSense("Metadata related to the latest deployment performed on this model using Tabular Editor."),Category("Metadata")]
        public DeploymentMetadata DeploymentMetadata { get; set; }
        const string DM = "TabularEditor_DeploymentMetadata";
        private void InitDeploymentMetadata()
        {
            var deploymentMetadataJson =
                Handler.CompatibilityLevel >= 1400 && HasExtendedProperty(DM) ? GetExtendedProperty(DM) : GetAnnotation(DM);
            if (deploymentMetadataJson != null)
            {
                try
                {
                    DeploymentMetadata = JsonConvert.DeserializeObject<DeploymentMetadata>(deploymentMetadataJson);
                }
                catch { }
            }
        }
        public void RemoveDeploymentMetadata()
        {
            DeploymentMetadata = null;
            UpdateDeploymentMetadata();
        }

        public void UpdateDeploymentMetadata()
        {
            if (Handler.CompatibilityLevel >= 1400) RemoveExtendedProperty(DM, false);
            RemoveAnnotation(DM, false);
            if (DeploymentMetadata == null) return;

            var json = JsonConvert.SerializeObject(DeploymentMetadata);
            if (Handler.CompatibilityLevel >= 1400)
                SetExtendedProperty(DM, json, ExtendedPropertyType.Json, false);
            else
                SetAnnotation(DM, json, false);
        }

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return Groups;
        }

        protected override bool AllowDelete(out string message)
        {
            message = Messages.CannotDeleteObject;
            return false;
        }

        protected override void Init()
        {
            if (Handler.CompatibilityLevel >= 1400 && MetadataObject.DataAccessOptions == null)
                MetadataObject.DataAccessOptions = new TOM.DataAccessOptions();

            InitDeploymentMetadata();
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                // DataAccessOptions properties:
                case Properties.FASTCOMBINE:
                case Properties.RETURNERRORVALUESASNULL:
                case Properties.LEGACYREDIRECTS:
                    return Handler.CompatibilityLevel >= 1400;
                case Properties.HASLOCALCHANGES:
                    return false;
                default:
                    return base.IsBrowsable(propertyName);
            }
        }
        [Browsable(false)]
        public LogicalGroups Groups { get { return LogicalGroups.Singleton; } }

        [Category("Basic")]
        [IntelliSense("Gets the database object of the model.")]
        public Database Database { get; internal set; }

        private void EnableDataAccessOptions()
        {
            if (MetadataObject.DataAccessOptions == null)
                MetadataObject.DataAccessOptions = new TOM.DataAccessOptions();
        }

        [Category("Data Access Options"),DisplayName("Enable Fast Combine")]
        public bool FastCombine {
            get { return MetadataObject.DataAccessOptions?.FastCombine ?? false; } 
            set {
                EnableDataAccessOptions();
                SetValue(FastCombine, value, v => MetadataObject.DataAccessOptions.FastCombine = (bool)v, Properties.FASTCOMBINE);
            }
        }
        private bool ShouldSerializeFastCombine() { return false; }
        [Category("Data Access Options"),DisplayName("Enable Legacy Redirects")]
        public bool LegacyRedirects {
            get { return MetadataObject.DataAccessOptions?.LegacyRedirects ?? false; } 
            set {
                EnableDataAccessOptions();
                SetValue(LegacyRedirects, value, v => MetadataObject.DataAccessOptions.LegacyRedirects = (bool)v, Properties.LEGACYREDIRECTS);
            }
        }
        private bool ShouldSerializeLegacyRedirects() { return false; }
        [Category("Data Access Options"), DisplayName("Return Error Values As Null")]
        public bool ReturnErrorValuesAsNull
        {
            get { return MetadataObject.DataAccessOptions?.ReturnErrorValuesAsNull ?? false; }
            set {
                EnableDataAccessOptions();
                SetValue(ReturnErrorValuesAsNull, value, v => MetadataObject.DataAccessOptions.ReturnErrorValuesAsNull = (bool)v, Properties.RETURNERRORVALUESASNULL);
            }
        }
        private bool ShouldSerializeReturnErrorValuesAsNull() { return false; }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if(propertyName == Properties.DISCOURAGEIMPLICITMEASURES && DiscourageImplicitMeasures && (bool)newValue == false)
            {
                if (Model.CalculationGroups.Any()) throw new ArgumentOutOfRangeException(Properties.DISCOURAGEIMPLICITMEASURES, "This property must be set 'True' when a model contains calculation groups.");
            }
            if(propertyName == Properties.DEFAULTMODE && (ModeType)newValue != ModeType.Import && (ModeType)newValue != ModeType.DirectQuery)
            {
                throw new ArgumentOutOfRangeException(Properties.DEFAULTMODE, "This property must be set to either 'Import' or 'DirectQuery'.");
            }
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        [Browsable(false)]
        public IEnumerable<CalculationGroupTable> CalculationGroups => Tables.OfType<CalculationGroupTable>();
    }

    internal static partial class Properties
    {
        public const string OBJECTTYPENAME = "ObjectTypeName";
        public const string FASTCOMBINE = "FastCombine";
        public const string LEGACYREDIRECTS = "LegacyRedirects";
        public const string RETURNERRORVALUESASNULL = "ReturnErrorValuesAsNull";
    }

}
