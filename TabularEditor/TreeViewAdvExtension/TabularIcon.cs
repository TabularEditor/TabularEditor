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
            { ObjectType.Partition, TabularIcons.ICON_PARTITION }
        };


        public System.Windows.Forms.ImageList.ImageCollection Images { get; set; }

        protected override Image GetIcon(TreeNodeAdv node)
        {
            var iconIndex = GetIconIndex(node.Tag as ITabularNamedObject);
            if(node.Tag is Folder || node.Tag is LogicalGroup)
            {
                iconIndex = node.IsExpanded ? TabularIcons.ICON_FOLDEROPEN : TabularIcons.ICON_FOLDER;
            }
            return iconIndex >= 0 ? Images[iconIndex] : base.GetIcon(node);
        }

        public static int GetIconIndex(ITabularNamedObject obj)
        {
            if (obj is TabularObject)
            {
                if (obj is CalculatedColumn) return TabularIcons.ICON_CALCCOLUMN;
                if (obj is CalculatedTable)
                {
                    return TabularIcons.ICON_CALCTABLE;
                }
                if (obj is Level) return TabularIcons.ICON_LEVEL1 + (obj as Level).Ordinal;

                int iconIndex;
                if (IconMap.TryGetValue((obj as TabularObject).ObjectType, out iconIndex)) return iconIndex;
            }
            if (obj is PartitionViewTable) return GetIconIndex((obj as PartitionViewTable).Table);
            return -1;
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            base.Draw(node, context);

            var err = (node.Tag as IErrorMessageObject)?.ErrorMessage;
            var needsVal = (node.Tag as IDAXExpressionObject)?.NeedsValidation ?? false;
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
            var needsVal = (node.Tag as IDAXExpressionObject)?.NeedsValidation ?? false;
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
        public const int ICON_KPI = 6;
        public const int ICON_MEASURE = 7;
        public const int ICON_SIGMA = 8;
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
    }

}
