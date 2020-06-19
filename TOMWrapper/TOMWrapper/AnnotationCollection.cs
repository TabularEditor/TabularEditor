using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public sealed class Annotation
    {
        public string Name { get; set; }
        [Editor(typeof(MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Value { get; set; }
    }

    [TypeConverter(typeof(IndexerConverter))]
    public sealed class AnnotationCollection : IExpandableIndexer
    {
        public IAnnotationObject Parent { get; private set; }

        internal AnnotationCollection(IAnnotationObject parent)
        {
            Parent = parent;
        }

        public bool EnableMultiLine => true;

        public object this[string index]
        {
            get
            {
                return Parent.GetAnnotation(index);
            }

            set
            {
                if (value == null)
                    Parent.RemoveAnnotation(index);
                else
                    Parent.SetAnnotation(index, value.ToString());
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return Parent.GetAnnotations().Where(key => key != DatabaseHelper.TabularEditorTag);
            }
        }

        public string Summary
        {
            get
            {
                var n = Parent.GetAnnotationsCount() - (Parent.HasAnnotation(DatabaseHelper.TabularEditorTag) ? 1 : 0);
                return string.Format("{0} annotation{1}", n, n == 1 ? "" : "s");
            }
        }

        public string GetDisplayName(string key)
        {
            return key;
        }

        public void Refresh()
        {
            //
        }
    }
}
