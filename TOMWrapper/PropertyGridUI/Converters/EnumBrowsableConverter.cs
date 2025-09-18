using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TabularEditor.PropertyGridUI.Converters
{
    internal class EnumBrowsableConverter(Type type): EnumConverter(type)
    {
        private readonly Type _enumType = type;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var values = Enum.GetValues(_enumType)
                .Cast<Enum>()
                .Where(e =>
                {
                    var field = _enumType.GetField(e.ToString());
                    var attr = field.GetCustomAttribute<EnumBrowsableAttribute>();
                    return attr?.Browsable != false;
                })
                .ToList();

            return new StandardValuesCollection(values);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class EnumBrowsableAttribute(bool browsable): Attribute
    {
        public bool Browsable { get; } = browsable;
    }
}
