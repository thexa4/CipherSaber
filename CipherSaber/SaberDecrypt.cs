using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CipherSaber
{
    class SaberDecrypt : Stream
    {
        public override bool CanRead { get { return true; }}
        public override bool CanWrite { get { return false; }}
        public override bool CanSeek { get { return false; }}

        public override long Length { get { return baseStream.Length - 10; } }
        public override long Position
        {
            get { return generator.Position; }
            set { throw new NotImplementedException(); }
        }

        protected Arcfour generator;
        protected Stream baseStream;

        public SaberDecrypt(Stream stream, string key, int n)
        {
            baseStream = stream;
            Initialize(key, n);
        }

        public void Initialize(string key, int n)
        {
            byte[] iv = new byte[10];
            
            int pos = 0;
            while (pos != 10)
            {
                int len = baseStream.Read(iv, pos, 10 - pos);
                if (len == 0)
                    throw new IOException("Stream closed during initalization");

                pos += len;
            }

            generator = new Arcfour(Encoding.Default.GetBytes(key).Concat(iv).ToArray(), n);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            byte[] cyphertext = new byte[count];
            int len = baseStream.Read(cyphertext, 0, count);

            byte[] keytext = new byte[len];
            generator.Read(keytext, 0, len);

            for (int i = 0; i < len; i++)
                buffer[offset + i] = (byte)(cyphertext[i] ^ keytext[i]);

            return len;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
