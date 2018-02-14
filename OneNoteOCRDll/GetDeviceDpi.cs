using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OneNoteOCRDll
{
    public class GetDeviceDpi
    {
        /// <summary>
        /// Gets or sets the image captured.
        /// </summary>
        /// <value>
        /// The image captured.
        /// </value>
        private Bitmap _imageCaptured { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDeviceDpi"/> class.
        /// </summary>
        /// <param name="imageCreated">The image created.</param>
        public GetDeviceDpi(Image imageCreated)
        {
            _imageCaptured = new Bitmap(imageCreated);
        }

        /// <summary>
        /// Transforms to pixels.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="pixel">The pixel.</param>
        public void TransformToPixels(float point, out float pixel)
        {
            using (Graphics g = Graphics.FromImage(_imageCaptured))
            {
                pixel = point * g.DpiX / 72;
            }
        }

    }
}
