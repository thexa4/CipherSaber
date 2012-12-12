using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CipherSaber
{
    public class HexStream : Stream
    {
        public override long Position
        {
            get { return Position; }
            set { throw new NotImplementedException(); }
        }

        public override long Length { get { return long.MaxValue; } }
        public override bool CanWrite { get { return baseStream.CanWrite; } }
        public override bool CanRead { get { return baseStream.CanRead; } }
        public override bool CanSeek { get { return false; } }

        protected Stream baseStream;
        protected long position = 0;

        protected byte? prefetched = null;

        public HexStream(Stream stream)
        {
            baseStream = stream;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string hexString = BitConverter.ToString(buffer, offset, count).Replace("-", " ") + " ";
            byte[] outbuffer = Encoding.ASCII.GetBytes(hexString);

            baseStream.Write(outbuffer, 0, outbuffer.Length);

            position += outbuffer.Length;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            byte[] inbuffer = new byte[count * 3];
            int inpos = 0;

            if (prefetched != null)
            {
                inbuffer[0] = prefetched.Value;
                prefetched = null;
                inpos++;
            }

            inpos += baseStream.Read(inbuffer, inpos, inbuffer.Length - inpos);
            
            byte[] hexChars = new byte[]{
                48, 49, 50,  51,  52,  53, 54, 55, 56, 57, //Decimals
                65, 66, 67,  68,  69,  70, //Uppercase
                97, 98, 99, 100, 101, 102, //Lowercase
            };

            byte[] filtered = new byte[inpos];
            int i = 0;
            for (int j = 0; j < inpos; j++)
                if (hexChars.Contains(inbuffer[j]))
                    filtered[i++] = inbuffer[j];

            if (i % 2 != 0)
                prefetched = filtered[--i];

            string ascii = Encoding.ASCII.GetString(filtered);
            for (int j = 0; j < ascii.Length / 2; j++)
                buffer[offset + j] = Byte.Parse(ascii.Substring(j * 2, 2), System.Globalization.NumberStyles.HexNumber);

            position += ascii.Length / 2;
            return ascii.Length / 2;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
        }

    }
}
