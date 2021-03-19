using UT = Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TOMWrapperTest
{
    public class FluentPropertyGridTest: Dictionary<string, PropertyDescriptor>
    {
        public FluentPropertyGridTest Assert(string propertyPath, bool? readOnly = null, string displayName = null, string errorMessage = null)
        {
            UT.Assert.IsTrue(this.ContainsKey(propertyPath), $"Property '{propertyPath}' is not browsable but it should be.");
            if (readOnly.HasValue)
            {
                if (readOnly.Value)
                    IsReadOnly(propertyPath, errorMessage);
                else
                    IsReadWrite(propertyPath, errorMessage);
            }

            if (displayName != null) HasDisplayName(propertyPath, displayName, errorMessage);
            return this;
        }

        public FluentPropertyGridTest HasDisplayName(string propertyPath, string displayName, string errorMessage = null)
        {
            UT.Assert.IsTrue(this.ContainsKey(propertyPath), $"Property '{propertyPath}' is not browsable but it should be.");
            UT.Assert.AreEqual(this[propertyPath].DisplayName, displayName, errorMessage ?? $"Property '{propertyPath}' expected display name: '{displayName}' but actual name: '{this[propertyPath].DisplayName}'");
            return this;
        }

        public FluentPropertyGridTest AreReadOnly(params string[] propertyPath)
        {
            foreach (var prop in propertyPath) IsReadOnly(prop);
            return this;
        }

        public FluentPropertyGridTest AreReadWrite(params string[] propertyPath)
        {
            foreach (var prop in propertyPath) IsReadWrite(prop);
            return this;
        }

        public FluentPropertyGridTest Dump()
        {
            foreach (var key in this.Keys.OrderBy(k => k))
            {
                System.Console.WriteLine($"{key}: ReadOnly={this[key].IsReadOnly}, DisplayName={this[key].DisplayName}");
            }
            return this;
        }

        public FluentPropertyGridTest IsReadOnly(string propertyPath, string errorMessage = null)
        {
            UT.Assert.IsTrue(this.ContainsKey(propertyPath), $"Property '{propertyPath}' is not browsable but it should be.");
            UT.Assert.IsTrue(this[propertyPath].IsReadOnly, errorMessage ?? $"Expected property '{propertyPath}' to be read only, but it wasn't.");
            return this;
        }

        public FluentPropertyGridTest IsReadWrite(string propertyPath, string errorMessage = null)
        {
            UT.Assert.IsTrue(this.ContainsKey(propertyPath), $"Property '{propertyPath}' is not browsable but it should be.");
            UT.Assert.IsFalse(this[propertyPath].IsReadOnly, errorMessage ?? $"Expected property '{propertyPath}' to be read/write, but it wasn't.");
            return this;
        }

        public FluentPropertyGridTest AreHidden(params string[] propertyPath)
        {
            foreach (var prop in propertyPath) IsHidden(prop);
            return this;
        }

        public FluentPropertyGridTest IsHidden(string propertyPath, string errorMessage = null)
        {
            UT.Assert.IsFalse(this.ContainsKey(propertyPath) && this[propertyPath].IsBrowsable, errorMessage ?? $"Property '{propertyPath}' is browsable, but it shouldn't be.");
            return this;
        }
    }
}
