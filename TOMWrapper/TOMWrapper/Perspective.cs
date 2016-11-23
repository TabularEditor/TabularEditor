using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class Perspective
    {
        public Perspective() : base(TabularModelHandler.Singleton, new TOM.Perspective() { Name = TabularModelHandler.Singleton.Model.Perspectives.MetadataObjectCollection.GetNewName("Perspective") }, false )
        {

        }

        public virtual void Delete()
        {
            if (Collection != null) Collection.Remove(this);
        }

        internal override void Undelete(ITabularObjectCollection collection)
        {
            var tom = new TOM.Perspective();
            MetadataObject.CopyTo(tom);
            tom.IsRemoved = false;
            MetadataObject = tom;

            base.Undelete(collection);
        }
    }
}
