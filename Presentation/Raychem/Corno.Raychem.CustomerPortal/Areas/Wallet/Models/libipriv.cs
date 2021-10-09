/*
   Copyright (C) 1998-2005 CyberPlat. All Rights Reserved.
   e-mail: support@cyberplat.com
*/

using System.Runtime.InteropServices;
using System.Text;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    public class PrivException : System.Exception
    {
        private readonly int _code;
        public PrivException(int c)
        {
            _code = c;
        }
        public override string ToString()
        {
            switch (_code)
            {
                case -1: return "Error in arguments";
                case -2: return "Memory allocation error";
                case -3: return "Invalid document format";
                case -4: return "The reading of the document has not been completed";
                case -5: return "Error in the document’s internal structure";
                case -6: return "Unknown encryption algorithm";
                case -7: return "The key length and the signature length do not match";
                case -8: return "Invalid code phrase to the secret key";
                case -9: return "Invalid document type";
                case -10: return "Error ASCII in encoding of the document";
                case -11: return "Error ASCII in decoding of the document";
                case -12: return "Unknown type of the encryption item";
                case -13: return "The encryption item is not ready";
                case -14: return "The call is not supported by the encryption item";
                case -15: return "Failed to find the file";
                case -16: return "File reading error";
                case -17: return "The key cannot be used";
                case -18: return "Error in signature creation";
                case -19: return "A public key with this serial number is not found";
                case -20: return "The signature and the document contents do not match";
                case -21: return "File creation error";
                case -22: return "Filing error";
                case -23: return "Invalid key card format";
                case -24: return "Keys generation error";
                case -25: return "Encryption error";
                case -26: return "Decryption error";
                case -27: return "The sender is not defined";
            }
            return "General error";
        }
        public int code => _code;
    }

    public class PrivKey
    {
        private readonly byte[] _pkey;

        public PrivKey()
        {
            _pkey = new byte[36];
        }
        public string SignText(string src)
        {
            return Priv.SignText(src, this);
        }
        public string VerifyText(string src)
        {
            return Priv.VerifyText(src, this);
        }
        public string EncryptText(string src)
        {
            return Priv.EncryptText(src, this);
        }
        public string DecryptText(string src)
        {
            return Priv.DecryptText(src, this);
        }
        public void CloseKey()
        {
            Priv.CloseKey(this);
        }

        public byte[] GetKey()
        {
            return _pkey;
        }
    };

    public class Priv
    {
        // for internal usage only
        [DllImport("libipriv.dll")]
        internal static extern int Crypt_Initialize();

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_Done();

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_OpenSecretKeyFromFile(int eng,
            [MarshalAs(UnmanagedType.LPStr)]string path,
            [MarshalAs(UnmanagedType.LPStr)]string passwd,
            [MarshalAs(UnmanagedType.LPArray)]byte[] pkey);

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_OpenPublicKeyFromFile(int eng,
            [MarshalAs(UnmanagedType.LPStr)]string path,
            uint keyserial,
            [MarshalAs(UnmanagedType.LPArray)]byte[] pkey,
            [MarshalAs(UnmanagedType.LPArray)]byte[] ñakey);

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_Sign([MarshalAs(UnmanagedType.LPStr)]string src,
            int nsrc, [MarshalAs(UnmanagedType.LPStr)]StringBuilder dst,
            int ndst,
            [MarshalAs(UnmanagedType.LPArray)]byte[] pkey);

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_Verify([MarshalAs(UnmanagedType.LPStr)]string src,
            int nsrc, [MarshalAs(UnmanagedType.LPArray)]byte[] pdst,
            [MarshalAs(UnmanagedType.LPArray)]byte[] pndst, [MarshalAs(UnmanagedType.LPArray)]byte[] pkey);

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_Encrypt([MarshalAs(UnmanagedType.LPStr)]string src,
            int nsrc, [MarshalAs(UnmanagedType.LPStr)]StringBuilder dst,
            int ndst,
            [MarshalAs(UnmanagedType.LPArray)]byte[] pkey);

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_Decrypt([MarshalAs(UnmanagedType.LPStr)]string src,
            int nsrc, [MarshalAs(UnmanagedType.LPStr)]StringBuilder dst,
            int ndst,
            [MarshalAs(UnmanagedType.LPArray)]byte[] pkey);

        [DllImport("libipriv.dll")]
        internal static extern int Crypt_CloseKey([MarshalAs(UnmanagedType.LPArray)]byte[] pkey);


        public static void Initialize()
        {
            Crypt_Initialize();
        }

        public static void Done()
        {
            Crypt_Done();
        }

        public static PrivKey OpenSecretKey(string path, string passwd)
        {
            var k = new PrivKey();
            var rc = Crypt_OpenSecretKeyFromFile(0, path, passwd, k.GetKey());
            if (rc != 0)
                throw new PrivException(rc);
            return k;
        }
        public static PrivKey OpenPublicKey(string path, uint keyserial)
        {
            var k = new PrivKey();
            var rc = Crypt_OpenPublicKeyFromFile(0, path, keyserial, k.GetKey(), null);
            if (rc != 0)
                throw new PrivException(rc);
            return k;
        }
        public static string SignText(string src, PrivKey key)
        {
            var tmp = new StringBuilder(40960);
            var rc = Crypt_Sign(src, src.Length, tmp, tmp.Capacity, key.GetKey());
            if (rc < 0)
                throw new PrivException(rc);
            var dst = tmp.ToString(0, rc);
            return dst;
        }
        public static string VerifyText(string src, PrivKey key)
        {
            var rc = Crypt_Verify(src, -1, null, null, key.GetKey());
            if (rc != 0)
                throw new PrivException(rc);
            return "";
        }
        public static string EncryptText(string src, PrivKey key)
        {
            var tmp = new StringBuilder(40960);
            var rc = Crypt_Encrypt(src, src.Length, tmp, tmp.Capacity, key.GetKey());
            if (rc < 0)
                throw new PrivException(rc);
            var dst = tmp.ToString(0, rc);
            return dst;
        }
        public static string DecryptText(string src, PrivKey key)
        {
            var tmp = new StringBuilder(40960);
            var rc = Crypt_Decrypt(src, src.Length, tmp, tmp.Capacity, key.GetKey());
            if (rc < 0)
                throw new PrivException(rc);
            var dst = tmp.ToString(0, rc);
            return dst;
        }
        public static void CloseKey(PrivKey key)
        {
            Crypt_CloseKey(key.GetKey());
        }
    }
}
