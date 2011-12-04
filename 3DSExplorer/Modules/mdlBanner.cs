using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using _3DSExplorer.Utils;

namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CBMD
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Padding0;
        public uint CompressedCGFXOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x78)]
        public byte[] Padding1;
        public uint CWAVOffset;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CWAV
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3C)]
        public byte[] Padding0;
        public INFOBlob InfoBlob;
        public DATABlobHeader DataBlob;
        /*
         * 		
            @0x0CA0    starts with 0x23 (size = 0x8E88)
		    @0x9B40   starts with 0x63 (size = 0x8E88)
		    maybe it's stereo and these are the left & right channels data
         * 
         */
    }

    public struct INFOBlob
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xBC)]
        public byte[] InfoData;        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DATABlobHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint Length;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CGFX
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public ushort ImageLength;
        public ushort DataOffset;
        public uint Unknown0;
        public uint FileSize;
        public uint Unknown1;
        public DATABlobHeader DataBlob;
        /*
         * 
            0x094 DICT (the first one of many) { uint32 length; and data[length-8]}

		    0x0F0 CMDL

			    0x83C SOBJ (first one of several)
			    0x8BC MTOB (MaterialsObject?)
			    0xBE0 TXOB (TextureObject??)
			    0xC20 SHDR (Shader?)
	
		    0x1068 COMMON
         * 
         */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CGFXIMAG
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public ushort Length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x82)]
        public byte[] Unknown;
    }

    public class BannerContext : IContext
    {
        public enum BannerView
        {
            Banner,
            CGFX,
            CWAV
        };

        public CBMD Header;
        public byte[] DecompressedCGFX;
        public CGFX Graphics;
        public byte[] GraphicsData;
        public CGFXIMAG GraphicsImage;
        public Bitmap BannerImage;

        public CWAV Wave;
        public byte[] WaveData;

        

        public bool Open(FileStream fs)
        {
            Header = MarshalUtil.ReadStruct<CBMD>(fs); //read header

            //-- Graphics Reading --

            //Read ahead the size of the uncompressed file
            fs.Seek(Header.CompressedCGFXOffset + 1, SeekOrigin.Begin);
            var intBytes = new byte[4];
            fs.Read(intBytes, 0, 4);
            DecompressedCGFX = new byte[BitConverter.ToUInt32(intBytes, 0)];
            //Read again from the start
            fs.Seek(Header.CompressedCGFXOffset, SeekOrigin.Begin);
            var ms = new MemoryStream(DecompressedCGFX);
            try
            {
                var lz11 = new DSDecmp.Formats.Nitro.LZ11();
                lz11.Decompress(fs, Header.CWAVOffset - fs.Position, ms);
            }
            catch
            { //might throw exception if size of compressed is bigger than it should be
            }
            ms.Seek(0, SeekOrigin.Begin);
            Graphics = MarshalUtil.ReadStruct<CGFX>(ms);
            GraphicsData = new byte[Graphics.DataBlob.Length - Marshal.SizeOf(Graphics.DataBlob)];
            ms.Read(GraphicsData, 0, GraphicsData.Length);
            var imagPos = ms.Position;
            GraphicsImage = MarshalUtil.ReadStruct<CGFXIMAG>(ms);
            ms.Seek(imagPos + GraphicsImage.Length, SeekOrigin.Begin);
            
            BannerImage = ImageUtil.ReadImageFromStream(ms, 256, 128, ImageUtil.PixelFormat.RGBA4);

            //-- Wave reading --
            fs.Seek(Header.CWAVOffset, SeekOrigin.Begin);
            Wave = MarshalUtil.ReadStruct<CWAV>(fs);
            WaveData = new byte[Wave.DataBlob.Length - Marshal.SizeOf(Graphics.DataBlob)];
            fs.Read(WaveData, 0, WaveData.Length);
            return true;
        }

        public void Create(FileStream fs, FileStream src)
        {
            throw new NotImplementedException();
        }

        public void View(frmExplorer f, int view, int[] values)
        {
            f.ClearInformation();
            switch ((BannerView)view)
            {
                case BannerView.Banner:
                    var bmd = Header;
                    f.SetGroupHeaders("CBMD");
                    f.AddListItem(0, 4, "Magic", bmd.Magic, 0);
                    f.AddListItem(4, 4, "Padding 0", bmd.Padding0, 0);
                    f.AddListItem(8, 4, "Compressed CGFX Offset", bmd.CompressedCGFXOffset, 0);
                    f.AddListItem(0x10, 0x78, "Padding 1", bmd.Padding1, 0);
                    f.AddListItem(0x84, 4, "CWAV Offset", bmd.CWAVOffset, 0);
                    break;
                case BannerView.CGFX:
                    f.SetGroupHeaders("CGFX","DATA");
                    f.AddListItem(0x00, 4, "Magic", Graphics.Magic, 0);
                    f.AddListItem(0x04, 2, "Image Length", Graphics.ImageLength, 0);
                    f.AddListItem(0x06, 2, "DATA Offset", Graphics.DataOffset, 0);                            
                    f.AddListItem(0x08, 4, "Unknown 0", Graphics.Unknown0, 0);
                    f.AddListItem(0x0C, 4, "File Size", Graphics.FileSize, 0);
                    f.AddListItem(0x10, 4, "Unknown 1", Graphics.Unknown1, 0);
                    f.AddListItem(0x14, 4, "Magic", Graphics.DataBlob.Magic, 1);
                    f.AddListItem(0x18, 4, "Length", Graphics.DataBlob.Length, 1);
                    break;
                case BannerView.CWAV:
                    f.SetGroupHeaders("CWAV","INFO","DATA");
                    f.AddListItem(0, 4, "Magic", Wave.Magic, 0);
                    f.AddListItem(0x04, 4, "Padding0", Wave.Padding0, 0);
                    
                    f.AddListItem(0, 4, "Magic", Wave.InfoBlob.Magic, 1);
                    f.AddListItem(4, 4, "Info Data", Wave.InfoBlob.InfoData, 1);

                    f.AddListItem(0, 4, "Magic", Wave.DataBlob.Magic, 2);
                    f.AddListItem(4, 4, "Length", Wave.DataBlob.Length, 2);
                    break;
            }
            f.AutoAlignColumns();
        }
    }

}