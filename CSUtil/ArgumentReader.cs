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
using Mono.Options;
using System.IO;

namespace CSUtil
{
    static class ArgumentReader
    {
        public static CSSettings Parse(string[] args)
        {
            CSSettings settings = new CSSettings();

            OptionSet s = new OptionSet()
            .Add("cs1", "Use CipherSaber-1 compliant settings (n=1)", cs1 => settings.KeyIterations = 1)
            .Add<int>("n|iterations=", "The amount of key iterations (default: 20)", n => settings.KeyIterations = n)
            .Add("d|decrypt", "Decrypt the input file", d => settings.Action = Action.Decrypt)
            .Add("e|encrypt", "Encrypt the input file", e => settings.Action = Action.Encrypt)
            .Add<string>("f|file=", "The location of the input file (default: stdin)", f => settings.InFile = f)
            .Add<string>("o|output=", "The location of the output file (default: stdout)", o => settings.OutFile = o)
            .Add("x|hex", "Use hexadecimal notation for input/output", x => settings.Hexadecimal = true)
            .Add("i|interactive", "Ask for the keyphrase when needed", i => settings.KeyMethod = KeyMethod.Interactive)
            .Add<string>("p|password=", "Use a given password. Caution! This is unsafe, use --interactive or a keyfile.", p =>
            {
                settings.KeyMethod = KeyMethod.Predefined;
                settings.Key = p;
            })
            .Add<string>("k|keyfile=", "Use a keyfile that contains the key.", k =>
            {
                settings.KeyMethod = KeyMethod.KeyFile;
                settings.KeyFilePath = k;
            })
            .Add("q|quiet", "Don't show warnings", q => settings.Quiet = true)
            .Add("h|?|help", "Display this help message", h => settings.Help = true);
            
            s.Parse(args);

            if (settings.Help)
            {
                Console.WriteLine("CSUtil  Copyright (C) 2012  Max Maton <max@maton.info>");
                Console.WriteLine();
                Console.WriteLine("This application encrypts and decrypts files using the CipherSaber encryption algorithm.");
                Console.WriteLine("For more information about CipherSaber: http://ciphersaber.gurus.org/faq.html");
                Console.WriteLine();
                Console.WriteLine("Command line options");
                s.WriteOptionDescriptions(Console.Out);
                Console.WriteLine();
                Console.WriteLine("Examples");
                Console.WriteLine("To encrypt a file using hex notation:");
                Console.WriteLine("CSUtil.exe -e -x");
                Console.WriteLine();
                Console.WriteLine("To decrypt a file encoded using hex notation:");
                Console.WriteLine("CSUtil.exe -d -x");
                Console.WriteLine();
                Console.WriteLine("Decrypt a file using cs1 encryption using a keyfile:");
                Console.WriteLine("CSUtil.exe -d --cs1 -f cknight.cs1 -o cknight.gif -k keyfile.txt");
                Console.WriteLine();
                Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY; This is free software, and you are welcome to redistribute it under certain conditions;");
                System.Environment.Exit(0);
            }

            return settings;
        }
    }
}
