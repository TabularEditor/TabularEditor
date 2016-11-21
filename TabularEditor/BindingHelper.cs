using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections;

namespace TabularEditor
{
    public class ComboboxItem<T>
    {
        public ComboboxItem(string name, T value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public T Value { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public static class BindingHelper
    {
        public class Item
        {
            public string Text;
            public object Value;
            public override string ToString()
            {
                return Text;
            }
        }

        public static void BindTo<T>(this ComboBox comboBox, IList<T> items, string displayMember, INotifyPropertyChanged currentObject, string currentMember, string nullDisplayValue = "")
            where T: class, INotifyPropertyChanged
        {
            comboBox.Items.Clear();

            var displayProp = typeof(T).GetProperty(displayMember);
            var currentProp = currentObject.GetType().GetProperty(currentMember);

            //List<T> map = new List<T>();
            PropertyChangedEventHandler change = (s, e) =>
            {
                if (e.PropertyName == displayMember)
                {
                    var item = comboBox.Items.OfType<Item>().First(i => i.Value == s);
                    item.Text = displayProp.GetValue(s).ToString();
                    comboBox.Items[comboBox.Items.IndexOf(item)] = item;
                }
            };

            if (nullDisplayValue != null) comboBox.Items.Add(new Item() { Text = nullDisplayValue });

            foreach (var item in items)
            {
                comboBox.Items.Add(new Item() { Text = displayProp.GetValue(item).ToString(), Value = item });
                item.PropertyChanged += change;
            }

            if(items is INotifyCollectionChanged)
            {
                (items as INotifyCollectionChanged).CollectionChanged += (s, e) => {
                    if(e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (var item in e.NewItems)
                        {
                            comboBox.Items.Add(new Item() { Text = displayProp.GetValue(item).ToString(), Value = item });
                            (item as T).PropertyChanged += change;
                        }
                    }
                    if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (var item in e.OldItems)
                        {
                            var cbItem = comboBox.Items.OfType<Item>().First(i => i.Value == item);
                            comboBox.Items.Remove(cbItem);
                            (item as T).PropertyChanged -= change;
                        }
                    }
                };
            }

            EventHandler indexChanged = (s, e) => currentProp.SetValue(currentObject, (comboBox.SelectedItem as Item).Value);
            comboBox.SelectedIndexChanged += indexChanged;

            // Initial value:
            comboBox.SelectedItem = comboBox.Items.OfType<Item>().First(i => i.Value == currentProp.GetValue(currentObject));
        }
    }
}
