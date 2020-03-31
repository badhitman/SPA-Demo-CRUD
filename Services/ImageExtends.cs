using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace SPADemoCRUD.Services
{
    public static class ImageExtends
    {
        public static Image Resize(this Image current, int maxWidth, int maxHeight)
        {
            int width, height;
            #region reckon size
            if (current.Width > current.Height)
            {
                width = maxWidth;
                height = Convert.ToInt32(current.Height * maxHeight / (double)current.Width);
            }
            else
            {
                width = Convert.ToInt32(current.Width * maxWidth / (double)current.Height);
                height = maxHeight;
            }
            #endregion

            #region get resized bitmap
            var canvas = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(canvas))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(current, 0, 0, width, height);
            }

            return canvas;
            #endregion
        }

        public static byte[] ToByteArray(this Image current)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                current.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
    }
}
