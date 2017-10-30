using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    internal class AnnotationCollectionEditor : RefreshGridCollectionEditor
    {
        AnnotationCollection Collection;

        public AnnotationCollectionEditor() : base(typeof(Collection<Annotation>))
        {
        }

        protected override object CreateInstance(Type itemType)
        {
            var result = new Annotation { Name = Collection.Parent.GetNewAnnotationName(), Value = "" };
            return result;
        }

        protected override bool CanRemoveInstance(object value)
        {
            return true;
        }
        protected override Type CreateCollectionItemType()
        {
            return typeof(Annotation);
        }

        protected override string GetDisplayText(object value)
        {
            return (value as Annotation).Name;
        }

        protected override object[] GetItems(object editValue)
        {
            Collection = (editValue as AnnotationCollection);
            return Collection.Keys.Select(k => new Annotation { Name = k, Value = Collection[k].ToString() }).ToArray();
        }

        protected override object SetItems(object editValue, object[] value)
        {
            foreach(Annotation an in value)
            {
                Collection.Parent.SetAnnotation(an.Name, an.Value);
            }
            foreach(var n in Collection.Keys.ToList().Except(value.OfType<Annotation>().Select(an => an.Name)))
            {
                Collection.Parent.RemoveAnnotation(n);
            }

            return Collection;
        }
    }
}
