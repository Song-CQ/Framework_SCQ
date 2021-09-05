using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Endogine.Serialization
{
    public class RleCodec
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="destinationBuffer"></param>
        /// <param name="bufferOffset"></param>
        /// <returns>the number of uncompressed bytes written to the destination buffer</returns>
        public static int DecodeChunk(Stream reader, byte[] destinationBuffer, int bufferOffset)
        {
            int numUncompressedWritten = 0;

            int len = (int)reader.ReadByte();

            if (len < 128)
            {
                //less than 128: uncompressed pixels
                int numUncompressedToRead = len + 1;
                numUncompressedWritten = numUncompressedToRead;

                while (numUncompressedToRead != 0)
                {
                    destinationBuffer[bufferOffset] = (byte)reader.ReadByte();
                    bufferOffset++;
                    numUncompressedToRead--;
                }
            }
            else if (len > 128)
            {
                //more than 128: RLE-compressed pixels
                int numCompressedToRead = (len ^ 0xff) + 2;

                numUncompressedWritten = numCompressedToRead;

                // Next -len+1 bytes in the dest are replicated from next source byte.
                // (Interpret len as a negative 8-bit int.)
                //									len ^= 0x0FF;
                //									len += 2;
                byte byteValue = (byte)reader.ReadByte();

                while (numCompressedToRead != 0)
                {
                    destinationBuffer[bufferOffset] = byteValue;
                    bufferOffset++;
                    numCompressedToRead--;
                }
            }
            else if (len == 128)
            {
                // Do nothing
            }

            return numUncompressedWritten;
        }
    }
}
