using Aga.Controls.Tree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using static System.Windows.Forms.ImageList;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.UI.Tree
{
    public class TabularIcon : Aga.Controls.Tree.NodeControls.NodeIcon
    {
        public static readonly Dictionary<ObjectType, int> IconMap = new Dictionary<ObjectType, int>()
        {
            { ObjectType.Table, TabularIcons.ICON_TABLE },
            { ObjectType.Hierarchy, TabularIcons.ICON_HIERARCHY },
            { ObjectType.Column, TabularIcons.ICON_COLUMN },
            { ObjectType.Measure, TabularIcons.ICON_MEASURE },
            { ObjectType.KPI, TabularIcons.ICON_KPI },
            { ObjectType.Model, TabularIcons.ICON_CUBE },
            { ObjectType.Level, TabularIcons.ICON_LEVEL1 },
            { ObjectType.Culture, TabularIcons.ICON_CULTURE },
            { ObjectType.Relationship, TabularIcons.ICON_LINK },
            { ObjectType.Perspective, TabularIcons.ICON_PERSPECTIVE },
            { ObjectType.Role, TabularIcons.ICON_ROLE },
            { ObjectType.DataSource, TabularIcons.ICON_DATASOURCE },
            { ObjectType.Partition, TabularIcons.ICON_PARTITION },
            { ObjectType.Expression, TabularIcons.ICON_EFFECTS },
            { ObjectType.PartitionCollection, TabularIcons.ICON_PARTITION },
            { ObjectType.CalculationGroupTable, TabularIcons.ICON_TABLECALC },
            { ObjectType.CalculationItem, TabularIcons.ICON_CALCULATOR }
        };


        public System.Windows.Forms.ImageList.ImageCollection Images { get; set; }

        protected override Image GetIcon(TreeNodeAdv node)
        {
            var iconIndex = GetIconIndex(node.Tag as ITabularNamedObject, node.IsExpanded);
            return iconIndex >= 0 ? Images[iconIndex] : base.GetIcon(node);
        }

        public static int GetIconIndex(ITabularObject obj, bool isExpanded = false)
        {
            int iconIndex = -1;

            switch (obj.ObjectType)
            {
                case ObjectType.Table:
                    var t = obj as Table;
                    if (t.DataCategory == "Time" && t.Columns.Any(c => c.IsKey))
                        return obj is CalculatedTable ? TabularIcons.ICON_CALCTIMETABLE : TabularIcons.ICON_TIMETABLE;
                    return obj is CalculatedTable ? TabularIcons.ICON_CALCTABLE : TabularIcons.ICON_TABLE;

                case ObjectType.Column:
                    if (obj is DataColumn dc && dc.Table is CalculationGroupTable && dc.SourceColumn.EqualsI("name"))
                        return TabularIcons.ICON_FIELDCOLUMN;
                    else
                        return obj is CalculatedColumn ? TabularIcons.ICON_CALCCOLUMN : TabularIcons.ICON_COLUMN;

                case ObjectType.Level:
                    iconIndex = TabularIcons.ICON_LEVEL1 + (obj as Level).Ordinal;
                    return iconIndex > TabularIcons.ICON_LEVEL12 ? TabularIcons.ICON_LEVEL12 : iconIndex;

                case ObjectType.Folder:
                case ObjectType.Group:
                    return isExpanded ? TabularIcons.ICON_FOLDEROPEN : TabularIcons.ICON_FOLDER;
                case ObjectType.CalculationItemCollection:
                    return isExpanded ? TabularIcons.ICON_FOLDEROPENCALC : TabularIcons.ICON_FOLDERCALC;

                case ObjectType.TablePermission:
                case ObjectType.Function:
                    return TabularIcons.ICON_EFFECTS;

                default:
                    IconMap.TryGetValue(obj.ObjectType, out iconIndex);
                    return iconIndex;
            }
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            base.Draw(node, context);

            var err = (node.Tag as IErrorMessageObject)?.ErrorMessage;
            var needsVal = (node.Tag as IExpressionObject)?.NeedsValidation ?? false;
            if (needsVal)
            {
                context.Graphics.DrawImage(Images[TabularIcons.ICON_QUESTION], 4 + context.Bounds.Left, 3 + context.Bounds.Top);
            }
            else if (!string.IsNullOrEmpty(err))
            {
                context.Graphics.DrawImage(Images[TabularIcons.ICON_WARNING], 3 + context.Bounds.Left, 3 + context.Bounds.Top);
            }
        }

        public override string GetToolTip(TreeNodeAdv node)
        {
            var err = (node.Tag as IErrorMessageObject)?.ErrorMessage;
            var needsVal = (node.Tag as IExpressionObject)?.NeedsValidation ?? false;
            return needsVal ? "Expression was changed. Deploy to validate." : err ?? string.Empty;
        }
    }


    static class TabularIcons
    {
        public const int ICON_FOLDER = 0;
        public const int ICON_FOLDEROPEN = 1;
        public const int ICON_TABLE = 2;
        public const int ICON_HIERARCHY = 3;
        public const int ICON_COLUMN = 4;
        public const int ICON_CALCULATOR = 5;
        public const int ICON_MEASURE = 8;
        public const int ICON_CUBE = 9;
        public const int ICON_LINK = 10;
        public const int ICON_LEVEL = 11;
        public const int ICON_CALCCOLUMN = 12;
        public const int ICON_LEVEL1 = 13;
        public const int ICON_LEVEL2 = 14;
        public const int ICON_LEVEL3 = 15;
        public const int ICON_LEVEL4 = 16;
        public const int ICON_LEVEL5 = 17;
        public const int ICON_LEVEL6 = 18;
        public const int ICON_LEVEL7 = 19;
        public const int ICON_LEVEL8 = 20;
        public const int ICON_LEVEL9 = 21;
        public const int ICON_LEVEL10 = 22;
        public const int ICON_LEVEL11 = 23;
        public const int ICON_LEVEL12 = 24;
        public const int ICON_WARNING = 25;
        public const int ICON_QUESTION = 26;
        public const int ICON_METHOD = 27;
        public const int ICON_PROPERTY = 28;
        public const int ICON_EXMETHOD = 29;
        public const int ICON_ENUM = 30;
        public const int ICON_CALCTABLE = 31;
        public const int ICON_PERSPECTIVE = 32;
        public const int ICON_TRANSLATION = 33;
        public const int ICON_ROLE = 34;
        public const int ICON_CULTURE = 35;
        public const int ICON_DATASOURCE = 36;

        public const int ICON_PARTITION = 38;
        public const int ICON_KPI = 39;
        public const int ICON_EFFECTS = 40;
        public const int ICON_TIMETABLE = 41;
        public const int ICON_CALCTIMETABLE = 42;

        public const int ICON_DATABASE = 43;
        public const int ICON_VIEW = 44;

        public const int ICON_FIELDCOLUMN = 45;
        public const int ICON_FIELD = 46;

        public const int ICON_FOLDERCALC = 47;
        public const int ICON_FOLDEROPENCALC = 48;
        public const int ICON_TABLECALC = 49;
    }

}
