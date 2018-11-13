using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectioDivina.Service.ImageTools
{
    public class ImageProcessor : IDisposable
    {
        public enum VerticalAlignment { top, middle, down };
        public enum HorizontalAlignment { left, center, right };

        public class RGB
        {
            public RGB(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }

            public byte R { get; private set; }
            public byte G { get; private set; }
            public byte B { get; private set; }
        }

        private string fileName;
        private Bitmap image;
        private Graphics graphics;
        private double horizontalPercent;
        private double verticalPercent;

        public ImageProcessor(string fileName)
        {
            this.fileName = fileName;
        }

        public void WriteRelativeText(double horizontalMargin, HorizontalAlignment horizontalAlignment, double verticalMargin, VerticalAlignment verticalAlignment, double fontSize, RGB rgb, string text, string saveTo)
        {
            if (this.fileName.Equals(saveTo, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("Target filename cannot be the same as input filename", "saveTo");

            EnsureObjects();

            using (Font font = new Font("Arial", (float)fontSize))
            {
                PointF location = CalculationLocation(horizontalAlignment, verticalAlignment, horizontalMargin, verticalMargin, rgb, font, text);

                Brush brush = new SolidBrush(Color.FromArgb(rgb.R, rgb.G, rgb.B));

                graphics.DrawString(text, font, brush, location);

            }

            image.Save(saveTo);
        }

        private PointF CalculationLocation(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, double horizontalMargin, double verticalMargin, RGB rgb, Font font, string text)
        {
            float x = 1;
            float y = 1;

            // calculate the rectangular occupied by the text
            SizeF textRect = graphics.MeasureString(text, font);

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.center:
                    {
                        // we ignore given margin in case of this alignment
                        x = (float)Math.Max((image.Width - textRect.Width) / 2, 1);
                        break;
                    }
                case HorizontalAlignment.right:
                    {
                        x = (float)Math.Max(image.Width - textRect.Width - horizontalMargin * horizontalPercent, 1);
                        break;
                    }
                default: //left
                    {
                        x = (float)(horizontalMargin * horizontalPercent); break;

                    }
            }
            switch (verticalAlignment)
            {
                case VerticalAlignment.down:
                    {
                        y = (float)Math.Max(image.Height - textRect.Height - verticalMargin * verticalPercent, 1);
                        break;
                    }
                case VerticalAlignment.middle:
                    {
                        // we ignore given margin in case of this alignment
                        y = (float)Math.Max((image.Height - textRect.Height) / 2, 1);
                        break;
                    }
                default: // top
                    {
                        y = (float)(verticalMargin * verticalPercent); break;
                    }
            }

            return new PointF(x, y);
        }

        public void SaveTo(string newName)
        {

        }
        private void EnsureObjects()
        {
            if (image == null)
            {
                image = (Bitmap)Image.FromFile(fileName);

                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }
            }
            if (graphics == null)
            {
                graphics = Graphics.FromImage(image);
                horizontalPercent = (float)(image.Width / 100.0);
                verticalPercent = (float)(image.Height / 100.0);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (image != null)
                        image.Dispose();
                    if (graphics != null)
                        graphics.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ImageProcessor() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
