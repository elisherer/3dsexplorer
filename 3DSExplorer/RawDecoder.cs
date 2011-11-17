using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    static class RawDecoder
    {
        private static Color colorFrom2Bytes(byte[] bytes) //Using GBR655
        {
            int green = (bytes[0] & 0xFC) >> 2;
            int blue = ((bytes[0] & 0x03) << 3);
            blue += ((bytes[1] & 0xE0) >> 5);
            int red = bytes[1] & 0x1F;
            return Color.FromArgb(red * 8, green * 4, blue * 8);
        }

        private static void fillBitmap(int iconSize, int tileSize, int ax, int ay, Bitmap bmp, FileStream fs)
        {
            if (tileSize == 0)
            {
                byte[] rgbVal = new byte[2];
                fs.Read(rgbVal, 0, 2);
                bmp.SetPixel(ax, ay, colorFrom2Bytes(rgbVal));
            }
            else
                for (int y = 0; y < iconSize; y += tileSize)
                    for (int x = 0; x < iconSize; x += tileSize)
                       fillBitmap(tileSize, tileSize / 2, x + ax, y + ay, bmp, fs);
        }

        public static Bitmap CIAIcoDecode(FileStream fs, int iconSize, int tileSize)
        {
            Bitmap bmp = new Bitmap(iconSize, iconSize);
            fillBitmap(iconSize, tileSize, 0, 0, bmp, fs);
            return bmp;
        }
    }
}
