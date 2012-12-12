using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherSaber;

namespace CSUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            Arcfour generator = new Arcfour(Encoding.ASCII.GetBytes("Key"));
            byte[] res = new byte[10];
            generator.Read(res, 0, 10);
            Console.WriteLine(BitConverter.ToString(res).Replace("-",""));

            Console.WriteLine("EB9F7781B734CA72A719");

            Console.ReadLine();
        }
    }
}
