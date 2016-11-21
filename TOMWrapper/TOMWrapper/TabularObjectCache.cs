using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public class TabularObjectCache
    {
        TabularModelHandler handler;
        Dictionary<NamedMetadataObject, TabularNamedObject> cache = new Dictionary<NamedMetadataObject, TabularNamedObject>();

        public TabularNamedObject this[NamedMetadataObject obj]
        {
            get
            {
                TabularNamedObject result;
                if (!cache.TryGetValue(obj, out result))
                {
                    result = TabularNamedObject.CreateFromMetadata(obj,handler);
                    cache.Add(obj, result);
                }
                return result;
            }
        }

        public TabularObjectCache(TabularModelHandler handler)
        {
            this.handler = handler;
        }

        public Model Model { get; private set; }

        public void LoadFromModel(Model model)
        {
            Model = model;
            cache = new Dictionary<NamedMetadataObject, TabularNamedObject>();
            cache.Add(model, TabularNamedObject.CreateFromMetadata(model, handler));

            // Cache all tables, measures, columns, hierarchies and their levels:
            model.Tables.ToList().ForEach(table =>
            {
                cache.Add(table, new TabularTable(table, this));
                table.Measures.ToList().ForEach(m => cache.Add(m, TabularNamedObject.CreateFromMetadata(m, handler)));
                table.Columns.ToList().ForEach(c => cache.Add(c, TabularNamedObject.CreateFromMetadata(c, handler)));
                table.Hierarchies.ToList().ForEach(h =>
                {
                    cache.Add(h, TabularNamedObject.CreateFromMetadata(h, handler));
                    h.Levels.ToList().ForEach(l => cache.Add(l, TabularNamedObject.CreateFromMetadata(l, handler)));
                });
            });
        }

        public void Add(NamedMetadataObject metadataObject, TabularNamedObject tabularObject)
        {
            cache.Add(metadataObject, tabularObject);
        }
    }

}
