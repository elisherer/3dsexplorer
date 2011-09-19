using System;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace _3DSExplorer
{
    class SaveTool
    {
        [DllImport("msvcrt.dll")]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        private struct HashEntry
        {
            public byte[] Hash;
            public int BlockIndex;
            public int Count;
        }

        public static Boolean isJournalMagic(byte[] buf, int offset)
        {
            return (buf[offset] == 0xE0 && buf[offset+1] == 0x6C && buf[offset+2] == 0x0D && buf[offset+3] == 0x08);
        }

        public static Boolean isDifiMagic(char[] buf)
        {
            return (buf[0] == 'D' && buf[1] == 'I' && buf[2] == 'F' && buf[3] == 'I');
        }

        public static Boolean isSaveMagic(char[] buf)
        {
            return (buf[0] == 'S' && buf[1] == 'A' && buf[2] == 'V' && buf[3] == 'E');
        }

        public static void XorByteArray(byte[] array, byte[] mask,int start)
        {
            for (int i = start; i < array.Length; i++)
                array[i] ^= mask[i % mask.Length];
        }

        public static byte[] FindKey(byte[] input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            int count = 0, rec_idx = 0, rec_count = 0;
            bool found;
            HashEntry[] hash_list;
            byte[] outbuf;

            byte[] hash;
            byte[] ff_hash = new byte[] { 0xde, 0x03, 0xfe, 0x65, 0xa6, 0x76, 0x5c, 0xaa, 0x8c, 0x91, 0x34, 0x3a, 0xcc, 0x62, 0xcf, 0xfc };

            hash_list = new HashEntry[4 * ((input.Length / 0x200) + 1)];

            for (int i = 0; i < (input.Length / 0x200); i++)
            {
                hash = md5.ComputeHash(input, i * 0x200, 0x200);

                if (memcmp(hash, ff_hash, 16) == 0) //skip ff blocks...
                    continue;

                found = false;
                // see if we already came up with that hash
                for (int j = 0; j < count; j++)
                    if (memcmp(hash_list[j].Hash, hash, 16) == 0)
                    {
                        hash_list[j].Count++;
                        found = true;
                        break;
                    }

                // push new hashlist entry
                if (!found)
                {
                    hash_list[count] = new HashEntry();
                    hash_list[count].Hash = new byte[hash.Length];
                    Buffer.BlockCopy(hash, 0, hash_list[count].Hash, 0, hash.Length);
                    hash_list[count].Count = 1;
                    hash_list[count].BlockIndex = i;
                    count++;
                }
            }
            // find the most common hash
            for (int i = 0; i < count; i++)
                if (hash_list[i].Count > rec_count)
                {
                    rec_count = hash_list[i].Count;
                    rec_idx = i;
                }

            if (rec_count == 0)
                return null;

            outbuf = new byte[0x200];
            Buffer.BlockCopy(input, hash_list[rec_idx].BlockIndex * 0x200, outbuf, 0, 0x200);

            return outbuf;
        } 
    }
}
