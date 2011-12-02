using System.Drawing;
using System.IO;

namespace _3DSExplorer.Utils
{

    public static class ImageUtil
    {
        //Decode RGB5A4 Taken from the dolphin project
        private static readonly int[] lut5to8 = { 0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,
                                                0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,
                                                0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,
                                                0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF };
        private static readonly int[] lut3to8 = { 0x00, 0x24, 0x48, 0x6D, 0x91, 0xB6, 0xDA, 0xFF };

        private static readonly byte[] _tempBytes = new byte[4];
        public enum PixelFormat
        {
            RGBA8 = 0,      //Format32bppArgb
            RGB8 = 1,       //Format24bppRgb
            RGBA5551 = 2,   //Format16bppArgb1555
            RGB565 = 3,     //Format16bppRgb565
            RGBA4 = 4,      //Format16bppArgb4444 / 1555 (?guessing same as A3)
            LA8 = 5,
            HILO8 = 6,
            L8 = 7,
            A8 = 8,
            LA4 = 9,
            L4 = 10,
            ETC1 = 11,      //Ericsson Texture Compression
            ETC1A4 = 12     //Ericsson Texture Compression
        }

        public static int PixelFormatBytes(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.RGBA8:
                    return 4;
                case PixelFormat.RGB8:
                    return 3;
                case PixelFormat.RGBA5551:
                case PixelFormat.RGB565:
                case PixelFormat.LA4:
                case PixelFormat.LA8:
                case PixelFormat.ETC1:
                case PixelFormat.ETC1A4:
                    return 2;
                default:
                    return 1;
            }
        }

        public static Color DecodeColor(int val, PixelFormat pixelFormat)
        {
            int alpha = 0xFF, red, green, blue;
            switch (pixelFormat)
            {
                case PixelFormat.RGBA8:
                    red = (val >> 24) & 0xFF;
                    green = (val >> 16) & 0xFF;
                    blue = (val >> 8) & 0xFF;
                    alpha = val & 0xFF;
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGB8:
                    red = (val >> 16) & 0xFF;
                    green = (val >> 8) & 0xFF;
                    blue = val & 0xFF;
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGBA5551:
                    red = lut5to8[(val >> 11) & 0x1F];
                    green = lut5to8[(val >> 6) & 0x1F];
                    blue = lut5to8[(val >> 1) & 0x1F];
                    alpha = (val & 0x0001) == 1 ? 0xFF : 0x00;
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGB565:
                    red = lut5to8[(val >> 11) & 0x1F];
                    green = ((val >> 5) & 0x3F) * 4;
                    blue = lut5to8[val & 0x1F];
                    return Color.FromArgb(alpha, red, green, blue);
                case PixelFormat.RGBA4:
                    if ((val & 0x8000) > 0) //If Alpha flag is set then it's Full - 0xFF
                    {
                        red = lut5to8[(val >> 10) & 0x1f];     //5 bits
                        blue = lut5to8[(val >> 5) & 0x1f];     //5 bits
                        green = lut5to8[(val) & 0x1f];         //5 bits
                    }
                    else  //Otherwise the alpha channel is the 3 bits after the flag
                    {
                        alpha = lut3to8[(val >> 12) & 0xf];    //4 bits
                        red = 0x11 * ((val >> 8) & 0xf);       //4 bits
                        green = 0x11 * ((val >> 4) & 0xf);     //4 bits
                        blue = 0x11 * ((val) & 0xf);           //4 bits
                    }
                    return Color.FromArgb(alpha, red, green, blue);
                default:
                    return Color.White;
            }
        }

        public static void EncodeColor(Color color, PixelFormat pixelFormat, byte[] bytes)
        {
            switch (pixelFormat)
            {
                case PixelFormat.RGBA8:
                    
                    break;
                case PixelFormat.RGB8:
                    
                    break;
                case PixelFormat.RGBA5551:
                    bytes[1] = (byte)((color.G & 0xE0) >> 5);
                    bytes[1] += (byte)(color.R & 0xF8);
                    bytes[0] = (byte)(color.B >> 3);
                    bytes[0] += (byte)((color.G & 0x1C) << 3);
                    break;
                case PixelFormat.RGB565:
                    bytes[1] = (byte)((color.G & 0xE0) >> 5);
                    bytes[1] += (byte)(color.R & 0xF8);
                    bytes[0] = (byte)(color.B >> 3);
                    bytes[0] += (byte)((color.G & 0x1C) << 3);
                    break;
                case PixelFormat.RGBA4:
                    
                    break;
                default:
                    bytes[0] = 0;
                    bytes[1] = 0;
                    break;
            }
        }

        /* //With no tiling
        private static void DirectDecode(int width, int height, Bitmap bmp, Stream fs, PixelFormat pixelFormat)
        {
            for (var y = 0; y < height; y ++)
                for (var x = 0; x < width; x ++)
                {
                    fs.Read(_tempBytes, 0, 2);
                    bmp.SetPixel(x, y, DecodeColor((_tempBytes[1] << 8) + _tempBytes[0], pixelFormat));
                }
        }
        */

        private static void DecodeTile(int iconSize, int tileSize, int ax, int ay, Bitmap bmp, Stream fs, PixelFormat pixelFormat)
        {
            if (tileSize == 0)
            {
                fs.Read(_tempBytes, 0, 2);
                bmp.SetPixel(ax, ay, DecodeColor((_tempBytes[1] << 8) + _tempBytes[0], pixelFormat));
            }
            else
                for (var y = 0; y < iconSize; y += tileSize)
                    for (var x = 0; x < iconSize; x += tileSize)
                        DecodeTile(tileSize, tileSize / 2, x + ax, y + ay, bmp, fs, pixelFormat);
        }

        private static void Encode(int iconSize, int tileSize, int ax, int ay, Bitmap bmp, Stream fs, PixelFormat pixelFormat)
        {
            if (tileSize == 0)
            {
                EncodeColor(bmp.GetPixel(ax, ay), pixelFormat, _tempBytes);
                fs.Write(_tempBytes, 0, PixelFormatBytes(pixelFormat));
            }
            else
                for (var y = 0; y < iconSize; y += tileSize)
                    for (var x = 0; x < iconSize; x += tileSize)
                        Encode(tileSize, tileSize / 2, x + ax, y + ay, bmp, fs, pixelFormat);
        }

        public static Bitmap ReadImageFromStream(Stream fs, int width, int height, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(width, height);
            for (var y = 0; y < height; y += 8)
                for (var x = 0; x < width; x += 8)
                    DecodeTile(8, 8, x, y, bmp, fs, pixelFormat);
            return bmp;
        }

        public static void WriteImageToStream(Image source, Stream fs, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(source);
            for (var y = 0; y < bmp.Height; y += 8)
                for (var x = 0; x < bmp.Width; x += 8)
                    Encode(8, 8, 0, 0, bmp, fs, pixelFormat);
        }
    }

}
