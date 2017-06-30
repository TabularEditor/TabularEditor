using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI
{
    /// <summary>
    /// Provides a set of methods for changing multiple object properties at once, as well
    /// as accessing specific items by name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UISelectionList<T> : IEnumerable<T> where T : ITabularNamedObject
    {
        #region IEnumerable implementation

        private List<T> _list = null;
        [IntelliSense("Provide a lambda statement that is executed once for each object in the collection.\nExample: .ForEach(obj => obj.Name += \" OLD\");")]
        public void ForEach(Action<T> action)
        {
            if (_list == null)
                _list = this.ToList();

            _list.ForEach(action);
        }

        private int _count = -1;
        public int Count
        {
            get
            {
                if (_count == -1)
                    _count = _items.Count();
                return _count;
            }
        }

        [IntelliSense("Filters the collection using a lambda expression which should return true.\nExample: .Where(Measure => Measure.Description == \"\")")]
        public UISelectionList<T> Where(Func<T,bool> predicate)
        {
            return new UISelectionList<T>(this.Where<T>(predicate));
        }

        private IEnumerable<T> _items;

        protected internal UISelectionList(IEnumerable<T> items)
        {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Finds an object in the collection by name, using DAX syntax:
        ///     [Measure Name]
        ///     'Table Name'[Column Name]
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public T this[string search]
        {
            get
            {
                return this.FindByDAX(search);
            }
        }

        [IntelliSense("Sets the DisplayFolder property of all objects in the collection at once.")]
        public virtual string DisplayFolder
        {
            get
            {
                var c = this.OfType<IDetailObject>();
                if (c.Count() != 1) throw new Exception("The collection does not contain exactly one object with the DisplayFolder property.");
                return c.First().DisplayFolder;
            }
            set
            {
                Handler.BeginUpdate("display folder");
                this.OfType<IDetailObject>().ToList().ForEach(i => i.DisplayFolder = value);
                Handler.EndUpdate();
            }
        }

        [IntelliSense("Sets the IsHidden property of all objects in the collection at once.")]
        public bool IsHidden
        {
            set
            {
                Handler.BeginUpdate("visibility");
                this.OfType<IHideableObject>().ToList().ForEach(i => i.IsHidden = value);
                Handler.EndUpdate();
            }
        }

        [IntelliSense("Sets the FormatString property of all objects in the collection at once.")]
        public string FormatString
        {
            set
            {
                Handler.BeginUpdate("format string");
                this.OfType<Column>().ToList().ForEach(i => i.FormatString = value);
                this.OfType<Measure>().ToList().ForEach(i => i.FormatString = value);
                Handler.EndUpdate();
            }
        }

        [IntelliSense("Sets the Expression property of all objects in the collection at once.")]
        public string Expression
        {
            get
            {
                var c = this.OfType<IDAXExpressionObject>();
                if (c.Count() > 1) throw new InvalidOperationException("Multiple objects selected");
                return c.FirstOrDefault()?.Expression;
            }
            set
            {
                Handler.BeginUpdate("expression");
                this.OfType<IDAXExpressionObject>().ToList().ForEach(i => i.Expression = value);
                Handler.EndUpdate();
            }
        }

        [IntelliSense("Sets the Description property of all objects in the collection at once.")]
        public string Description
        {
            get
            {
                var c = this.OfType<IDescriptionObject>();
                if (c.Count() > 1) throw new InvalidOperationException("Multiple objects selected");
                return c.FirstOrDefault()?.Description;
            }
            set
            {
                Handler.BeginUpdate("description");
                this.OfType<IDescriptionObject>().ToList().ForEach(i => i.Description = value);
                Handler.EndUpdate();
            }
        }

        [IntelliSense("Adds all objects in the collection to the given perspective.")]
        public void ShowInPerspective(string perspectiveName)
        {
            ShowInPerspective(Handler.Model.Perspectives[perspectiveName]);
        }
        [IntelliSense("Adds all objects in the collection to the given perspective.")]
        public void ShowInPerspective(Perspective perspective)
        {
            Handler.BeginUpdate("show in perspetive");
            this.OfType<ITabularPerspectiveObject>().ToList().ForEach(i => i.InPerspective[perspective] = true);
            Handler.EndUpdate();
        }
        [IntelliSense("Removes all objects in the collection from the given perspective.")]
        public void HideInPerspective(string perspectiveName)
        {
            HideInPerspective(Handler.Model.Perspectives[perspectiveName]);
        }
        [IntelliSense("Removes all objects in the collection from the given perspective.")]
        public void HideInPerspective(Perspective perspective)
        {
            Handler.BeginUpdate("show in perspetive");
            this.OfType<ITabularPerspectiveObject>().ToList().ForEach(i => i.InPerspective[perspective] = false);
            Handler.EndUpdate();
        }
        [IntelliSense("Adds all objects in the collection to all perspectives of the model.")]
        public void ShowInAllPerspectives()
        {
            Handler.BeginUpdate("show in all perspectives");
            this.OfType<ITabularPerspectiveObject>().ToList().ForEach(i => i.InPerspective.All() );
            Handler.EndUpdate();
        }
        [IntelliSense("Removes all objects in the collection from all perspectives of the model.")]
        public void HideInAllPerspectives()
        {
            Handler.BeginUpdate("show in all perspectives");
            this.OfType<ITabularPerspectiveObject>().ToList().ForEach(i => i.InPerspective.None() );
            Handler.EndUpdate();
        }

        [IntelliSense("Specify a search pattern and a replacement value, that will be applied to the Names of the objects in the collection.")]
        public void Rename(string pattern, string replacement, bool regex = false, bool includeNameTranslations = false)
        {
            var objects = this.ToList();
            Handler.DelayBuildDependencyTree = true;
            Handler.BeginUpdate("rename" + (objects.Count > 1 ? " objects" : ""));

            int errCount = 0;
            if (regex)
            {
                var rgc = new Regex(pattern, objects.Count > 10 ? RegexOptions.Compiled : RegexOptions.None);
                foreach (var obj in objects)
                    try
                    {
                        obj.Name = rgc.Replace(obj.Name, replacement);
                        if(includeNameTranslations && obj is ITranslatableObject)
                        {
                            var trans = (obj as ITranslatableObject).TranslatedNames;
                            foreach (var culture in trans.Keys)
                            {
                                if (!string.IsNullOrEmpty(trans[culture])) trans[culture] = rgc.Replace(trans[culture], replacement);
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        errCount++;
                    }
            }
            else
            {
                foreach (var obj in objects)
                    try
                    {
                        obj.Name = obj.Name.Replace(pattern, replacement);
                        if (includeNameTranslations && obj is ITranslatableObject)
                        {
                            var trans = (obj as ITranslatableObject).TranslatedNames;
                            foreach (var culture in trans.Keys)
                            {
                                if (!string.IsNullOrEmpty(trans[culture])) trans[culture] = trans[culture].Replace(pattern, replacement);
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        errCount++;
                    }
            }
            if (errCount > 0) System.Windows.Forms.MessageBox.Show(string.Format("{0} item{1} could not be renamed, since the replaced name was invalid.", errCount, errCount > 1 ? "s" : ""), "Errors during batch rename", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            Handler.EndUpdate();
            Handler.DelayBuildDependencyTree = false;
            Handler.BuildDependencyTree();
        }

        [IntelliSense("Specify a search pattern and a replacement value, that will be applied to the Expression of the objects in the collection.")]
        public void ReplaceExpression(string pattern, string replacement, bool regex = false)
        {
            var objects = this.OfType<IDAXExpressionObject>().ToList();
            Handler.BeginUpdate("replace expression");

            if (regex)
            {
                var rgc = new Regex(pattern, objects.Count > 10 ? RegexOptions.Compiled : RegexOptions.None);
                objects.ForEach(i => i.Expression = rgc.Replace(i.Expression, replacement));
            }
            else
            {
                objects.ForEach(i => i.Expression = i.Expression.Replace(pattern, replacement));
            }

            Handler.EndUpdate();
        }

        [IntelliSense("Sets the name of the object (provided the collection only contains a single object).")]
        public string Name
        {
            get
            {
                return this.Cast<ITabularNamedObject>().Summary();
            }
            set
            {
                var count = this.Count();
                if (count == 0) throw new ArgumentException("Nothing selected");
                if (count > 1) throw new ArgumentException("Cannot set the Name property of multiple objects at once. Use the Rename(pattern, replacement) method instead.");
                this.First().Name = value;
            }
        }

        [IntelliSense("Deletes all objects in the collection.")]
        public void Delete()
        {
            var objects = this.ToList();
            if (objects.Count == 0) return;
            Handler.BeginUpdate("delete" + (objects.Count > 1 ? " objects" : ""));

            // TODO: We really need an IDeletable interface...
            this.OfType<TabularNamedObject>().Where(i => !(i is CalculatedTableColumn)).ToList().ForEach(i => i.Delete());
            Handler.EndUpdate();
        }

        private TabularModelHandler Handler { get { return UIController.Current.Handler; } }
    }
}
