/*  
// LZ77Stream.cs by Atacama
// Decompresses LZ77 Streams

Example usage:
            string openFileName = "", saveFileName ="";
            OpenFileDialog oFDlg = new OpenFileDialog();

            oFDlg.Title = "Open LZ77 Compressed File...";
            oFDlg.Filter = "All files (*.*)|*.*";
            oFDlg.FilterIndex = 1;
            oFDlg.RestoreDirectory = true;
            if (oFDlg.ShowDialog() == DialogResult.OK)
            {
                openFileName = oFDlg.FileName;
            }
            else
            {
                return;

            }

            SaveFileDialog sFDlg = new SaveFileDialog();

            sFDlg.Title = "File to Save Compressed Data to...";
            sFDlg.Filter = "All files (*.*)|*.*";
            sFDlg.FilterIndex = 1;
            sFDlg.RestoreDirectory = true;
            if (sFDlg.ShowDialog() == DialogResult.OK)
            {
                saveFileName = sFDlg.FileName;
            }
            else
            {
                return;
            }
            MemoryStream lzdata = new MemoryStream();
            FileStream fIn = new FileStream(openFileName, FileMode.Open, FileAccess.Read);
            FileStream fOut = new FileStream(saveFileName, FileMode.CreateNew, FileAccess.Write);
            LZ77Stream lz = new LZ77Stream(lzdata, CompressionMode.Decompress);
            byte[] Indata = new byte[fIn.Length];
            fIn.Read(Indata, 0, (int) fIn.Length);
            lz.Write(Indata, 0, (int) fIn.Length);
            lzdata.WriteTo(fOut);
            fOut.Close();
            fIn.Close();
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace _3DSExplorer
{
    // TODO LZ77Stream for inline decompression of data
    class LZ77Stream : Stream
    {
        MemoryStream outMS;
        CompressionMode cmode;
        MemoryStream sIn;

        override public bool CanRead
        {
            get { return outMS.CanRead; }
        }
        override public bool CanSeek
        {
            get { return outMS.CanSeek; }
        }
        override public bool CanWrite
        {
            get { return outMS.CanWrite; }
        }
        override public long Length
        {
            get { return outMS.Length; }
        }
        override public long Position
        {
            get { return outMS.Position; }
            set { outMS.Position = value; }
        }
        override public void Flush()
        {
            outMS.Flush();
        }
        override public long Seek(long offset, System.IO.SeekOrigin loc)
        {

            return outMS.Seek(offset, loc);

        }

        override public void SetLength(long value)
        {
            outMS.SetLength(value);

        }
        override public int Read(byte[] buffer, int offset, int length)
        {
            return outMS.Read(buffer, offset, length);
        }
        override public void Write(byte[] buffer, int offset, int count)
        {
            if (cmode == CompressionMode.Decompress)
            {
                sIn = new MemoryStream(buffer, offset, count);
                Decompress();
            }
        }


        public LZ77Stream(Stream stream, CompressionMode mode)
        {
            outMS = (MemoryStream) stream;
            cmode = mode;


        }
        public void Decompress()
        {

            int uncompSize, compSize;
            byte Control;
            short compOffset;
            const int compSizeMin = 3;
            Buffer mbuffer = new Buffer();
            System.IO.BinaryReader bfio = new BinaryReader(this.sIn);
            System.IO.BinaryWriter bfout = new BinaryWriter(this.outMS);

            uncompSize = bfio.ReadInt32();
            uncompSize = uncompSize >> 8;


            while (this.sIn.Position < this.sIn.Length)
            {
                // Read a new control byte
                Control = bfio.ReadByte();
                for (int Index = 7; Index >= 0; Index--)
                {
                    if (mbuffer.Position == uncompSize)
                        break;
                    if (((Control >> Index) & 1) > 0)
                    {

                        compOffset = bfio.ReadInt16();

                        compSize = ((compOffset >> 4) & 0xf) + compSizeMin;
                        short compLeft = (short)((compOffset & 0x0f) << 8);
                        short compRight = (short)((compOffset & 0xff00) >> 8);
                        compOffset = (short)(compLeft | compRight);

                        for (; compSize > 0; compSize--)
                        {
                            mbuffer.Current = mbuffer.Read((mbuffer.Position % mbuffer.Size) - compOffset - 1);
                            mbuffer.Write(mbuffer.Current);
                            bfout.Write(mbuffer.Current);
                        }
                    }
                    else // Uncompressed data.
                    {
                        // Get the next uncompressed byte.
                        mbuffer.Current = bfio.ReadByte();

                        // Push it into the buffer.
                        mbuffer.Write(mbuffer.Current);

                        // Write the byte to the output.
                        bfout.Write(mbuffer.Current);
                    }
                }
            }
        }


    }
    class Buffer
    {
        public int Size = 0x80000;
        public int Start = 0,
                            End = 0;
        public byte Current;
        public int Position = 0;
        private byte[] bBuffer;

        public Buffer()        {
            bBuffer = new byte[Size];
        }

        public void Write(byte Value)
        {
            bBuffer[this.End] = Value;

            if (this.Start == this.End + 1)
                this.Start++;

            this.End++;

            if (this.End >= this.Size)
            {
                this.End = 0;
                this.Start = 1;
            }

            if (this.Start >= this.Size)
                this.Start = 0;

            this.Position++;
        }

        public byte Read(int Position)
        {
            return bBuffer[Position];
        }
    }

     
}