//
// AES128CTR.cs
//
// Copyright 2011 Eli Sherer
//

using System;
using System.Linq;
using System.Security.Cryptography;

namespace _3DSExplorer
{
	public class AES128CTR
    {
        private const int KEY_SIZE = 128;
        private const int BLOCK_SIZE = KEY_SIZE / 8;

		private byte[] key, iv;
        private AesManaged am; //Aes for the counterBlock

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key">Key byte array (should be 16 bytes long)</param>
        /// <param name="iv">Initialization Vector byte array (should be 16 bytes long)</param>
		///        
        public AES128CTR(byte[] key, byte[] iv)
        {
            this.key = new byte[BLOCK_SIZE];
            this.iv = new byte[BLOCK_SIZE];
            Buffer.BlockCopy(key, 0, this.key, 0, BLOCK_SIZE);
            Buffer.BlockCopy(iv, 0, this.iv, 0, BLOCK_SIZE);
            am = new AesManaged();
            am.KeySize = KEY_SIZE;
            am.Key = this.key;
            am.IV = new byte[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            am.Mode = CipherMode.ECB;
            am.Padding = PaddingMode.None;
		}

		/// <summary>
		/// Decrypt or Encrypt a block in AES-128 CTR Mode (changes the input array)
		/// </summary>
        /// <returns>The same array that was input</returns>
        public void TransformBlock(byte[] input)
        {
            ICryptoTransform ict = am.CreateEncryptor(); //reflective
            byte[] encryptedIV = new byte[BLOCK_SIZE];
            ulong counter = BitConverter.ToUInt64(iv.Reverse().ToArray(), 0); //get the nonce
            
            for (int offset = 0; offset < input.Length; offset += BLOCK_SIZE, counter++)
            {
                for (int i = 0; i < 8; i++) //Push the new counter to the end of iv
                    iv[i + BLOCK_SIZE - 8] = (byte)((counter >> ((7 - i) * 8)) & 0xff);
                ict.TransformBlock(iv, 0, BLOCK_SIZE, encryptedIV, 0); // ECB on counter
                // Xor it with the data
                for (int i = 0; i < BLOCK_SIZE && i + offset < input.Length; i++)
                    input[i + offset] ^= encryptedIV[i];
            }
		}
	}
}