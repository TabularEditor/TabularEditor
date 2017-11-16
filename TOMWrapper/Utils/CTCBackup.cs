using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.Utils
{

    class CTCBackup
    {
        public static CTCBackup BackupColumn(CalculatedTableColumn ctc)
        {
            var table = ctc.Table;

            var backup = new CTCBackup()
            {
                TableName = ctc.Table.Name,
                ColumnName = ctc.Name
            };

            // Backup SortByColumn:
            backup.SortByColumnName = ctc.SortByColumn?.Name;
            ctc.SortByColumn = null;


            // Backup translations and perspectives:
            backup.InPerspective = ctc.InPerspective.Copy();
            ctc.InPerspective.None();

            backup.TranslatedNames = ctc.TranslatedNames.Copy();
            ctc.TranslatedNames.Clear();

            backup.TranslatedDisplayFolders = ctc.TranslatedDisplayFolders.Copy();
            ctc.TranslatedDisplayFolders.Clear();

            backup.TranslatedDescriptions = ctc.TranslatedDescriptions.Copy();
            ctc.TranslatedDescriptions.Clear();


            // Backup relationships:
            foreach (var rel in ctc.Model.Relationships)
            {
                if (rel.FromColumn == ctc)
                {
                    backup.FromColumnInRelationships.Add(rel.ID);
                    rel.FromColumn = backup.CreateDummyColumn(table, rel.ToColumn.DataType);
                }
                if (rel.ToColumn == ctc)
                {
                    backup.ToColumnInRelationships.Add(rel.ID);
                    rel.ToColumn = backup.CreateDummyColumn(table, rel.ToColumn.DataType);
                }
            }

            // Backup levels:
            foreach (var level in ctc.Model.AllLevels)
            {
                if (level.Column == ctc)
                {
                    backup.Levels.Add(string.Format("[{0}].[{1}]", level.Hierarchy.Name, level.Name));
                    level.Column = backup.CreateDummyColumn(table);
                }
            }

            // TODO: Variations
            // TODO: OLS

            return backup;
        }

        private CTCBackup()
        {
        }

        private CalculatedColumn CreateDummyColumn(Table table, DataType dataType = DataType.String)
        {
            var result = table.AddCalculatedColumn(Guid.NewGuid().ToString());
            result.IsHidden = true;
            result.DataType = dataType;
            DummyColumns.Add(result.Name);
            return result;
        }

        public void Restore(Model model)
        {
            if (!model.Tables.Contains(TableName)) return;
            var table = model.Tables[TableName];

            if (!table.Columns.Contains(ColumnName)) return;
            var column = table.Columns[ColumnName];

            // Restore perspectives and translations:
            column.InPerspective.CopyFrom(InPerspective);
            column.TranslatedNames.CopyFrom(TranslatedNames);
            column.TranslatedDisplayFolders.CopyFrom(TranslatedDisplayFolders);
            column.TranslatedDescriptions.CopyFrom(TranslatedDescriptions);

            // Restore SortByColumn:
            if (SortByColumnName != null && table.Columns.Contains(SortByColumnName))
            {
                column.SortByColumn = table.Columns[SortByColumnName];
            }

            // Restore relationships:
            foreach (var rId in FromColumnInRelationships) model.Relationships[rId].FromColumn = column;
            foreach (var rId in ToColumnInRelationships) model.Relationships[rId].ToColumn = column;

            // Restore levels:
            foreach (var levelId in Levels)
            {
                var hierarchyName = levelId.Substring(1, levelId.IndexOf("].[") - 1);
                var levelName = levelId.Substring(levelId.IndexOf("].[") + 2, levelId.Length - levelId.IndexOf("].[") - 3);

                if (!table.Hierarchies.Contains(hierarchyName)) continue;
                var hierarchy = table.Hierarchies[hierarchyName];

                if (!hierarchy.Levels.Contains(levelName)) continue;
                var level = hierarchy.Levels[levelName];

                level.Column = column;
            }

            // TODO: Variations
            // TODO: OLS
        }

        private string TableName;
        private string ColumnName;
        private string SortByColumnName;
        private Dictionary<string, bool> InPerspective;
        private Dictionary<string, string> TranslatedNames;
        private Dictionary<string, string> TranslatedDisplayFolders;
        private Dictionary<string, string> TranslatedDescriptions;
        private List<string> FromColumnInRelationships = new List<string>();
        private List<string> ToColumnInRelationships = new List<string>();
        private List<string> Levels = new List<string>();
        private List<string> DummyColumns = new List<string>();
    }

}
