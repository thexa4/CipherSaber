using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace CipherSaber
{
    public class SaberEncrypt : Stream
    {
        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override bool CanSeek { get { return false; } }

        public override long Length { get { return baseStream.Length - 10; } }
        public override long Position
        {
            get { return position; }
            set { throw new NotImplementedException(); }
        }

        byte[] iv = new byte[10];
        protected Arcfour generator;
        protected Stream baseStream;
        protected long position = 0;

        public SaberEncrypt(Stream stream, string key, int n)
        {
            baseStream = stream;
            Initialize(key, n);
        }

        protected void Initialize(string key, int n)
        {
            RandomNumberGenerator.Create().GetBytes(iv);
            generator = new Arcfour(Encoding.Default.GetBytes(key).Concat(iv).ToArray(), n);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (position < iv.Length)
            {
                int len = (int)Math.Min(iv.Length - position, count);
                for (int i = 0; i < len; i++)
                    buffer[offset + i] = iv[position + i];
                position += len;
                return len;
            }
            else
            {
                byte[] cipherText = new byte[count];
                int len = baseStream.Read(cipherText, 0, count);

                byte[] keyChars = new byte[len];
                generator.Read(keyChars, 0, len);

                for (int i = 0; i < len; i++)
                    buffer[offset + i] = (byte)(cipherText[i] ^ keyChars[i]);

                position += len;

                return len;
            }
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
