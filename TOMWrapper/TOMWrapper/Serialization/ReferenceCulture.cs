using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper.Serialization
{
    internal class ReferenceCulture
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public ReferenceModel Model { get; set; }

        public static ReferenceCulture Create(TOM.Database database)
        {
            return new ReferenceCulture
            {
                Name = database.Name,
                Id = database.ID,
                Model = ReferenceModel.Create(database.Model)
            };
        }
    }

    internal class ReferenceModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }
        public ReferenceTable[] Tables { get; set; }
        public bool ShouldSerializeTables() { return Tables?.Length > 0; }
        public ReferencePerspective[] Perspectives { get; set; }
        public bool ShouldSerializePerspectives() { return Perspectives?.Length > 0; }
        public ReferenceRole[] Roles { get; set; }
        public bool ShouldSerializeRoles() { return Roles?.Length > 0; }


        public static ReferenceModel Create(TOM.Model model)
        {
            return new ReferenceModel
            {
                Name = model.Name,
                Description = model.Description,
                Tables = model.Tables.Select(t => ReferenceTable.Create(t)).ToArray(),
                Perspectives = model.Perspectives.Select(p => ReferencePerspective.Create(p)).ToArray(),
                Roles = model.Roles.Select(r => ReferenceRole.Create(r)).ToArray()
            };
        }
    }

    internal class ReferenceTable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }
        public ReferenceColumn[] Columns { get; set; }
        public bool ShouldSerializeColumns() { return Columns?.Length > 0; }
        public ReferenceMeasure[] Measures { get; set; }
        public bool ShouldSerializeMeasures() { return Measures?.Length > 0; }
        public ReferenceHierarchy[] Hierarchies { get; set; }
        public bool ShouldSerializeHierarchies() { return Hierarchies?.Length > 0; }

        public static ReferenceTable Create(TOM.Table table)
        {
            return new ReferenceTable
            {
                Name = table.Name,
                Description = table.Description,
                Columns = table.Columns.Select(c => ReferenceColumn.Create(c)).ToArray(),
                Measures = table.Measures.Select(m => ReferenceMeasure.Create(m)).ToArray(),
                Hierarchies = table.Hierarchies.Select(h => ReferenceHierarchy.Create(h)).ToArray()
            };
        }
    }

    internal class ReferenceColumn
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }
        public string DisplayFolder { get; set; }
        public bool ShouldSerializeDisplayFolder() { return !string.IsNullOrEmpty(DisplayFolder); }

        public static ReferenceColumn Create(TOM.Column column)
        {
            return new ReferenceColumn
            {
                Name = column.Name,
                Description = column.Description,
                DisplayFolder = column.DisplayFolder
            };
        }
    }

    internal class ReferenceMeasure
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }
        public string DisplayFolder { get; set; }
        public bool ShouldSerializeDisplayFolder() { return !string.IsNullOrEmpty(DisplayFolder); }

        public static ReferenceMeasure Create(TOM.Measure measure)
        {
            return new ReferenceMeasure
            {
                Name = measure.Name,
                Description = measure.Description,
                DisplayFolder = measure.DisplayFolder
            };
        }
    }

    internal class ReferenceHierarchy
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }
        public string DisplayFolder { get; set; }
        public bool ShouldSerializeDisplayFolder() { return !string.IsNullOrEmpty(DisplayFolder); }
        public ReferenceLevel[] Levels { get; set; }
        public bool ShouldSerializeLevels() { return Levels?.Length > 0; }

        public static ReferenceHierarchy Create(TOM.Hierarchy hierarchy)
        {
            return new ReferenceHierarchy
            {
                Name = hierarchy.Name,
                Description = hierarchy.Description,
                DisplayFolder = hierarchy.DisplayFolder,
                Levels = hierarchy.Levels.Select(l => ReferenceLevel.Create(l)).ToArray()
            };
        }
    }

    internal class ReferenceLevel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }

        public static ReferenceLevel Create(TOM.Level level)
        {
            return new ReferenceLevel
            {
                Name = level.Name,
                Description = level.Description
            };
        }
    }

    internal class ReferencePerspective
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }

        public static ReferencePerspective Create(TOM.Perspective perspective)
        {
            return new ReferencePerspective
            {
                Name = perspective.Name,
                Description = perspective.Description
            };
        }
    }

    internal class ReferenceRole
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ShouldSerializeDescription() { return !string.IsNullOrEmpty(Description); }

        public static ReferenceRole Create(TOM.ModelRole role)
        {
            return new ReferenceRole
            {
                Name = role.Name,
                Description = role.Description
            };
        }
    }
}