using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Aga.Controls
{
	public static class TextHelper
	{
		public static StringAlignment TranslateAligment(HorizontalAlignment alignment)
		{
			if (alignment == HorizontalAlignment.Left)
				return StringAlignment.Near;
			else if (alignment == HorizontalAlignment.Right)
				return StringAlignment.Far;
			else
				return StringAlignment.Center;
		}

        public static TextFormatFlags TranslateAligmentToFlag(HorizontalAlignment alignment)
        {
            if (alignment == HorizontalAlignment.Left)
                return TextFormatFlags.Left;
            else if (alignment == HorizontalAlignment.Right)
                return TextFormatFlags.Right;
            else
                return TextFormatFlags.HorizontalCenter;
        }

		public static TextFormatFlags TranslateTrimmingToFlag(StringTrimming trimming)
		{
			if (trimming == StringTrimming.EllipsisCharacter)
				return TextFormatFlags.EndEllipsis;
			else if (trimming == StringTrimming.EllipsisPath)
				return TextFormatFlags.PathEllipsis;
			if (trimming == StringTrimming.EllipsisWord)
				return TextFormatFlags.WordEllipsis;
			if (trimming == StringTrimming.Word)
				return TextFormatFlags.WordBreak;
			else
				return TextFormatFlags.Default;
		}

        public static string Splice(this string str, int start, int length,
                                        string replacement)
        {
            return str.Substring(0, start) +
                   replacement +
                   str.Substring(start + length);
        }

        
    }
}
