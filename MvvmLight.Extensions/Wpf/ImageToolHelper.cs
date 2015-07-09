using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MvvmLight.Extensions.Wpf
{
    public class ImageToolHelper
    {
        public static void DecreaseToWidth(string source, string target, int? newWidth, int? newHeight)
        {
            if (!newWidth.HasValue && !newHeight.HasValue)
                throw new Exception("At least one dimension must be set");

            var image = new Bitmap(source);
            int newW = 0;
            int newH = 0;
            if (newWidth.HasValue)
            {
                newW = newWidth.Value;
                if (newHeight.HasValue)
                    newH = newHeight.Value;
                else if (image.Width > newW)
                    newH = Convert.ToInt32(image.Height * (1.0 * newW / image.Width));
                else
                    newH = image.Height;
            }
            else
            {
                newH = newHeight.Value;
                if (image.Height > newH)
                    newW = Convert.ToInt32(image.Width * (1.0 * newH / image.Height));
                else
                    newW = image.Width;
            }
            var newImage = new Bitmap(image, newW, newH);
            newImage.Save(target);
        }
    }
}
