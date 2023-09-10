using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class DataHashing : MonoBehaviour
{
    public const string salt = "EYM0fe6oFNNWKP0Px0AR9wtCYKZ0O1Z/";

    [SerializeField]
    private byte[] key = new byte[] { 177, 81, 67, 77, 68, 206, 244, 209, 83, 148, 41, 64, 149, 18, 127, 103, 226, 58, 50, 239, 153, 20, 180, 142, 26, 3, 2, 87, 164, 193, 124, 109 };
    [SerializeField]
    private byte[] iv = new byte[] { 110, 125, 20, 142, 213, 74, 192, 89, 133, 178, 187, 217, 0, 16, 115, 103, 226, 58, 50, 239, 153, 20, 180, 142, 26, 3, 2, 87, 164, 193, 124, 109 };


    public string GenerateRandomKey(int keyLength)
    {
        using (RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
    public string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {

            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor();

            byte[] encryptedData;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                encryptedData = msEncrypt.ToArray();
            }

            return Convert.ToBase64String(encryptedData);
        }
    }

    public string Decrypt(string encryptedText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor();

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            string decryptedText = null;

            using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        decryptedText = srDecrypt.ReadToEnd();
                    }
                }
            }

            return decryptedText;
        }
    }
}
