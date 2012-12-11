using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CipherSaber
{
    /// <summary>
    /// Arcfour stream cipher algorithm
    /// http://www.mozilla.org/projects/security/pki/nss/draft-kaukonen-cipher-arcfour-03.txt
    /// </summary>
    public sealed class Arcfour : Stream
    {
        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }

        public override long Length { get { return long.MaxValue; } }
        public override long  Position
        {
            get { return position; }
            set { throw new NotImplementedException(); }
        }

        private long position = 0;
        private int i = 0;
        private int j = 0;
        private byte temp;
        private byte[] sbox = new byte[256];

        /// <summary>
        /// Creates a standard Arcfour stream
        /// </summary>
        /// <param name="key">The key to initialize with</param>
        public Arcfour(byte[] key)
            : this(key, 1)
        {
        }

        /// <summary>
        /// Creates a new arcfour stream
        /// </summary>
        /// <param name="key">The key to initialize the stream with</param>
        /// <param name="n">The amount of initalization rounds</param>
        public Arcfour(byte[] key, int n)
        {
            // Initialize the S-Box
            for (i = 0; i < 256; i++)
                sbox[i] = (byte)i;

            byte[] sbox2 = new byte[256];
            for (i = 0; i < 256; i++)
                sbox2[i] = key[i % key.Length];

            for(int k = 0; k < n; k++)
                for (i = 0; i < 256; i++)
                {
                    j = (j + sbox[i] + sbox2[i]) % 256;
                    temp = sbox[i];
                    sbox[i] = sbox[j];
                    sbox[j] = temp;
                }

            // Try to reset memory to prevent memory sniffing
            for (i = 0; i < 256; i++)
                sbox2[i] = 0;

            temp = 0;
            j = 0;
            i = 0;
        }

        /// <summary>
        /// Read a number of bytes from the key stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int p = offset; p < offset + count; p++)
            {
                i = (i + 1) % 256;
                j = (j + sbox[i]) % 256;
                temp = sbox[i];
                sbox[i] = sbox[j];
                sbox[j] = temp;
                temp = (byte)((sbox[i] + sbox[j]) % 256);
                buffer[p] = sbox[temp];
                temp = 0;
            }

            position += count;
            return count;
        }

        public override void Flush()
        {
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
