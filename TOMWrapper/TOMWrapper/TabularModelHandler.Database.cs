using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class TabularModelHandler
    {
        public struct ConflictInfo
        {
            public long DatabaseVersion;
            public long LoadedVersion;
            public DateTime DatabaseLastUpdate;
            public bool Conflict;
        }

        public ConflictInfo CheckConflicts()
        {
            if (database == null || database?.Server == null) return new ConflictInfo { DatabaseVersion = -1 };
            var s = new TOM.Server();
            s.Connect(database?.Server.ConnectionString);
            var db = s.Databases[database?.ID];
            return new ConflictInfo
            {
                DatabaseVersion = db.Version,
                LoadedVersion = Version,
                DatabaseLastUpdate = db.LastUpdate,
                Conflict = db.Version != Version
            };
        }

        public void KeepAlive()
        {
            if (database?.Server != null) {
                if (database.Server.GetConnectionState(true) != System.Data.ConnectionState.Open)
                    database.Server.Reconnect();
             }
        }

        /// <summary>
        /// Saves the changes to the database. It is the users responsibility to check if changes were made
        /// to the database since it was loaded to the TOMWrapper. You can use Handler.CheckConflicts() for
        /// this purpose.
        /// </summary>
        public void SaveDB()
        {
            if (database?.Server == null || Version == -1)
            {
                throw new InvalidOperationException("The model is currently not connected to any server. Please use Deploy() instead of SaveDB().");
            }

            KeepAlive();

            _disableUpdates = true;
            UndoManager.Enabled = false;

            var sw = Stopwatch.StartNew();

            // TODO: The current detach/attach functionality for calculated tables is not good enough. A better approach is this:
            // Upon saving the model (this method), for each calculated table that needs validation (= has its expression changed),
            // a new calculated table is created, using the new DAX expression, where as the original DAX expression is kept on the
            // original table. After saving, the new table will hold all columns produced by the DAX, which we can then use to check
            // if columns on the original table are still valid. If the old table holds columns that do not appear in the new table,
            // we can show a warning to the user - for example in cases where those columns have translations, perspectives or are
            // used in hierarchies, as SortByColumn properties, etc...

            //DetachCalculatedTableMetadata();
            try
            {
                // TODO: Deleting a column with IsKey = true, then undoing, then saving causes an error... Check if this is still the case.
                database.Model.SaveChanges();

                AttachCalculatedTableMetadata();

                // Rebuild dependency tree, as expressions may have changed server-side when saving:
                //FormulaFixup.BuildDependencyTree();

                // If reattaching Calculated Table metadata caused local changes, let's save the model again:
                //if (database.Model.HasLocalChanges) database.Model.SaveChanges();
            }
            catch (Exception ex)
            {
                Status = "Save to DB error!";
                throw ex;
            }
            finally
            {
                UndoManager.Enabled = true;
                _disableUpdates = false;
            }

            sw.Stop();
            Console.WriteLine("Save took: {0} ms", sw.ElapsedMilliseconds);

            Version = CheckConflicts().DatabaseVersion;

            if (IsConnected) UndoManager.SetCheckpoint();

            Status = "Changes saved.";
            CheckErrors();
        }

        /// <summary>
        /// Temporarily removes all perspective and translation information from the CalculatedTableColumns of
        /// all CalculatedTables in the model. Otherwise, we may get errors when deploying the model, if the
        /// CalculatedTable expression or the source objects have changed such that one or more columns on the
        /// CalculatedTable is removed.
        /// </summary>
        private void DetachCalculatedTableMetadata()
        {
            CTCMetadataBackup = new List<CTCBackup>();

            foreach (var ct in Model.Tables.OfType<CalculatedTable>())
            {
                foreach (var ctc in ct.Columns.OfType<CalculatedTableColumn>())
                {
                    var columnBackup = CTCBackup.BackupColumn(ctc);

                    CTCMetadataBackup.Add(columnBackup);
                }
            }

            // TODO: See issue https://github.com/otykier/TabularEditor/issues/110
            // To handle all possible use cases, we should also clear all SortByColumn properties, Variations
            // Levels in hierarchies, Relationships, and any other properties that reference columns on the
            // calculated table, since we cannot know if these columns will still exist after the model is
            // saved.
        }

        private List<CTCBackup> CTCMetadataBackup;

        /// <summary>
        /// Reattaches any metadata removed from CalculatedTableColumns that are still present (by name) after
        /// succesful deployment.
        /// </summary>
        private void AttachCalculatedTableMetadata()
        {
            //Model.MetadataObject.Database.Update();
            foreach (var ct in Model.Tables.OfType<CalculatedTable>())
            {
                ct.Columns.CreateChildrenFromMetadata();
                Tree.RebuildFolderCacheForTable(ct);
                Tree.OnStructureChanged(ct);
            }
            /*if (CTCMetadataBackup == null) return;
            foreach (var ctcbackup in CTCMetadataBackup)
            {
                ctcbackup.Restore(Model);
            }*/
        }
    }
}
