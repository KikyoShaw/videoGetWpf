using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace videoGet.Tools
{
    public static class QRCodeHelper
    {
        // <summary>
        // 生成二维码
        // </summary>
        // <param name="msg">信息</param>
        // <param name="pixel">像素点大小</param>
        // <param name="iconPath">图标路径</param>
        // <param name="iconSize">图标尺寸</param>
        // <param name="iconBorder">图标边框厚度</param>
        // <param name="whiteEdge">二维码白边</param>
        // <returns>BitmapImage</returns>
        public static BitmapImage GetBitmapCode(string msg, int pixel, string iconPath, int iconSize, int iconBorder, bool whiteEdge)
        {
            try
            {
                QRCoder.QRCodeGenerator codeGenerator = new QRCoder.QRCodeGenerator();
                QRCoder.QRCodeData codeData = codeGenerator.CreateQrCode(msg, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCode code = new QRCoder.QRCode(codeData);
                Bitmap icon = null;
                if(!string.IsNullOrEmpty(iconPath))
                    icon = new Bitmap(iconPath);
                Bitmap bmp = code.GetGraphic(10, Color.Black, Color.White, icon, iconSize, iconBorder, whiteEdge);
                return BitmapToBitmapImage(bmp, pixel);
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }
            return null;
        }

        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap, int pixel)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    BitmapImage result = new BitmapImage();
                    result.BeginInit();
                    result.DecodePixelWidth = pixel;
                    result.DecodePixelHeight = pixel;
                    result.CacheOption = BitmapCacheOption.OnLoad;
                    result.StreamSource = stream;
                    result.EndInit();
                    result.Freeze();
                    return result;
                }
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }

            return null;
        }
    }
}
