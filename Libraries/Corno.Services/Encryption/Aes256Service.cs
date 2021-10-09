using System;
using System.Security.Cryptography;
using System.Text;
using Corno.Services.Encryption.Interfaces;

namespace Corno.Services.Encryption
{
    /*****************************************************************
	 * CrossPlatform CryptLib
	 * 
	 * <p>
	 * This cross platform CryptLib uses AES 256 for encryption. This library can
	 * be used for encryptoion and de-cryption of string on iOS, Android and Windows
	 * platform.<br/>
	 * Features: <br/>
	 * 1. 256 bit AES encryption
	 * 2. Random IV generation. 
	 * 3. Provision for SHA256 hashing of key. 
	 * </p>
	 * 
	 * @since 1.0
	 * @author navneet
	 *****************************************************************/
    public class Aes256Service : IAes256Service
    {
        private readonly UTF8Encoding _enc;
        private readonly RijndaelManaged _rcipher;
        private readonly byte[] _key;
        private byte[] _pwd, _ivBytes;
        private readonly byte[] _iv;

        /***
		 * Encryption mode enumeration
		 */
        private enum EncryptMode { Encrypt, Decrypt };

        static readonly char[] CharacterMatrixForRandomIvStringGeneration = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
        };

        /**
		 * This function generates random string of the given input length.
		 * 
		 * @param _plainText
		 *            Plain text to be encrypted
		 * @param _key
		 *            Encryption Key. You'll have to use the same key for decryption
		 * @return returns encrypted (cipher) text
		 */
        public string GenerateRandomIv(int length) {
            var iv = new char[length];
            var randomBytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(randomBytes);
            }

            for (var i = 0; i < iv.Length; i++) {
                var ptr = randomBytes[i] % CharacterMatrixForRandomIvStringGeneration.Length;
                iv[i] = CharacterMatrixForRandomIvStringGeneration[ptr];
            }

            return new string(iv);
        }

		public Aes256Service()
		{
			_enc = new UTF8Encoding();
		    _rcipher = new RijndaelManaged
		    {
		        Mode = CipherMode.CBC,
		        Padding = PaddingMode.PKCS7,
		        KeySize = 256,
		        BlockSize = 128
		    };
		    _key = new byte[32];
			_iv = new byte[_rcipher.BlockSize/8]; //128 bit / 8 = 16 bytes
			_ivBytes =  new byte[16];
		}

        
		/**
		 * 
		 * @param _inputText
		 *            Text to be encrypted or decrypted
		 * @param _encryptionKey
		 *            Encryption key to used for encryption / decryption
		 * @param _mode
		 *            specify the mode encryption / decryption
		 * @param _initVector
		 * 			  initialization vector
		 * @return encrypted or decrypted string based on the mode
	 	*/
		private string EncryptDecrypt (string inputText, string encryptionKey, EncryptMode mode, string initVector)
		{

			var _out = "";// output string
			//_encryptionKey = MD5Hash (_encryptionKey);
			_pwd = Encoding.UTF8.GetBytes(encryptionKey);
			_ivBytes = Encoding.UTF8.GetBytes (initVector);

			var len = _pwd.Length;
			if (len > _key.Length)
			{
				len = _key.Length;
			}
			var ivLenth = _ivBytes.Length;
			if (ivLenth > _iv.Length) 
			{
				ivLenth = _iv.Length;		
			}

			Array.Copy(_pwd, _key, len);
			Array.Copy (_ivBytes, _iv, ivLenth);
			_rcipher.Key = _key;
			_rcipher.IV = _iv;

			if (mode.Equals (EncryptMode.Encrypt)) {
				//encrypt
				var plainText = _rcipher.CreateEncryptor().TransformFinalBlock(_enc.GetBytes(inputText) , 0, inputText.Length);
				_out = Convert.ToBase64String(plainText);
			}
			if (mode.Equals (EncryptMode.Decrypt)) {
				//decrypt
				var plainText = _rcipher.CreateDecryptor().TransformFinalBlock(Convert.FromBase64String(inputText), 0, Convert.FromBase64String(inputText).Length);
				_out = _enc.GetString(plainText);
			}
			_rcipher.Dispose();
			return _out;// return encrypted/decrypted string
		}

		/**
		 * This function encrypts the plain text to cipher text using the key
		 * provided. You'll have to use the same key for decryption
		 * 
		 * @param _plainText
		 *            Plain text to be encrypted
		 * @param _key
		 *            Encryption Key. You'll have to use the same key for decryption
		 * @return returns encrypted (cipher) text
		 */
		public string Encrypt (string plainText, string key, string initVector)
		{
			return EncryptDecrypt(plainText, key, EncryptMode.Encrypt, initVector);
		}

		/***
		 * This funtion decrypts the encrypted text to plain text using the key
		 * provided. You'll have to use the same key which you used during
		 * encryprtion
		 * 
		 * @param _encryptedText
		 *            Encrypted/Cipher text to be decrypted
		 * @param _key
		 *            Encryption key which you used during encryption
		 * @return encrypted value
		 */
		 
		public string Decrypt(string encryptedText, string key, string initVector)
		{
			return EncryptDecrypt(encryptedText, key, EncryptMode.Decrypt, initVector);
		}

	    /***
		 * This function decrypts the encrypted text to plain text using the key
		 * provided. You'll have to use the same key which you used during
		 * encryption
		 * 
		 * @param _encryptedText
		 *            Encrypted/Cipher text to be decrypted
		 * @param _key
		 *            Encryption key which you used during encryption
		 */
		public string GetHashSha256(string text, int length)
		{
			var bytes = Encoding.UTF8.GetBytes(text);
			var hashstring = new SHA256Managed();
			var hash = hashstring.ComputeHash(bytes);
			var hashString = string.Empty;
			foreach (var x in hash)
			{
				hashString += $"{x:x2}"; //covert to hex string
			}
			if (length > hashString.Length)
				return hashString;
			else
				return hashString.Substring (0, length);
		}
		
		//this function is no longer used.
		public string Md5Hash(string text)
		{
			MD5 md5 = new MD5CryptoServiceProvider();

			//compute hash from the bytes of text
			md5.ComputeHash(Encoding.ASCII.GetBytes(text));

			//get hash result after compute it
			var result = md5.Hash;

			var strBuilder = new StringBuilder();
			foreach (byte bt in result)
			{
                //change it into 2 hexadecimal digits
			    //for each byte
			    strBuilder.Append(bt.ToString("x2"));
			}
			Console.WriteLine (@"md5 hash of they key=" + strBuilder);
			return strBuilder.ToString();

        }
    }
}
