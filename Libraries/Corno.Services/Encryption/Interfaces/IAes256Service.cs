namespace Corno.Services.Encryption.Interfaces
{
    public interface IAes256Service
    {
        #region -- Methods --
        string Encrypt(string plainText, string key, string initVector);
        string Decrypt(string encryptedText, string key, string initVector);
        string GetHashSha256(string text, int length);
        string GenerateRandomIv(int length);
        string Md5Hash(string text);
        #endregion
    }
}
