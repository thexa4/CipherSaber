using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSUtil
{
    class CSSettings
    {
        public CSSettings()
        {
            KeyIterations = 20;
        }

        /// <summary>
        /// The amount of key initializations performed prior to encryption/decryption
        /// Should be at least 20 according to the standard implementation (http://ciphersaber.gurus.org/faq.html#cs2)
        /// </summary>
        public int KeyIterations { get; set; }

        /// <summary>
        /// Wether the input/output should be hexadecimal
        /// </summary>
        public bool Hexadecimal { get; set; }

        /// <summary>
        /// How the application should aquire the encryption key
        /// </summary>
        public KeyMethod KeyMethod { get; set; }

        /// <summary>
        /// Keyfile location
        /// </summary>
        public string KeyFilePath { get; set; }

        /// <summary>
        /// The predefined key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The action to use
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// The file to use as input (null for stdin)
        /// </summary>
        public string InFile { get; set; }

        /// <summary>
        /// The file to use as output (null for stdin)
        /// </summary>
        public string OutFile { get; set; }

        /// <summary>
        /// Wether informative output should be suppressed
        /// </summary>
        public bool Quiet { get; set; }

        /// <summary>
        /// Wether the help message should be shown
        /// </summary>
        public bool Help { get; set; }
    }

    public enum KeyMethod
    {
        Interactive = 0,
        KeyFile = 1,
        Predefined = 2,
    }

    public enum Action
    {
        Encrypt = 0,
        Decrypt = 1,
    }
}
