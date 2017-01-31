namespace TabularEditor.UndoFramework
{
    class UndoBatchAction : IUndoAction
    {
        public UndoBatchAction(string batchName, int depth, bool begin)
        {
            ActionName = batchName;
            Begin = begin;
            Depth = depth;
        }

        public int Depth { get; private set; }

        internal bool Begin { get; private set; }

        public string ActionName { get; private set; }

        public bool TreeStructureChanged { get { return false; } }

        public void Redo()
        {
        }

        public void Undo()
        {
        }

        public string GetSummary()
        {
            return string.Format("{0} batch: \"{1}\"", Begin ? "Begin" : "End", ActionName);
        }

        public string GetCode()
        {
            return string.Empty;
        }
    }

}
