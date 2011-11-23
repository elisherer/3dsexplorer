using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace _3DSExplorer
{
    public static class Util
    {
        public static string byteArrayToString(byte[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length && i < 40; i++)
                arraystring += String.Format("{0:X2}", array[i]);
            if (i == 40) return arraystring + "..."; //ellipsis
            return arraystring;
        }

        public static string byteArrayToStringSpaces(byte[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length && i < 33; i++)
                arraystring += String.Format("{0:X2}", array[i]) + (i < array.Length - 1 ? " " : "");
            if (i == 33) return arraystring + "..."; //ellipsis
            return arraystring;
        }

        public static string charArrayToString(char[] array)
        {
            int i;
            string arraystring = "";
            for (i = 0; i < array.Length; i++)
            {
                if (array[i] == 0) break;
                arraystring += array[i];
            }
            return arraystring + "";
        }

        public static string toHexString(int digits, ulong number)
        {
            return "0x" + String.Format("{0:X" + digits + "}", number);
        }

        public static byte[] parseKeyStringToByteArray(string str)
        {
            if (str.Equals("")) return new byte[0];
            if ((str.Length % 2 > 0) || (str.Length != 32)) return null; //must be a mutliple of 2
            byte[] retArray = new byte[str.Length / 2];
            try
            {
                for (int i = 0; i < str.Length; i += 2)
                {
                    retArray[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't parse key string!\n" + ex.Message);
                return null;
            }
            return retArray;
        }
    }
}
