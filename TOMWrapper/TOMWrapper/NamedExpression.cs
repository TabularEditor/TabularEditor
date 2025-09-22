using System.Linq;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class NamedExpression: IExpressionObject
    {
        public bool NeedsValidation { get { return false; } private set { } }

        static partial void InitMetadata(TOM.NamedExpression metadataObject, Model parent)
        {
            metadataObject.Kind = TOM.ExpressionKind.M;
        }

        internal override void DeleteLinkedObjects(bool isChildOfDeleted)
        {
            foreach (var p in Model.AllPartitions.OfType<EntityPartition>())
            {
                if (p.ExpressionSource == this) p.ExpressionSource = null;
            }
            foreach (var p in Model.Expressions.Where(e => e != this))
            {
                if (p.ExpressionSource == this) p.ExpressionSource = null;
            }

            base.DeleteLinkedObjects(isChildOfDeleted);
        }
    }
}
