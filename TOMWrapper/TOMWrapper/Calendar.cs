using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.PropertyGridUI.CollectionEditors;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper
{
    public partial class Calendar: IDaxObject
    {
        private ReferencedByList _referencedBy;

        /// <summary>
        /// The collection of column mappings that define the calendar's time unit associations. This is an alias for <see cref="CalendarColumnGroups"/>.
        /// </summary>
        [Category("Basic"), Description("The collection of column mappings that define the calendar's time unit associations. This is an alias for CalendarColumnGroups."), DisplayName("Column Mappings")]
        [NoMultiselect(), Editor(typeof(CalendarColumnGroupCollectionEditor), typeof(UITypeEditor))]
        public CalendarColumnGroupCollection ColumnMappings => CalendarColumnGroups;

        /// <summary>
        /// Adds a new time unit association to the calendar.
        /// </summary>
        /// <param name="timeUnit">The unit of time</param>
        /// <param name="primaryColumn">The primary column to associate with this time unit</param>
        /// <param name="associatedColumns">Additional associated columns (i.e. columns representing the same time unit as the primary column in other formats, languages, etc.)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TimeUnitColumnAssociation AddTimeUnit(TimeUnit timeUnit, Column primaryColumn,
            params Column[] associatedColumns)
        {
            if (primaryColumn == null) throw new ArgumentNullException(nameof(primaryColumn));
            if (primaryColumn.Table != Table) throw new ArgumentException(string.Format("Column must be part of table '{0}'", Table.Name), nameof(primaryColumn));
            if (CalendarColumnGroups.Find(timeUnit) != null) throw new ArgumentException(string.Format("Time unit '{0}' already has a column association in this calendar.", timeUnit), nameof(timeUnit));

            Handler.BeginUpdate("Add time unit");
            var association = TimeUnitColumnAssociation.CreateNew(this, timeUnit);
            association.PrimaryColumn = primaryColumn;
            if (associatedColumns != null && associatedColumns.Length > 0)
            {
                foreach (var associatedColumn in associatedColumns) association.AssociatedColumns.Add(associatedColumn);
            }

            Handler.EndUpdate();
            return association;
        }

        /// <summary>
        /// Returns all time unit associations in the calendar.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TimeUnitColumnAssociation> GetTimeUnits() =>
            CalendarColumnGroups.OfType<TimeUnitColumnAssociation>();

        /// <summary>
        /// Finds the time unit association for the specified time unit. Returns null if no association exists for the specified time unit.
        /// </summary>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public TimeUnitColumnAssociation FindTimeUnit(TimeUnit timeUnit)
        {
            return CalendarColumnGroups.Find(timeUnit);
        }

        /// <summary>
        /// Removes the specified time unit association from the calendar.
        /// </summary>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public bool DeleteTimeUnit(TimeUnit timeUnit)
        {
            var association = CalendarColumnGroups.Find(timeUnit);
            if (association == null) return false;
            association.Delete();
            return true;
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
            if (cancel) return;

            if (propertyName == Properties.NAME)
            {
                // Check that the name is valid:


                // When formula fixup is enabled, we need to begin a new batch of undo operations, as this
                // name change could result in expression changes on multiple objects:
                if (Handler.Settings.AutoFixup) Handler.BeginUpdate("Set Property 'Name'");
            }
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            switch (propertyName)
            {
                case nameof(CalendarColumnGroups):
                    HandleCalendarMappingChange();
                    break;

                case nameof(Name):
                    if (Handler.Settings.AutoFixup)
                    {
                        // Fixup is not performed during an undo operation. We rely on the undo stack to fixup the expressions
                        // affected by the name change (the undo stack should contain the expression changes that were made
                        // when the name was initially changed).
                        if (!Handler.UndoManager.UndoInProgress) FormulaFixup.DoFixup(this, true);
                        FormulaFixup.BuildDependencyTree();
                        Handler.EndUpdate();
                    }
                    break;
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        internal void HandleCalendarMappingChange()
        {
            // Nothing to do, as TE2 does not perform semantic analysis involving calendar mappings
        }

        private protected override bool IsBrowsable(string propertyName)
        {
            if (propertyName == nameof(CalendarColumnGroups)) return false; // Hidden, because we would rather expose its alias, ColumnMappings in the PropertyGrid.
            return base.IsBrowsable(propertyName);
        }

        /// <summary>
        /// Gets the list of objects that reference this calendar based on their DAX expressions.
        /// </summary>
        [Browsable(false)]
        public ReferencedByList ReferencedBy
        {
            get
            {
                if (_referencedBy == null)
                    _referencedBy = new ReferencedByList();
                return _referencedBy;
            }
        }

        /// <summary>
        /// Gets a string that may be used for referencing the calendar in a DAX expression.
        /// </summary>
        [Browsable(false)]
        public string DaxObjectName => string.Format("'{0}'", Name.Replace("'", "''"));

        /// <summary>
        /// Gets a string that may be used for referencing the caæemdar in a DAX expression.
        /// </summary>
        [Browsable(false)]
        public string DaxObjectFullName => DaxObjectName;

        /// <summary>
        /// Gets a string that may be used for referencing the calendar in a DAX expression.
        /// </summary>
        [Browsable(false)]
        public string DaxTableName => DaxObjectName;
    }

    public partial class CalendarCollection: ITabularTableObject, ITabularObjectContainer
    {
        internal static string GetNewName(Table table, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Calendar";

            // For calendars, we must ensure that the new calendar name is unique in the model
            // and that no tables have the same name.
            return table.Model.AllCalendars.GetNewName(prefix, table.Model.Tables);
        }

        internal static string GetNewName(Model model, string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix)) prefix = "New Calendar";

            // For calendars, we must ensure that the new calendar name is unique in the model
            // and that no tables have the same name.
            return model.AllCalendars.GetNewName(prefix, model.Tables);
        }

        internal override string GetNewName(string prefix = null) =>
            // For calendars, we must ensure that the new calendar name is unique across all tables,
            // which is why we have to override the GetNewName method here. Also, we must make sure
            // that no tables have the same name as the calendar.
            GetNewName(Table, prefix);

        /// <summary>
        /// This property points to the CalendarCollection itself. It is used only to display a clickable
        /// "Calendar" property in the Property Grid, which will open the CalendarCollectionEditor when
        /// clicked.
        /// </summary>
        [DisplayName("Calendars")]
        [Description("The collection of Calendar objects on this Table.")]
        [Category("Basic")]
        [NoMultiselect]
        [Editor(typeof(CalendarCollectionEditor), typeof(UITypeEditor))]
        public CalendarCollection PropertyGridCalendars => this;

        void ITabularObject.ReapplyReferences() => ReapplyReferences();
        void ITabularNamedObject.RemoveReferences() { }

        bool ITabularNamedObject.CanEditName() => false;

        bool ITabularObject.IsRemoved => false;

        int ITabularNamedObject.MetadataIndex => -1;

        Model ITabularObject.Model => Table.Model;

        [ReadOnly(true)]
        string ITabularNamedObject.Name
        {
            get => "Calendars";
            set { }
        }

        ObjectType ITabularObject.ObjectType => ObjectType.CalendarCollection;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

        bool ITabularNamedObject.CanDelete() => false;

        bool ITabularNamedObject.CanDelete(out string message)
        {
            message = "This object cannot be deleted.";
            return false;
        }

        void ITabularNamedObject.Delete()
        {
            throw new NotSupportedException();
        }

        IEnumerable<ITabularNamedObject> ITabularObjectContainer.GetChildren() => this;

        Table ITabularTableObject.Table => Table;
    }
}
