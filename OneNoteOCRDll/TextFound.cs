using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace OneNoteOCRDll
{
    [Serializable]
    public class TextFound
    {

        /// <summary>
        /// Gets the text area.
        /// </summary>
        /// <value>
        /// The text area.
        /// </value>
        public RectangleF TextArea
        {
            get { return Rectangle.Round(_mArea); }
        }

        /// <summary>
        /// The search text
        /// </summary>
        public string SearchText;

        /// <summary>
        /// The image text
        /// </summary>
        public string ImageText;

        /// <summary>
        /// The m area
        /// </summary>
        private RectangleF _mArea;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFound"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="search">The search.</param>
        /// <param name="text">The text.</param>
        public TextFound(float x, float y, float width, float height, string search, string text)
        {
            SearchText = search;
            ImageText = text;
            _mArea = new RectangleF(x, y, width, height);
        }

        /// <summary>
        /// Centers this instance.
        /// </summary>
        /// <returns></returns>
        public Point Center()
        {
            var center_x = TextArea.Location.X + (TextArea.Width / 2);
            var center_y = TextArea.Location.Y + (TextArea.Height / 2);
            return Point.Round(new PointF(center_x, center_y));
        }

    }
}
