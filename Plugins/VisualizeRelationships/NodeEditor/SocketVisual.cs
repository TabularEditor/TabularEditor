/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2016 Mariusz Komorowski (komorra)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES 
 * OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VisualizeRelationships.Properties;

namespace NodeEditor
{
    public class SocketVisual
    {
        public const float SocketHeight = 16;

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool Input { get; set; }
        public object Value { get; set; }

        public void Draw(Graphics g, Point mouseLocation, MouseButtons mouseButtons)
        {            
            var socketRect = new RectangleF(X, Y, Width, Height);
            var hover = socketRect.Contains(mouseLocation);
            var fontBrush = Brushes.Black;

            if (hover)
            {
                socketRect.Inflate(4, 4);
                fontBrush = Brushes.Blue;
            }

            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;
            
            if (Input)
            {
                var sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;                
                g.DrawString(Name,SystemFonts.SmallCaptionFont, fontBrush, new RectangleF(X+Width+2,Y,1000,Height), sf);
            }
            else
            {
                var sf = new StringFormat();
                sf.Alignment = StringAlignment.Far;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(Name, SystemFonts.SmallCaptionFont, fontBrush, new RectangleF(X-1000, Y, 1000, Height), sf);
            }

            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.DrawImage(Resources.socket, socketRect);
        }

        public RectangleF GetBounds()
        {
            return new RectangleF(X, Y, Width, Height);
        }
    }
}
