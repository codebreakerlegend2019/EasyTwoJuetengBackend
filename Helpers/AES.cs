using System.IO;
using System.Security.Cryptography;

namespace EasyTwoJuetengBackend.Helpers
{
    public class AES
    {
        public static byte[] Key = new byte[] { 215, 210, 125, 32, 68, 139, 179, 190, 60, 187, 104, 236, 178, 179, 159, 132, 23, 93, 110, 121, 46, 192, 140, 231, 79, 191, 154, 164, 65, 207, 40, 109 };
        public static byte[] IV = new byte[] { 39, 131, 103, 245, 37, 195, 2, 41, 159, 247, 140, 209, 3, 39, 50, 248 };


        public static string Encrypt(string plainText)
        {
            return Base64.Base64Encode(EncryptToBytes(plainText));
        }

        public static byte[] EncryptToBytes(string plainText)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        public static string Decrypt(string encryptedString)
        {
            return Decode(Base64.Base64Decode(encryptedString));
        }
        public static string Decode(byte[] cipherText)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}