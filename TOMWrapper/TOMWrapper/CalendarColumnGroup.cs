using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class CalendarColumnGroup
    {
        /// <summary>
        /// Deletes this column mapping from the calendar.
        /// </summary>
        public override void Delete()
        {
            Calendar.CalendarColumnGroups.Remove(this);
        }
    }

    // Classes in this file derive from CalendarColumnGroup:

    public partial class TimeUnitColumnAssociation
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TimeUnitColumnAssociation"/> class and associates it with the
        /// specified calendar.
        /// </summary>
        /// <remarks>This method creates a new <see cref="TimeUnitColumnAssociation"/> object, initializes
        /// it, and adds it to the <see cref="Calendar.CalendarColumnGroups"/> collection of the specified <paramref
        /// name="parent"/>.</remarks>
        /// <param name="parent">The <see cref="Calendar"/> to which the new <see cref="TimeUnitColumnAssociation"/> will be added. Cannot be
        /// null.</param>
        /// <param name="timeUnit">The time unit to associate with the new column.</param>
        /// <returns>A new <see cref="TimeUnitColumnAssociation"/> instance associated with the specified calendar.</returns>
        public static TimeUnitColumnAssociation CreateNew(Calendar parent, TimeUnit timeUnit)
        {
            var metadataObject = new TOM.TimeUnitColumnAssociation((TOM.TimeUnit)timeUnit);
            var obj = new TimeUnitColumnAssociation(metadataObject);

            parent.CalendarColumnGroups.Add(obj);

            obj.Init();

            return obj;
        }

        private AssociatedColumnCollection _associatedColumns;

        /// <summary>
        /// A collection of columns that should be grouped together with this column when used in visuals (RelatedColumnDetails).
        /// </summary>
        [NoMultiselect]
        [Editor(typeof(CustomDialogEditor), typeof(UITypeEditor))]
        [Category("Options")]
        [DisplayName("Associated Columns")]
        [Description("A collection of associated columns in the association.")]
        public AssociatedColumnCollection AssociatedColumns
        {
            get
            {
                if (_associatedColumns == null)
                {
                    _associatedColumns = new AssociatedColumnCollection(this, nameof(AssociatedColumns));
                }
                return _associatedColumns;
            }
        }

        /// <summary>
        /// Returns a string representation of the object, including the primary column name and time unit.
        /// </summary>
        /// <remarks>If the <see cref="PrimaryColumn"/> is null, the method returns "(Unassigned)".
        /// Otherwise, it returns the <see cref="IDaxObject.DaxObjectName"/> followed by the time unit, formatted
        /// with camel case splitting.</remarks>
        /// <returns>A string that represents the object. The format is either "(Unassigned)" if the primary column is null, or
        /// "<c>PrimaryColumn.DaxObjectName</c> (<c>TimeUnit</c>)" with the time unit formatted.</returns>
        public override string ToString() => PrimaryColumn == null
            ? "(Unassigned)"
            : PrimaryColumn.DaxObjectName + " (" + TimeUnit.ToString().SplitCamelCase() + ")";
    }

    public partial class TimeRelatedColumnGroup
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TimeRelatedColumnGroup"/> class and adds it to the specified
        /// calendar's column groups.
        /// </summary>
        /// <param name="parent">The <see cref="Calendar"/> to which the new <see cref="TimeRelatedColumnGroup"/> will be added. Cannot be
        /// null.</param>
        /// <returns>A new instance of <see cref="TimeRelatedColumnGroup"/> associated with the specified calendar.</returns>
        public static TimeRelatedColumnGroup CreateNew(Calendar parent)
        {
            var metadataObject = new TOM.TimeRelatedColumnGroup();
            var obj = new TimeRelatedColumnGroup(metadataObject);

            parent.CalendarColumnGroups.Add(obj);

            obj.Init();

            return obj;
        }

        private AssociatedColumnCollection _columns;

        /// <summary>
        /// A collection of columns that should be grouped together with this column when used in visuals (RelatedColumnDetails).
        /// </summary>
        [NoMultiselect]
        [Editor(typeof(CustomDialogEditor), typeof(UITypeEditor))]
        [Category("Options")]
        [DisplayName("Columns")]
        [Description("Gets the collection of columns in the group.")]
        public AssociatedColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new AssociatedColumnCollection(this, nameof(Columns));
                    _columns.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Columns), e.OldItems, e.NewItems);
                }
                return _columns;
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Mutable { get; set; } // This is only used to ensure that DevExpress does not treat this type as a primitive type (causing it to appear differently in a property grid)

        /// <summary>
        /// Returns a string representation of the object, listing the names of its columns.
        /// </summary>
        /// <remarks>If the object contains no columns, the method returns the string "(Empty)".
        /// Otherwise, it returns a comma-separated list of column names, sorted alphabetically.</remarks>
        /// <returns>A string representing the object. Returns "(Empty)" if no columns are present, or a sorted, comma-separated
        /// list of column names if columns exist.</returns>
        public override string ToString() => Columns.Count == 0
            ? "(Empty)"
            : string.Join(", ", Columns.Select(c => c.DaxObjectName).OrderBy(n => n));
    }

    public partial class CalendarColumnGroupCollection
    {
        /// <summary>
        /// Gets the TimeUnitColumnAssociation for the specified TimeUnit. Throws KeyNotFoundException if no association is found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public TimeUnitColumnAssociation this[TimeUnit key]
        {
            get
            {
                var result = Find(key);
                if (result == null) throw new KeyNotFoundException($"No TimeUnitColumnAssociation found for TimeUnit '{key}'.");
                return result;
            }
        }

        /// <summary>
        /// Finds the first TimeUnitColumnAssociation in the collection that matches the specified TimeUnit. Returns null if no match is found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TimeUnitColumnAssociation Find(TimeUnit key) => this.OfType<TimeUnitColumnAssociation>().FirstOrDefault(c => c.TimeUnit == key);

        /// <summary>
        /// Gets a string representation of the column group, showing the names of the primary columns mapped to each time unit.
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            this.OfType<TimeUnitColumnAssociation>().Count(c => c.PrimaryColumn != null) == 0
                ? "(No columns mapped)"
                : string.Join("-", this.OfType<TimeUnitColumnAssociation>().Where(c => c.PrimaryColumn != null).Select(c => c.PrimaryColumn.DaxObjectName));
    }
}
