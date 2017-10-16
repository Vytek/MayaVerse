using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public static class Encryption {
    public static string FileType = ".encry";

    internal static void Encryptor(String FileCompleteName, List<GameObjectValue> ToSave) {        
        using (Stream innerStream = File.Create(FileCompleteName)) {
            using (Stream cryptoStream = new CryptoStream(innerStream, GetDES().CreateEncryptor(), CryptoStreamMode.Write)) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(cryptoStream, ToSave);
            }
        }
    }

    internal static object Decryptor(String FileCompleteName) {
        try {
            object load;
            using (Stream innerStream = File.Open(FileCompleteName, FileMode.Open)) {
                using (Stream cryptoStream = new CryptoStream(innerStream, GetDES().CreateDecryptor(), CryptoStreamMode.Read)) {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    load = binaryFormatter.Deserialize(cryptoStream);
                }
            }
            if (load is List<GameObjectValue>) {
                return (List<GameObjectValue>)load;
            }
            return null;
        } catch {
            return "Failed to decrypting, bad password?";
        }
    }
    private static DESCryptoServiceProvider GetDES() {
        string hash;
        using (MD5 md5Hash = MD5.Create()) {
            hash = GetMd5Hash(md5Hash, SerializableManager.current.password);
            hash = hash.Substring(0, 8);
        }
        // Setup the password generator
        DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
        DES.Key = ASCIIEncoding.ASCII.GetBytes(hash);
        DES.IV = ASCIIEncoding.ASCII.GetBytes(hash);
        return DES;
    }

    private static string GetMd5Hash(MD5 md5Hash, string input) {
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
            sBuilder.Append(data[i].ToString("x2"));
        return sBuilder.ToString();
    }
}