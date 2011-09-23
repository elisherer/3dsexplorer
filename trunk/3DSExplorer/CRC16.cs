using System;

namespace _3DSExplorer
{
    public class CRC16
    {
        private const ushort InitalValue = 0xFFFF;
        private const ushort Polynomial = 0x8005; // (1000 0000 0000 0101)
        private const ushort RevPolynomial = 0xA001; //reversed (1010 0000 0000 0001)

        public static byte[] GetCRC(byte[] message)
        {
            return GetCRC(message, 0, message.Length);
        }

        public static byte[] GetCRC(byte[] message, long offset, long length)
        {
            ushort CRCFull = InitalValue;
            char CRCLSB;

            for (long i = offset; i < offset + length; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);
                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ RevPolynomial);
                }
            }
            return BitConverter.GetBytes(CRCFull);
        }
    }
}