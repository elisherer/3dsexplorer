using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace _3DSExplorer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct INFOBlobHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
        public char[] Magic;
        public uint InfoDataLength;
        public uint NumOfChannles;
        public ulong SamplesPerSec;
        public ulong Unknown0;
        public uint NumOf0x48Blocks;
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
        public INFOBlobHeader InfoBlob;
        public byte[][] InfoBlocks;
        public DATABlobHeader DataBlob;
        public byte[] WaveData;
        public byte[] MicrosoftWaveData;
        
        public bool Open(Stream fs)
        {
            Wave = MarshalUtil.ReadStruct<CWAV>(fs);
            fs.Seek(Wave.InfoChunkOffset, SeekOrigin.Begin);
            InfoBlob = MarshalUtil.ReadStruct<INFOBlobHeader>(fs);
            InfoBlocks = new byte[InfoBlob.NumOf0x48Blocks][];
            for (var i = 0; i < InfoBlob.NumOf0x48Blocks; i++)
            {
                InfoBlocks[i] = new byte[0x48];
                fs.Read(InfoBlocks[i], 0, InfoBlocks[i].Length);
            }
            fs.Seek(Wave.DataChunkOffset, SeekOrigin.Begin);
            DataBlob = MarshalUtil.ReadStruct<DATABlobHeader>(fs);
            WaveData = new byte[DataBlob.Length - Marshal.SizeOf(DataBlob)];
            fs.Read(WaveData, 0, WaveData.Length);

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
            //BUG MicrosoftWaveData = WinMM.WriteWAVFile(wf, WaveData);
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

        public void View(frmExplorer f, int view, int[] values)
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
                    f.AddListItem(8, 4, "Number of channles", InfoBlob.NumOfChannles, 1);
                    f.AddListItem(0x0C, 8, "Samples per second", InfoBlob.SamplesPerSec, 1);
                    f.AddListItem(0x14, 8, "Unknown 0", InfoBlob.Unknown0, 1);
                    f.AddListItem(0x1C, 4, "Number of 0x48 blocks", InfoBlob.NumOf0x48Blocks, 1);
                    for (var i = 0; i < InfoBlob.NumOf0x48Blocks;i++ )
                        f.AddListItem(0, 0x48, "Block " + i, InfoBlocks[i], 1);

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

        public TreeNode GetExplorerTopNode()
        {
            var topNode = new TreeNode("CWAV") { Tag = TreeViewContextTag.Create(this, (int)CWAVView.CWAV) };

            return topNode;
        }

        public TreeNode GetFileSystemTopNode()
        {
            var topNode = new TreeNode("CWAV", 1, 1);
            topNode.Nodes.Add(new TreeNode(TreeListView.TreeListViewControl.CreateMultiColumnNodeText("Wave.cwav",WaveData.Length.ToString())) { Tag = this });
            return topNode;
        }

        public static void Play(CWAVContext cxt)
        {
            MessageBox.Show("Experimental: still doesn't work!");
            /* BUG
            var ms = new MemoryStream(cxt.MicrosoftWaveData);
            var sm = new SoundPlayer(ms);
            sm.Play();*/
        }
    }

}