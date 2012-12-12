using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CipherSaber;
using System.IO;

namespace CSUtil
{
    class Program
    {

        static void Main(string[] args)
        {
            CSSettings settings = ArgumentReader.Parse(args);

            string key = "";
            switch (settings.KeyMethod)
            {
                case KeyMethod.Predefined:
                    key = settings.Key;
                    break;
                case KeyMethod.KeyFile:
                    FileInfo f = new FileInfo(settings.KeyFilePath);
                    if (!f.Exists)
                    {
                        Console.Error.WriteLine("Error: Keyfile does not exist");
                        return;
                    }
                    StreamReader r = new StreamReader(f.OpenRead());
                    key = r.ReadToEnd();
                    break;
                case KeyMethod.Interactive:
                    Console.Error.Write("Key: ");
                    ConsoleKeyInfo k;
                    
                    do
                    {
                        k = Console.ReadKey(true);

                        // Backspace Should Not Work
                        if (k.Key != ConsoleKey.Backspace)
                        {
                            key += k.KeyChar;
                        }
                    }
                    while (k.Key != ConsoleKey.Enter);

                    Console.Error.WriteLine();
                    break;
            }

            if(settings.Action == Action.Encrypt && key.Length < 12 && !settings.Quiet)
                Console.Error.WriteLine("Warning: Password is unsafe, use at least 12 characters!");

            Stream input = null;
            Stream saber = null;
            Stream output = null;
            Stream hex = null;

            if (settings.InFile == null || settings.InFile == "-")
            {
                input = Console.OpenStandardInput();
            }
            else
            {
                FileInfo f = new FileInfo(settings.InFile);
                if (!f.Exists)
                {
                    Console.Error.WriteLine("Error: Input file does not exist");
                    return;
                }

                input = f.OpenRead();
            }

            if (settings.Action == Action.Decrypt && settings.Hexadecimal)
                input = new HexStream(input);

            if (settings.Action == Action.Encrypt)
            {
                saber = new SaberEncrypt(input, key, settings.KeyIterations);
            }
            else
            {
                saber = new SaberDecrypt(input, key, settings.KeyIterations);
            }

            if (settings.Action == Action.Encrypt && settings.Hexadecimal)
                saber = new HexStream(saber);


            if (settings.OutFile == null || settings.OutFile == "-")
            {
                output = Console.OpenStandardOutput();
            }
            else
            {
                FileInfo f = new FileInfo(settings.OutFile);
                output = f.OpenWrite();
            }

            saber.CopyTo(output);
        }
    }
}
