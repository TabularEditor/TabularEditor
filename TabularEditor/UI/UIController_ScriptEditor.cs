using Aga.Controls.Tree;
using FastColoredTextBoxNS;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UI.Tree;
using System.Collections;
using TabularEditor.Scripting;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        AutocompleteMenu popupMenu;

        private int WAVY_STYLE;
        private bool scriptEditorErrorsVisible;

        IntellisenseCollection currentContext = new IntellisenseCollection();

        private void ScriptEditor_Init()
        {
            UI.ScriptEditor.DragEnter += ScriptEditor_DragEnter;
            UI.ScriptEditor.DragLeave += ScriptEditor_DragLeave;
            UI.ScriptEditor.TextChanged += ScriptEditor_TextChanged;

            WAVY_STYLE = 1 << UI.ScriptEditor.AddStyle(new WavyLineStyle(255, System.Drawing.Color.Red));

            popupMenu = new AutocompleteMenu(UI.ScriptEditor);
            popupMenu.AppearInterval = 100;
            popupMenu.SearchPattern = @"[\w\]\)\.]";
            popupMenu.AllowTabKey = true;
            popupMenu.ImageList = FormMain.Singleton.tabularTreeImages;
            //popupMenu.Items.SetAutocompleteItems(new DynamicCollection(popupMenu, UI.ScriptEditor).OrderBy(m => m.Text));
            popupMenu.Items.SetAutocompleteItems(currentContext);
        }

        public void ScriptEditor_HideErrors()
        {
            if (scriptEditorErrorsVisible)
            {
                UI.ScriptEditor.ClearStyle((StyleIndex)WAVY_STYLE);
                scriptEditorErrorsVisible = false;
            }
        }

        private void ScriptEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScriptEditor_HideErrors();

            // Determine context:
            if (UI.ScriptEditor.Selection.CharBeforeStart == '.')
            {
                currentContext.Update(UI.ScriptEditor.Text, UI.ScriptEditor.PlaceToPosition(UI.ScriptEditor.Selection.Start));
            }
        }


        #region Drag drop operations
        private void ScriptEditor_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            Tree_CurrentDragObject = e.Data;
            if (draggedNodes != null)
            {
                // To be able to drag objects into the Expression Editor, we must temporarily swap the String contents of the Drag Data object with the DAX name to be inserted.
                // But first, backup the current string contents, to make sure we can still retrieve it if the drag operation leaves the Expression Editor.
                Tree_DragBackup = (string)e.Data.GetData("Text");
                if (draggedNodes.Any(n => n.Tag is TabularNamedObject))
                {
                    e.Data.SetData(string.Join("\n", draggedNodes.Select(n => n.Tag).OfType<TabularNamedObject>().Select(obj => obj.GetLinqPath())));
                }
                else
                {
                    e.Data.SetData(typeof(string), null);
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void ScriptEditor_DragLeave(object sender, EventArgs e)
        {
            // If a drag backup was set, restore the backup
            if (Tree_DragBackup != null)
            {
                Tree_CurrentDragObject.SetData(Tree_DragBackup);
            }
        }
        #endregion

        public bool ScriptEditor_IsExecuting { get; private set; }
        
        public void ExecuteScript(string script, int offset = 0, bool undoErrors = false)
        {
            var dyn = ScriptEngine.CompileScript(script, out var compilerResults);
            if (compilerResults.Errors.Count > 0)
            {
                var outputMessages = new List<string>();
                foreach (System.CodeDom.Compiler.CompilerError error in compilerResults.Errors)
                {
                    var line = error.Line + offset - 1;
                    if (line >= 0 && line < UI.ScriptEditor.LinesCount)
                    {
                        UI.ScriptEditor.GetLine(line).SetStyle((StyleIndex)WAVY_STYLE);
                        UI.ScriptEditor.Refresh();
                        scriptEditorErrorsVisible = true;
                        outputMessages.Add($"({ line + 1 },{ error.Column }) {(error.IsWarning ? "warning" : "error") } { error.ErrorNumber }: { error.ErrorText }");
                    }
                }
                if (outputMessages.Count > 1)
                {
                    Scripting.ScriptOutputForm.ShowObject(outputMessages, "Compile errors", false);
                }
                else
                {
                    var error = compilerResults.Errors[0];
                    MessageBox.Show($"{ error.ErrorNumber } - { error.ErrorText }", "Error compiling code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (dyn == null) return;

            Handler.BeginUpdate("script");
            try
            {
                ScriptHelper.BeforeScriptExecution();
                ScriptEditor_IsExecuting = true;
                dyn.Invoke(Handler.Model, Selection);
                var actionCount = Handler.EndUpdateAll();
                UI.StatusExLabel.Text = string.Format("Script executed successfully. {0} model change{1}.", actionCount, actionCount == 1 ? "" : "s");
                UI.TreeView.Focus();
            }
            catch (ScriptCancelledException)
            {
                UI.StatusExLabel.Text = "Script cancelled.";
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                var msg = ex.Message;
                if (st.FrameCount >= 2)
                {
                    var frame = st.GetFrame(st.FrameCount - 2);
                    var line = frame.GetFileLineNumber() + offset - 1;
                    if (line >= 0 && line < UI.ScriptEditor.LinesCount)
                    {
                        msg = string.Format("Error on line {0}\n\n{1}\n{2}", line + 1, ex.GetType().Name, msg);
                        UI.ScriptEditor.GetLine(line).SetStyle((StyleIndex)WAVY_STYLE);
                        UI.ScriptEditor.Refresh();
                        scriptEditorErrorsVisible = true;
                    }
                }
                var actionCount = Handler.EndUpdateAll(undoErrors);

                MessageBox.Show(msg, "Error executing code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UI.StatusExLabel.Text = string.Format("Script execution failed. {0} model change{1}.", actionCount, actionCount == 1 ? "" : "s");
            }
            finally
            {
                Handler.Model.Database.CloseReader();
                ScriptEditor_IsExecuting = false;
                ScriptHelper.AfterScriptExecution();
            }
        }

        public void ScriptEditor_ExecuteScript(bool undoErrors)
        {
            using (new Hourglass())
            {
                ScriptEditor_HideErrors();
                Scripting.ScriptOutputForm.Reset();

                var script = !string.IsNullOrEmpty(UI.ScriptEditor.SelectedText) ? UI.ScriptEditor.SelectedText : UI.ScriptEditor.Text;
                var offset = (!string.IsNullOrEmpty(UI.ScriptEditor.SelectedText) ? UI.ScriptEditor.Selection.FromLine : 0);

                ExecuteScript(script, offset, undoErrors);

            }
        }


    }

    public class MethodAutocompleteItemParen: MethodAutocompleteItem
    {
        public MethodAutocompleteItemParen(string text) : base(text)
        {
        }

        public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
        {
            base.OnSelected(popupMenu, e);
            var x = e.Tb.PlaceToPosition(e.Tb.Selection.Start) - 1;
            e.Tb.Selection = e.Tb.GetRange(x,x);
        }

        public override string GetTextForReplace()
        {
            var s = base.GetTextForReplace();
            return s + "()";
        }
    }

    internal class IntellisenseCollection : List<AutocompleteItem>
    {
        TextServices.ScriptParser parser;
        public IntellisenseCollection()
        {
            parser = new TextServices.ScriptParser();
            parser.Types.Add("Selected", typeof(UITreeSelection));
            parser.Types.Add("Model", typeof(Model));
            parser.Types.Add("DaxToken", typeof(TOMWrapper.Utils.DaxToken));
            foreach (var t in typeof(Model).Assembly.ExportedTypes.Where(t => t.IsEnum)) parser.Types.Add(t.Name, t);
            foreach (var t in typeof(Model).Assembly.ExportedTypes.Where(t => t.IsClass)) parser.Classes.Add(t.Name, t);
        }

        public void Update(string script, int pos)
        {
            parser.Lex(script);
            var type = parser.GetTypeAtPos(pos);
            PopulateFromType(type);
        }

        public void PopulateFromType(Type type)
        {
            this.Clear();
            if (type == null) return;
            var compiler = new CSharpCodeProvider();
            MethodAutocompleteItem last = null;

            if(type.IsEnum)
            {
                AddRange(type.GetEnumNames().Select(s => new MethodAutocompleteItem(s) { ImageIndex = TabularIcons.ICON_ENUM }));
                Add(new MethodAutocompleteItem("HasFlag")
                {
                    ToolTipTitle = "bool Enum.HasFlag(Enum flag)",
                    ToolTipText = "Determines whether one or more bits are set in the bit mask.",
                    ImageIndex = TabularIcons.ICON_METHOD
                });
                // TODO: Add our own extensions to the "Types" enum?
                return;
            }

            //return methods
            foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).AsEnumerable())
            {
                if (mi.GetCustomAttribute<IntelliSenseAttribute>() == null) continue;
                switch (mi.Name)
                {
                    case "GetEnumerator":
                        continue;
                }
                
                // Overloaded methods only show up once, but let's display all overrides in the tooltip.
                var methodName = mi.Name;
                string returnTypeName;
                if (mi.ReturnType == typeof(void)) returnTypeName = "void";
                else
                {
                    var returnType = new CodeTypeReference(mi.ReturnType);
                    returnTypeName = compiler.GetTypeOutput(returnType).Replace("TabularEditor.UI.UISelectionList", "IEnumerable").Replace("TabularEditor.TOMWrapper.", "");
                }

                var methodSyntax = returnTypeName + " " + mi.Name + "(" + string.Join(", ", mi.GetParameters().Select(p => p.Name).ToArray()) + ")";

                if (last?.Text == methodName)
                    last.ToolTipText = methodSyntax + Environment.NewLine + last.ToolTipText;
                else
                {
                    last = new MethodAutocompleteItemParen(methodName)
                    {
                        ToolTipTitle = methodSyntax,
                        ToolTipText = mi.GetCustomAttribute<IntelliSenseAttribute>()?.Description ?? string.Empty,
                        ImageIndex = TabularIcons.ICON_METHOD
                    };
                    Add(last);
                }
            }

            // Return constants:
            foreach(var fi in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if(fi.IsLiteral)
                {
                    Add(new MethodAutocompleteItem(fi.Name) {
                        ToolTipTitle = $"(constant) int DaxToken.{fi.Name} = {fi.GetRawConstantValue()}",
                        ImageIndex = TabularIcons.ICON_PROPERTY
                    });
                }
            }

            // Type derives from IEnumerable, so let's add a few select LINQ methods:
            if (type.GetInterface("IEnumerable") != null)
            {
                Add(new MethodAutocompleteItemParen("Select")
                {
                    ToolTipTitle = "object Select(selector)",
                    ToolTipText = "Projects each item of the collection into something else using a lambda expression.\nExample: .Select(Measure => Measure.Table)",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                });
                Add(new MethodAutocompleteItemParen("Any")
                {
                    ToolTipTitle = "bool Any(predicate)",
                    ToolTipText = "Determines if the collection contains any items. Specify a lambda expression to determine if the collection contains any items satisfying the given condition.\nExample: .Any(Measure => Measure.Description == \"\")",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                });
                Add(new MethodAutocompleteItemParen("All")
                {
                    ToolTipTitle = "bool All(predicate)",
                    ToolTipText = "Determines if all items in the collection satisfies the given condition.\nExample: .All(Measure => Measure.Description == \"\")",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                });
                Add(new MethodAutocompleteItemParen("First")
                {
                    ToolTipTitle = type.Name + " First(predicate)",
                    ToolTipText = "Returns the first element of the sequence, satisfying the (optionally) specified condition.",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                });
                Add(new MethodAutocompleteItemParen("Last")
                {
                    ToolTipTitle = type.Name + " Last(predicate)",
                    ToolTipText = "Returns the last element of the sequency, satisfying the (optionally) specified condition.",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                });
                Add(new MethodAutocompleteItemParen("Take")
                {
                    ToolTipTitle = "IEnumerable<" + type.Name + "> Take(count)",
                    ToolTipText = "Returns a specified number of contiguous elements from the start of a sequence.",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                });
                Add(new MethodAutocompleteItemParen("Skip")
                {
                    ToolTipTitle = "IEnumerable<" + type.Name + "> Skip(count)",
                    ToolTipText = "Bypasses a specified number of elements in a sequence and then returns the remaining elements.",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                });

                // Below adds all LINQ methods, which may be overkill:
                /*foreach (var method in typeof(System.Linq.Enumerable)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public).Select(m => m.Name).Distinct())
                {
                    yield return new MethodAutocompleteItem(method + "()")
                    {
                        ToolTipTitle = method,
                        ImageIndex = TabularIcons.ICON_METHOD
                    };
                }*/
            }

            //return static properties of the class
            foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.GetCustomAttribute<IntelliSenseAttribute>() == null && 
                    pi.GetCustomAttribute<DisplayNameAttribute>() == null &&
                    pi.GetCustomAttribute<BrowsableAttribute>() == null) continue;
                var propType = new CodeTypeReference(pi.PropertyType);
                var propTypeName = compiler.GetTypeOutput(propType).Replace("TabularEditor.UI.UISelectionList", "IEnumerable").Replace("TabularEditor.TOMWrapper.", "");

                var canRead = pi.CanRead && pi.GetMethod.IsPublic;
                var canWrite = pi.CanWrite && pi.SetMethod.IsPublic;

                Add(new MethodAutocompleteItem(pi.Name)
                {
                    ToolTipTitle = string.Format("{0} {1} {{ {2}}}", propTypeName, pi.Name, (canRead ? "get; " : "") + (canWrite ? "set; " : "")),
                    ToolTipText = pi.GetCustomAttribute<IntelliSenseAttribute>()?.Description ?? string.Empty,
                    ImageIndex = TabularIcons.ICON_PROPERTY
                });
            }

            this.Sort((a, b) => a.Text.CompareTo(b.Text));
        }
    }

    /// <summary>
    /// Builds list of methods and properties for current class name was typed in the textbox
    /// </summary>
    internal class DynamicCollection : IEnumerable<AutocompleteItem>
    {
        private AutocompleteMenu menu;
        private FastColoredTextBox tb;

        public DynamicCollection(AutocompleteMenu menu, FastColoredTextBox tb)
        {
            this.menu = menu;
            this.tb = tb;
        }

        public IEnumerator<AutocompleteItem> GetEnumerator()
        {
            var compiler = new CSharpCodeProvider();
            // Just to prove a point...

            //get current fragment of the text
            var text = tb.GetLineText(menu.Fragment.ToLine);

            //extract class name (part before dot)
            var parts = text.Split('.');
            if (parts.Length < 2)
                yield break;
            var obj = parts[0];
            Type type = null;
            if (obj == "Selected") type = typeof(UITreeSelection);
            else if (obj == "Model") type = typeof(Model);

            if (type == null) yield break;

            PropertyInfo prop;

            for (var i = 1; i < parts.Length - 1; i++)
            {
                
                var brackets = parts[i].IndexOf('[');
                if (brackets > 0)
                    if (parts[i].IndexOf(']') > brackets + 1)
                    {
                        prop = type.GetProperty(parts[i].Substring(0, brackets));
                        if (prop != null)
                        {
                            var indexerType = prop.PropertyType.GetDefaultMembers().OfType<PropertyInfo>().FirstOrDefault()?.PropertyType;
                            if (indexerType != null)
                            {
                                type = indexerType;
                                continue;
                            }
                            else yield break;
                        }
                        else yield break;
                    }
                    else yield break;

                var parenthesis = parts[i].IndexOf('(');
                if (parenthesis > 0)
                    if (parts[i].IndexOf(')') > brackets)
                    {
                        var method = type.GetMethod(parts[i].Substring(0, parenthesis));
                        if (method != null) { type = method.ReturnType; continue; } else yield break;
                    }
                    else yield break;

                prop = type.GetProperty(parts[i]);
                if (prop != null) { type = prop.PropertyType; continue; }
                else yield break;
            }

            if (type == null)
                yield break;

            MethodAutocompleteItem last = null;

            //return methods
            foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName && m.DeclaringType != typeof(object)).AsEnumerable())
            {
                switch(mi.Name)
                {
                    case "GetEnumerator":
                        continue;
                }

                // Overloaded methods only show up once, but let's display all overrides in the tooltip.
                var methodName = mi.Name + "()";
                string returnTypeName;
                if (mi.ReturnType == typeof(void)) returnTypeName = "void";
                else
                {
                    var returnType = new CodeTypeReference(mi.ReturnType);
                    returnTypeName = compiler.GetTypeOutput(returnType).Replace("TabularEditor.UI.UISelectionList", "IEnumerable").Replace("TabularEditor.TOMWrapper.", "");
                }

                var methodSyntax = returnTypeName + " " + mi.Name + "(" + string.Join(", ", mi.GetParameters().Select(p => p.Name).ToArray()) + ")";

                if (last?.Text == methodName) last.ToolTipTitle += "\n" + methodSyntax;
                else
                {
                    last = new MethodAutocompleteItem(methodName)
                    {
                        ToolTipTitle = methodSyntax,
                        ToolTipText = mi.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
                        ImageIndex = TabularIcons.ICON_METHOD
                    };
                    yield return last;
                }
            }

            // Type derives from IEnumerable, so let's add a few select LINQ methods:
            if(type.GetInterface("IEnumerable") != null)
            {
                yield return new MethodAutocompleteItem("Select()")
                {
                    ToolTipTitle = "Select",
                    ToolTipText = "Projects each item of the collection into something else using a lambda expression.\nExample: .Select(Measure => Measure.Table)",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                };
                yield return new MethodAutocompleteItem("Any()")
                {
                    ToolTipTitle = "Any",
                    ToolTipText = "Determines if the collection contains any items. Specify a lambda expression to determine if the collection contains any items satisfying the given criteria.\nExample: .Any(Measure => Measure.Description == \"\")",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                };
                yield return new MethodAutocompleteItem("All()")
                {
                    ToolTipTitle = "All",
                    ToolTipText = "Determines if all items in the collection satisfies the given criteria.\nExample: .All(Measure => Measure.Description == \"\")",
                    ImageIndex = TabularIcons.ICON_EXMETHOD
                };

                // Below adds all LINQ methods, which may be overkill:
                /*foreach (var method in typeof(System.Linq.Enumerable)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public).Select(m => m.Name).Distinct())
                {
                    yield return new MethodAutocompleteItem(method + "()")
                    {
                        ToolTipTitle = method,
                        ImageIndex = TabularIcons.ICON_METHOD
                    };
                }*/
            }

            //return static properties of the class
            foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propType = new CodeTypeReference(pi.PropertyType);
                var propTypeName = compiler.GetTypeOutput(propType).Replace("TabularEditor.UI.UISelectionList", "IEnumerable").Replace("TabularEditor.TOMWrapper.", "");

                yield return new MethodAutocompleteItem(pi.Name)
                {
                    ToolTipTitle = string.Format("{0} {1} {{ {2}}}", propTypeName, pi.Name, (pi.CanRead ? "get; " : "") + (pi.CanWrite ? "set; " : "")),
                    ToolTipText = pi.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty,
                    ImageIndex = TabularIcons.ICON_PROPERTY
                };
            }
        }

        Type FindTypeByName(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                foreach (var t in a.GetTypes())
                    if (t.Name == name)
                    {
                        return t;
                    }
            }

            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
