using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public class TabularTable : TabularNamedObject, ITabularTableObjectCollection, INotifyPropertyChanged
    {
        public override void Clone(string newName, bool includeTranslations) { throw new InvalidOperationException("Cannot clone table."); }
        public override void Delete()
        {
            // TODO: Delete table
            throw new NotImplementedException();
        }


        TabularObjectCache _cache;

        public event PropertyChangedEventHandler PropertyChanged;

        [Browsable(false)]
        public Table Table { get { return MetadataObject as Table; } }
        public override int Icon { get { return TabularIcons.ICON_TABLE; } }
        public override TabularObjectType Type { get { return TabularObjectType.Table; } }

        [Browsable(false)]
        public string FullPath { get { return Table.Name; } }
        [Browsable(false)]
        public string Path { get { return string.Empty; } }

        public override bool Visible { get { return !Table.IsHidden; } set { Table.IsHidden = !value; } }

        public TabularMeasure AddMeasure(string name)
        {
            return AddMeasure(name, null);
        }
        public TabularMeasure AddMeasure(string name, string displayFolder)
        {
            if(string.IsNullOrEmpty(name))
            {
                name = Table.Measures.GetNewName(Table.Name + " New Measure");
            }

            var metadataObject = new Measure() { Name = name, DisplayFolder = displayFolder ?? string.Empty };
            Table.Measures.Add(metadataObject);
            var tabularMeasure = CreateFromMetadata(metadataObject, Handler) as TabularMeasure;
            _cache.Add(metadataObject, tabularMeasure);
            OnPropertyChanged("Children");
            return tabularMeasure;
        }
        public TabularCalculatedColumn AddCalculatedColumn(string name)
        {
            return AddCalculatedColumn(name, null);
        }
        public TabularCalculatedColumn AddCalculatedColumn(string name, string displayFolder)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Table.Columns.GetNewName(Table.Name + " New Column");
            }

            var metadataObject = new CalculatedColumn() { Name = name, DisplayFolder = displayFolder ?? string.Empty };
            Table.Columns.Add(metadataObject);
            var tabularColumn = CreateFromMetadata(metadataObject, Handler) as TabularCalculatedColumn;
            _cache.Add(metadataObject, tabularColumn);
            OnPropertyChanged("Children");
            return tabularColumn;
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }



        public TabularTable(Table table)
        {
            MetadataObject = table;
            //_cache = cache;
        }
        
        [Browsable(false)]
        public IEnumerable<TabularTableObject> Children
        {
            get
            {
                foreach (var m in Table.Measures) yield return _cache[m] as TabularMeasure;
                foreach (var c in Table.Columns) yield return _cache[c] as TabularColumn;
                foreach (var h in Table.Hierarchies) yield return _cache[h] as TabularHierarchy;
            }
        }

        public string LocalDisplayFolder
        {
            get
            {
                return string.Empty;
            }

            set
            {
                
            }
        }

        public override bool InPerspective(Perspective perspective)
        {
            return Table.InPerspective(perspective);
        }
        public override void SetPerspective(string perspectiveName, bool inPerspective)
        {
            Table.SetPerspective(perspectiveName, inPerspective);
        }

        public string GetDisplayFolder(Culture culture)
        {
            return string.Empty;
        }
    }

}
