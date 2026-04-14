using System;
using System.IO;
using System.Security.Cryptography;

namespace TodoApp.Services
{
    public static class EncryptionService
    {
        private static readonly string KeyFile = "data/encryption.key";
        private static readonly string IVFile = "data/encryption.iv";

        public static (byte[] Key, byte[] IV) GetOrCreateKeys()
        {
            if (File.Exists(KeyFile) && File.Exists(IVFile))
            {
                byte[] key = File.ReadAllBytes(KeyFile);
                byte[] iv = File.ReadAllBytes(IVFile);
                return (key, iv);
            }
            else
            {
                using (var aes = Aes.Create())
                {
                    aes.GenerateKey();
                    aes.GenerateIV();
                    
                    Directory.CreateDirectory("data");
                    File.WriteAllBytes(KeyFile, aes.Key);
                    File.WriteAllBytes(IVFile, aes.IV);
                    
                    return (aes.Key, aes.IV);
                }
            }
        }
    }
}