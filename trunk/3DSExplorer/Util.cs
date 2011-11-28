using System;
using System.Windows.Forms;

namespace _3DSExplorer
{
    public static class Util
    {
        public static string ByteArrayToString(byte[] array)
        {
            int i;
            var arraystring = string.Empty;
            for (i = 0; i < array.Length && i < 40; i++)
                arraystring += String.Format("{0:X2}", array[i]);
            if (i == 40) return arraystring + "..."; //ellipsis
            return arraystring;
        }
        
        /*
        public static string ByteArrayToStringSpaces(byte[] array)
        {
            int i;
            var arraystring = string.Empty;
            for (i = 0; i < array.Length && i < 33; i++)
                arraystring += String.Format("{0:X2}", array[i]) + (i < array.Length - 1 ? " " : "");
            if (i == 33) return arraystring + "..."; //ellipsis
            return arraystring;
        }*/

        public static string CharArrayToString(char[] array)
        {
            int i;
            var arraystring = string.Empty;
            for (i = 0; i < array.Length; i++)
            {
                if (array[i] == 0) break;
                arraystring += array[i];
            }
            return arraystring + "";
        }

        public static string ToHexString(int digits, ulong number)
        {
            var formatString = "{0:X" + digits + "}";
            return "0x" + String.Format(formatString, number);
        }

        public static byte[] ParseKeyStringToByteArray(string str)
        {
            if (str.Equals("")) return new byte[0];
            if ((str.Length % 2 > 0) || (str.Length != 32)) return null; //must be a mutliple of 2
            var retArray = new byte[str.Length / 2];
            try
            {
                for (var i = 0; i < str.Length; i += 2)
                {
                    retArray[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
                }
            }
            catch (Exception ex)
            {
// ReSharper disable LocalizableElement
                MessageBox.Show("Can't parse key string!\n" + ex.Message);
// ReSharper restore LocalizableElement
                return null;
            }
            return retArray;
        }
    }
}
