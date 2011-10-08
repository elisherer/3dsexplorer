using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DSExplorer
{
    public class lZ77
    {
        private int getle32(byte[] p,int offset)
        {
            return (p[0 + offset] << 0) | (p[1 + offset] << 8) | (p[2 + offset] << 16) | (p[3 + offset] << 24);
        } 

        public int GetDecompressedSize(byte[] compressed)
        {
            int footeroffset = compressed.Length - 8;

            //int buffertopandbottom = getle32(compressed,footeroffset);
            int originalbottom = getle32(compressed,footeroffset + 4);

            return originalbottom + compressed.Length;
        }

        /**
         * Returns true on success
         */
        public bool Decompress(byte[] compressed, byte[] decompressed)
        {
            int footeroffset = compressed.Length - 8;
            int buffertopandbottom = getle32(compressed, footeroffset);
            int originalbottom = getle32(compressed, footeroffset + 4);

            int i, j;
            int pout = decompressed.Length;
            int index = (buffertopandbottom & 0xFFFFFF) - ((buffertopandbottom >> 24) & 0xFF);
            int segmentoffset;
            int segmentsize;
	        byte control;
        	
	        while (pout != 0)
	        {
		        control = compressed[--index];

		        for(i = 0; i < 8; i++)
		        {
			        if (pout <= 0)
				        break;

			        if ((control & 0x80) != 0)
			        {
				        index -= 2;

				        segmentoffset = compressed[index] | (compressed[index+1]<<8);
				        segmentsize = ((segmentoffset >> 12)&15)+3;
				        segmentoffset &= 0x0FFF;
				        segmentoffset += 2;

				        if (pout < segmentsize) return false; //Error

				        for(j=0; j<segmentsize; j++)
				        {
					        byte data;

                            if (pout + segmentoffset >= decompressed.Length) return false; //Error

					        data  = decompressed[pout+segmentoffset];
					        decompressed[--pout] = data;
				        }
			        }
			        else
			        {
                        if (pout < 1) return false; //Error
				        decompressed[--pout] = compressed[--index];
			        }

			        control <<= 1;
		        }
	        }

            if (index != 0) return false; //Error
	        return true;
        }
    }
}
