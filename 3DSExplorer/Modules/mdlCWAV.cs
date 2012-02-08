using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3DSExplorer.Modules
{
    /*
     * Available encoders:
     * 
     * 16-bit PCM encoder
     * 8-bit PCM encoder
     * DSP ADPCM encoder
     * IMA ADPCM encoder
     * 
     */

    //Uses DATABlobHeader from mdlBanner.cs

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CWAVINFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint InfoDataLength;
        public uint Type;
        public uint SamplesPerSec;
        public uint Unknown0;
        public uint Unknown1;
        public uint Unknown2;
        public uint Channels;
        public uint Unknown4;
        public uint Unknown5;
        public uint Unknown6;
        public uint Unknown7;
        public uint Unknown8;
        public uint Unknown9;
        public uint Unknown10;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
        public byte[] Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC)]
        public byte[] Unused;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CWAV
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;

        public ushort Endianess;
        public ushort StructLength;
        public uint Unknown0;
        public uint FileSize;
        public uint NumOfChunks;
        public uint InfoChunkFlags;
        public uint InfoChunkOffset;
        public uint InfoChunkLength;
        public uint DataChunkFlags;
        public uint DataChunkOffset;
        public uint DataChunkLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
        public byte[] Reserved;
    }

    public class CWAVContext : IContext
    {
        public enum CWAVView
        {
            CWAV
        };

        private string errorMessage = string.Empty;
        public CWAV Wave;
        public CWAVINFO InfoBlob;
        public DATABlobHeader DataBlob;
        public byte[] WaveData;
        public byte[] MicrosoftWaveData;
        
        public bool Open(Stream fs)
        {
            var WavStartPos = fs.Position;
            Wave = MarshalUtil.ReadStruct<CWAV>(fs);
            fs.Seek(WavStartPos + Wave.InfoChunkOffset, SeekOrigin.Begin);
            InfoBlob = MarshalUtil.ReadStruct<CWAVINFO>(fs);
            fs.Seek(WavStartPos + Wave.DataChunkOffset, SeekOrigin.Begin);
            DataBlob = MarshalUtil.ReadStruct<DATABlobHeader>(fs);
            //WaveData = new byte[DataBlob.Length - Marshal.SizeOf(DataBlob)];
            //fs.Read(WaveData, 0, WaveData.Length);
            /* BUG
            var wf = new WaveFormat
                        {
                            cbSize = 0,
                            nAvgBytesPerSec = (ushort)(InfoBlob.SamplesPerSec * (InfoBlob.NumOfChannles * (16 / 8))),
                            nBlockAlign = (ushort)(InfoBlob.NumOfChannles * (16 / 8)),
                            nChannels = (ushort)InfoBlob.NumOfChannles,
                            nSamplesPerSec = (ushort)InfoBlob.SamplesPerSec,
                            wBitsPerSample = 16,
                            wFormatTag = 1
                        };
            MicrosoftWaveData = WinMM.WriteWAVFile(wf, WaveData);*/
            return true;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }

        public void Create(FileStream fs, FileStream src)
        {
            throw new NotImplementedException();
        }

        public void View(frmExplorer f, int view, object[] values)
        {
            f.ClearInformation();
            switch ((CWAVView)view)
            {
                case CWAVView.CWAV:
                    f.SetGroupHeaders("CWAV","INFO","DATA");
                    f.AddListItem(0, 4, "Magic", Wave.Magic, 0);
                    f.AddListItem(4, 2, "Endianess", Wave.Endianess, 0);
                    f.AddListItem(6, 2, "Struct length", Wave.StructLength, 0);
                    f.AddListItem(8, 4, "Unknown0", Wave.Unknown0, 0);
                    f.AddListItem(0x0C, 4, "File Size", Wave.FileSize, 0);
                    f.AddListItem(0x10, 4, "Number of chunks", Wave.NumOfChunks, 0);
                    f.AddListItem(0x14, 4, "Info Chunk Flags", Wave.InfoChunkFlags, 0);
                    f.AddListItem(0x18, 4, "Info Chunk Offset", Wave.InfoChunkOffset, 0);
                    f.AddListItem(0x1C, 4, "Info Chunk Length", Wave.InfoChunkLength, 0);
                    f.AddListItem(0x20, 4, "Data Chunk Flags", Wave.DataChunkFlags, 0);
                    f.AddListItem(0x24, 4, "Data Chunk Offset", Wave.DataChunkOffset, 0);
                    f.AddListItem(0x28, 4, "Data Chunk Length", Wave.DataChunkLength, 0);
                    f.AddListItem(0x2C, 0x14, "Reserved", Wave.Reserved, 0);
                    
                    f.AddListItem(0, 4, "Magic", InfoBlob.Magic, 1);
                    f.AddListItem(4, 4, "Info Data Length", InfoBlob.InfoDataLength, 1);
                    f.AddListItem(8, 4, "Type", InfoBlob.Type, 1);
                    f.AddListItem(12, 4, "Samples per second", InfoBlob.SamplesPerSec, 1);
                    f.AddListItem(16, 4, "Unknown 0", InfoBlob.Unknown0, 1);
                    f.AddListItem(20, 4, "Unknown 1", InfoBlob.Unknown1, 1);
                    f.AddListItem(24, 4, "Unknown 2", InfoBlob.Unknown2, 1);
                    f.AddListItem(28, 4, "Channels", InfoBlob.Channels, 1);
                    f.AddListItem(32, 4, "Unknown 4", InfoBlob.Unknown4, 1);
                    f.AddListItem(36, 4, "Unknown 5", InfoBlob.Unknown5, 1);
                    f.AddListItem(40, 4, "Unknown 6", InfoBlob.Unknown6, 1);
                    f.AddListItem(44, 4, "Unknown 7", InfoBlob.Unknown7, 1);
                    f.AddListItem(48, 4, "Unknown 8", InfoBlob.Unknown8, 1);
                    f.AddListItem(52, 4, "Unknown 9", InfoBlob.Unknown9, 1);
                    f.AddListItem(56, 4, "Unknown 10", InfoBlob.Unknown10, 1);
                    f.AddListItem(60, 0x18, "Reserved", InfoBlob.Reserved, 1);
                    f.AddListItem(0x54, 0x0C, "Unused", InfoBlob.Unused, 1);

                    f.AddListItem(0, 4, "Magic", DataBlob.Magic, 2);
                    f.AddListItem(4, 4, "Length", DataBlob.Length, 2);
                    break;
            }
            f.AutoAlignColumns();
        }

        public bool CanCreate()
        {
            return false;
        }

        public void Activate(string filePath, int type, object[] values)
        {
            switch (type)
            {
                case 0:
                    MessageBox.Show("Experimental: still doesn't work!");
                    /* BUG
                    var ms = new MemoryStream(cxt.MicrosoftWaveData);
                    var sm = new SoundPlayer(ms);
                    sm.Play();*/
                    break;
            }
        }

        public string GetFileFilter()
        {
            return "CTR Waves (*.b/cwav)|*.bcwav;*.cwav";
        }

        public TreeNode GetExplorerTopNode()
        {
            var topNode = new TreeNode("CWAV") { Tag = TreeViewContextTag.Create(this, (int)CWAVView.CWAV) };

            return topNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var topNode = new TreeNode("CWAV", 1, 1);
            topNode.Nodes.Add(
                new TreeNode(TreeListView.TreeListViewControl.CreateMultiColumnNodeText("Wave.cwav",
                                                                                        WaveData.Length.ToString()))
                    {Tag = new[] {TreeViewContextTag.Create(this,0,"Play")}});
            return topNode;
        }
    }

}