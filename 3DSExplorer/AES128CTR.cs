//
// AES128CTR.cs
//
// Copyright 2011 Eli Sherer
//

/* -= From (http://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged.aesmanaged(v=vs.90).aspx) =-
 * The AesManaged class supports a CipherMode to set its mode.
 * But what about Counter Mode, sometimes called CTR mode?
 * There is no CTR mode supported in the CipherMode enum. 
 *       
 * CTR mode is a little different than the other CipherModes because
 * in fact the plainText is not directly encrypted to obtain the cipherText.
 * Instead, a counter is encrypted, and the result of that is XOR'd with
 * the plainText to get the cipherText. 
 * 
 * It is possible to implement AES crypto in CTR mode using the AesManaged class,
 * though it requires some extra work.
 * 
 * To implement CTR mode with the .NET AesManaged class, here's what I did: 
 *
 * Use CipherMode.ECB, PaddingMode.None.
 * [No padding is necessary in CTR mode because we are always encrypting a 16-byte counter.]
 * 
 * When encrypting, call CreateEncryptor().
 * Using the resulting ICryptoTransform, for each block,
 * transform a nonce of 16-bytes (Same as the AES block size),
 * and then XOR the result of that transform with the plaintext to get the ciphertext.
 * Increment the nonce after each block.
 * Continue until there are no more blocks 
 * - don't forget the final block transform, for the last block of 16 or fewer bytes. 
 *
 * When decrypting, because the plaintext is recovered via an XOR,
 * it works exactly the same way;
 * again, you must call CreateEncryptor().
 * The idea is that with each successive block,
 * the decryptor will XOR the cipherText with exactly the same "encrypted nonce" as was used during encryption.
 * In fact when using AesManaged in CTR mode during decryption,
 * we are not really "Decrypting" the ciphertext directly;
 * we are encrypting a nonce and then XOR'ing the cipherText with the result, thus recovering the plaintext.
 */

using System;
using System.Security.Cryptography;

namespace _3DSExplorer
{

	public class AES128CTR
    {

        public static readonly byte[] KEY_WII_COMMON = { 0xeb, 0xe4, 0x2a, 0x22, 0x5e, 0x85, 0x93, 0xe4, 0x48, 0xd9, 0xc5, 0x45, 0x73, 0x81, 0xaa, 0xf7 };

        public static readonly byte[] KEY_WII_SD = { 0xab, 0x01, 0xb9, 0xd8, 0xe1, 0x62, 0x2b, 0x08, 0xaf, 0xba, 0xd8, 0x4d, 0xbf, 0xc2, 0xa5, 0x5d };
        public static readonly byte[] IV_WII_SD = { 0x21, 0x67, 0x12, 0xe6, 0xaa, 0x1f, 0x68, 0x9f, 0x95, 0xc5, 0xa2, 0x23, 0x24, 0xdc, 0x6a, 0x98 };

        

        private const int KEY_SIZE = 128;
        private const int BLOCK_SIZE = KEY_SIZE / 8;

		private byte[] key, iv;
        private AesManaged am; //Aes for the counterBlock

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key">key byte array (should be 16 bytes long)</param>
		///        
        public AES128CTR(byte[] key)
        {
            iv = new byte[BLOCK_SIZE];
            for (int i = 0; i < BLOCK_SIZE ;i++)
                iv[i] = 0;
            this.key = key;
            am = new AesManaged();
            am.KeySize = KEY_SIZE;
            am.Key = key;
            am.IV = iv;
            am.Mode = CipherMode.ECB;
            am.Padding = PaddingMode.None;
		}

        private void XorBlock(byte[] dst, int offset, byte[] ctrBlock)
        {
            for (int i=0;i<key.Length;i++)
                dst[i + offset] ^= ctrBlock[i];
        }

		/// <summary>
		/// Decrypt a block in AES-128 CTR Mode (changes the input array)
		/// </summary>
        /// <returns>The same array that was input</returns>
        public byte[] Decrypt(byte[] input)
        {
            ICryptoTransform ict = am.CreateEncryptor(); //encrypting instead of decrypting!
            //decryption process

            byte[] ctrBlock = new byte[BLOCK_SIZE];
            byte[] encCtrBlock = new byte[BLOCK_SIZE];

            Buffer.BlockCopy(input, 0, ctrBlock, 0, 8); //Nonce suppose to be at the first 8 bytes
            ctrBlock[8] = 0; ctrBlock[9] = 0; ctrBlock[10] = 0; ctrBlock[11] = 0;

            for (var b=0; b<input.Length; b+=BLOCK_SIZE)
            {
                // set counter (block #) in last 8 bytes of counter block (leaving nonce in 1st 8 bytes)
                // TODO: Might need to change counter to 64bit counter...
                Buffer.BlockCopy(BitConverter.GetBytes(b), 0, ctrBlock, BLOCK_SIZE - 4, 4);
                // encrypt counter block
                ict.TransformBlock(ctrBlock, 0, BLOCK_SIZE, encCtrBlock, 0);  
                // Xor it with the data
                XorBlock(input, b, encCtrBlock);
            }
            return input;
		}

		/// <summary>
		/// Encrypt a block in AES-128 CTR Mode
		/// </summary>
		public byte[] Encrypt(byte[] input) 
        {
            //Does nothing
            return input;
        }

	}
}