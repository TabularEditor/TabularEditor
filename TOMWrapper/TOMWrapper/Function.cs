using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper
{
    public partial class Function: IDaxObject, IDaxDependantObject
    {
        [Browsable(false)]
        public bool IsVisible => !IsHidden;
        [Browsable(false)]
        public bool NeedsValidation { get; set; }

        internal const int MinimumCompatibilityLevel = 1000000;

        internal static bool IsValidNameChar(char c)
        {
            return c is >= 'A' and <= 'Z'
                or >= 'a' and <= 'z'
                or >= '0' and <= '9'
                or '_' or '.';
        }
        internal static bool IsValidNameFirstChar(char c)
        {
            return c is >= 'A' and <= 'Z'
                or >= 'a' and <= 'z'
                or '_';
        }

        protected override void OnPropertyChanging(string propertyName, object newValue, ref bool undoable, ref bool cancel)
        {
            if (propertyName == Properties.NAME)
            {
                var newName = newValue as string;
                if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Function name cannot be empty.");
                if (!IsValidNameFirstChar(newName[0])) throw new ArgumentException("Function name must start with a letter or an underscore.");
                if (newName.Any(c => !IsValidNameChar(c))) throw new ArgumentException("Function name contains invalid characters.");
            }

            base.OnPropertyChanging(propertyName, newValue, ref undoable, ref cancel);
        }

        [Browsable(false)]
        public string DaxObjectName => Name;
        [Browsable(false)]
        public string DaxObjectFullName => Name;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DaxTableName => null;

        [Browsable(false)]
        public ReferencedByList ReferencedBy { get; } = new ReferencedByList();

        private DependsOnList _dependsOn;
        [Browsable(false)]
        public DependsOnList DependsOn
        {

            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }
    }

    public partial class FunctionCollection
    {
        protected override string RemoveInvalidNameChars(string name)
        {
            return string.Join(string.Empty, name.Where(Function.IsValidNameChar));
        }
    }
}
