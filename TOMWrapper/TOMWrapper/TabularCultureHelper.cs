using Newtonsoft.Json.Linq;

namespace TabularEditor.TOMWrapper
{
    public static class TabularCultureHelper
    {

        private static void ApplyTrans(string cultureName, ITranslatableObject obj, JToken trans, bool overwrite)
        {
            if (overwrite || string.IsNullOrEmpty(obj.TranslatedNames[cultureName]))
                obj.TranslatedNames[cultureName] = trans.Value<string>("translatedCaption");

            if (overwrite || string.IsNullOrEmpty(obj.TranslatedDescriptions[cultureName]))
                obj.TranslatedDescriptions[cultureName] = trans.Value<string>("translatedDescription");

            if (obj is IFolderObject)
            {
                var o = obj as IFolderObject;
                if (overwrite || string.IsNullOrEmpty(o.TranslatedDisplayFolders[cultureName]))
                    o.TranslatedDisplayFolders[cultureName] = trans.Value<string>("translatedDisplayFolder");
            }
        }

        private static JArray A(this JToken obj, string arrayName)
        {
            var result = obj[arrayName] as JArray;
            return result ?? new JArray();
        }

        /// <summary>
        /// Method returns 'false' if an error was encountered and <paramref name="haltOnError"/> is set to true.
        /// </summary>
        /// <returns></returns>
        public static bool ImportCulture(JObject cult, Model Model, bool overwriteExisting, bool haltOnError)
        {
            var cultureName = cult.Value<string>("name");
            Culture culture;
            if (Model.Cultures.Contains(cultureName)) culture = Model.Cultures[cultureName];
            else
            {
                culture = new Culture(cultureName);
                Model.Cultures.Add(culture);
            }

            if (overwriteExisting)
            {
                // When the user chooses to overwrite existing translations, we simply
                // delete all translations for the particular culture, to make sure
                // that only the translations in the Json appear in the model after import.
                culture.Delete();
                culture = Model.AddTranslation(cultureName);
            }

            var trans = cult["translations"];
            if (trans == null) return true;
            var model = trans["model"];
            if (model == null) return true;

            foreach (var t in model.A("tables"))
            {
                var tn = t.Value<string>("name");
                if (Model.Tables.Contains(tn))
                    ApplyTrans(cultureName, Model.Tables[tn], t, overwriteExisting);
                else if (haltOnError) return false;
                else continue;

                var table = Model.Tables[tn];

                foreach (var c in t.A("columns"))
                {
                    var cn = c.Value<string>("name");
                    if (table.Columns.Contains(cn))
                        ApplyTrans(cultureName, table.Columns[cn], c, overwriteExisting);
                    else if (haltOnError)
                        return false;
                }

                foreach (var m in t.A("measures"))
                {
                    var mn = m.Value<string>("name");
                    if (table.Measures.Contains(mn))
                        ApplyTrans(cultureName, table.Measures[mn], m, overwriteExisting);
                    else if (haltOnError)
                        return false;
                }

                foreach (var h in t.A("hierarchies"))
                {
                    var hn = h.Value<string>("name");
                    if (table.Hierarchies.Contains(hn))
                        ApplyTrans(cultureName, table.Hierarchies[hn], h, overwriteExisting);
                    else if (haltOnError)
                        return false;
                    else continue;

                    var hierarchy = table.Hierarchies[hn];

                    foreach (var l in h.A("levels"))
                    {
                        var ln = l.Value<string>("name");
                        if (hierarchy.Levels.Contains(ln))
                            ApplyTrans(cultureName, hierarchy.Levels[ln], l, overwriteExisting);
                        else if (haltOnError)
                            return false;
                    }
                }
            }

            foreach (var p in model.A("perspectives"))
            {
                var pn = p.Value<string>("name");
                if (Model.Perspectives.Contains(pn))
                    ApplyTrans(cultureName, Model.Perspectives[pn], p, overwriteExisting);
                else if (haltOnError) return false;
            }

            return true;
        }

        public static bool ImportTranslations(string culturesJson, Model Model, bool overwriteExisting, bool haltOnError)
        {

            var json = JObject.Parse(culturesJson);
            var cultures = json["cultures"] as JArray;
            foreach (JObject cult in cultures)
            {
                if (!ImportCulture(cult, Model, overwriteExisting, haltOnError)) return false;
            }

            return true;
        }
    }

}
