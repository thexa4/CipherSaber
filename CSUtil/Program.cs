/*  CipherSaber
    Copyright (C) 2012  Max Maton

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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
                    if (!settings.Quiet)
                        Console.Error.WriteLine("Warning: Specifying the password as argument is considered unsafe. Use a keyfile instead.");
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
                    key = key.Substring(0, key.Length - 1);
                    Console.Error.WriteLine();
                    break;
            }

            if(settings.Action == Action.Encrypt && key.Length < 12 && !settings.Quiet)
                Console.Error.WriteLine("Warning: Password is unsafe, use at least 12 characters!");

            Stream input = null;
            Stream saber = null;
            Stream output = null;

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

            if (settings.OutFile == null || settings.OutFile == "-")
            {
                output = Console.OpenStandardOutput();
            }
            else
            {
                FileInfo f = new FileInfo(settings.OutFile);
                output = f.OpenWrite();
            }

            if (settings.Action == Action.Encrypt && settings.Hexadecimal)
                output = new HexStream(output);

            saber.CopyTo(output);
        }
    }
}
