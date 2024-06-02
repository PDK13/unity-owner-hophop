using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class QEncypt
{
    #region ==================================== String Data

    #region ------------------------------------ String Data Main

    #region .................................... String Data Main Encypt

    #region List

    public static string GetEncypt(char Key, List<string> Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(string Key, List<string> Data)
    {
        return string.Join(Key, Data);
    }

    //

    public static string GetEncypt(char Key, List<int> Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(string Key, List<int> Data)
    {
        return string.Join(Key, Data);
    }

    //

    public static string GetEncypt(char Key, List<float> Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(string Key, List<float> Data)
    {
        return string.Join(Key, Data);
    }

    //

    public static string GetEncypt(char Key, List<bool> Data)
    {
        return string.Join(Key, Data.Select(t => t.ToString().ToLower()));
    }

    public static string GetEncypt(string Key, List<bool> Data)
    {
        return string.Join(Key, Data.Select(t => t.ToString().ToLower()));
    }

    //

    public static string GetEncypt<T>(char Key, List<T> Data) where T : Enum
    {
        string Encypt = "";
        //
        for (int i = 0; i < Data.Count; i++)
            Encypt = GetEncyptAdd(Key, Encypt, QEnum.GetChoice(Data[i]));
        //
        return Encypt;
    }

    public static string GetEncypt<T>(string Key, List<T> Data) where T : Enum
    {
        string Encypt = "";
        //
        for (int i = 0; i < Data.Count; i++)
            Encypt = GetEncyptAdd(Key, Encypt, QEnum.GetChoice(Data[i]));
        //
        return Encypt;
    }

    #endregion

    #region Array - Params

    public static string GetEncypt(char Key, params string[] Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(string Key, params string[] Data)
    {
        return string.Join(Key, Data);
    }

    //

    public static string GetEncypt(char Key, params int[] Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(string Key, params int[] Data)
    {
        return string.Join(Key, Data);
    }

    //

    public static string GetEncypt(char Key, params float[] Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(string Key, params float[] Data)
    {
        return string.Join(Key, Data);
    }

    //

    public static string GetEncypt(char Key, params bool[] Data)
    {
        return string.Join(Key, Data.Select(t => t.ToString().ToLower()));
    }

    public static string GetEncypt(string Key, params bool[] Data)
    {
        return string.Join(Key, Data.Select(t => t.ToString().ToLower()));
    }

    //

    public static string GetEncypt<T>(char Key, params T[] Data) where T : Enum
    {
        string Encypt = "";
        //
        for (int i = 0; i < Data.Length; i++)
            Encypt = GetEncyptAdd(Key, Encypt, QEnum.GetChoice(Data[i]));
        //
        return Encypt;
    }

    public static string GetEncypt<T>(string Key, params T[] Data) where T : Enum
    {
        string Encypt = "";
        //
        for (int i = 0; i < Data.Length; i++)
            Encypt = GetEncyptAdd(Key, Encypt, QEnum.GetChoice(Data[i]));
        //
        return Encypt;
    }

    #endregion

    #endregion

    #region .................................... String Data Main Add Encypt

    #region Single

    public static string GetEncyptAdd(char Key, string Data, string DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd;
    }

    public static string GetEncyptAdd(string Key, string Data, string DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd;
    }

    //

    public static string GetEncyptAdd(char Key, string Data, int DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
    }

    public static string GetEncyptAdd(string Key, string Data, int DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
    }

    //

    public static string GetEncyptAdd(char Key, string Data, float DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
    }

    public static string GetEncyptAdd(string Key, string Data, float DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
    }

    //

    public static string GetEncyptAdd(char Key, string Data, bool DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + (DataAdd.ToString().ToLower());
    }

    public static string GetEncyptAdd(string Key, string Data, bool DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + (DataAdd.ToString().ToLower());
    }

    //

    public static string GetEncyptAdd<T>(char Key, string Data, T DataAdd) where T : Enum
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd;
    }

    public static string GetEncyptAdd<T>(string Key, string Data, T DataAdd) where T : Enum
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd;
    }

    #endregion

    #region List

    public static string GetEncyptAdd(char Key, string Data, List<string> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static string GetEncyptAdd(string Key, string Data, List<string> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    //

    public static string GetEncyptAdd(char Key, string Data, List<int> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static string GetEncyptAdd(string Key, string Data, List<int> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    //

    public static string GetEncyptAdd(char Key, string Data, List<float> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static string GetEncyptAdd(string Key, string Data, List<float> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    //

    public static string GetEncyptAdd(char Key, string Data, List<bool> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static string GetEncyptAdd(string Key, string Data, List<bool> DataAdd)
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    //

    public static string GetEncyptAdd<T>(char Key, string Data, List<T> DataAdd) where T : Enum
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static string GetEncyptAdd<T>(string Key, string Data, List<T> DataAdd) where T : Enum
    {
        return Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    #endregion

    #endregion

    #region .................................... String Data Main Dencypt

    public static List<string> GetDencyptString(char Key, string Data)
    {
        return Data.Split(Key).ToList();
    }

    public static List<string> GetDencyptString(string Key, string Data)
    {
        return Data.Split(Key).ToList();
    }

    //

    public static List<int> GetDencyptInt(char Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(int.Parse);
    }

    public static List<int> GetDencyptInt(string Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(int.Parse);
    }

    //

    public static List<float> GetDencyptFloat(char Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(float.Parse);
    }

    public static List<float> GetDencyptFloat(string Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(float.Parse);
    }

    //

    public static List<bool> GetDencyptBool(char Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(bool.Parse);
    }

    public static List<bool> GetDencyptBool(string Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(bool.Parse);
    }

    //

    public static List<T> GetDencyptEnum<T>(char Key, string Data) where T : Enum
    {
        if (Data.Equals(""))
            return new List<T>();
        //
        List<string> DataString = GetDencyptString(Key, Data);
        //
        List<T> DataEnum = new List<T>();
        //
        for (int i = 0; i < DataString.Count; i++)
            DataEnum.Add(QEnum.GetChoice<T>(int.Parse(DataString[i])));
        //
        return DataEnum;
    }

    public static List<T> GetDencyptEnum<T>(string Key, string Data) where T : Enum
    {
        if (Data.Equals(""))
            return new List<T>();
        //
        List<string> DataString = GetDencyptString(Key, Data);
        //
        List<T> DataEnum = new List<T>();
        //
        for (int i = 0; i < DataString.Count; i++)
            DataEnum.Add(QEnum.GetChoice<T>(int.Parse(DataString[i])));
        //
        return DataEnum;
    }

    #endregion

    #endregion

    #region ------------------------------------ String Data Vector

    #region .................................... String Data Vector Encypt

    public static string GetEncyptVector2(char Key, Vector2 Data)
    {
        return GetEncypt(Key, Data.x, Data.y);
    }

    public static string GetEncyptVector2(string Key, Vector2 Data)
    {
        return GetEncypt((string)Key, Data.x, Data.y);
    }

    //

    public static string GetEncyptVector3(char Key, Vector3 Data)
    {
        return GetEncypt(Key, Data.x, Data.y, Data.z);
    }

    public static string GetEncyptVector3(string Key, Vector3 Data)
    {
        return GetEncypt(Key, Data.x, Data.y, Data.z);
    }

    //

    public static string GetEncyptVector2Int(char Key, Vector2Int Data)
    {
        return GetEncypt(Key, Data.x, Data.y);
    }

    public static string GetEncyptVector2Int(string Key, Vector2Int Data)
    {
        return GetEncypt(Key, Data.x, Data.y);
    }

    //

    public static string GetEncyptVector3Int(char Key, Vector3Int Data)
    {
        return GetEncypt(Key, Data.x, Data.y, Data.z);
    }

    public static string GetEncyptVector3Int(string Key, Vector3Int Data)
    {
        return GetEncypt(Key, Data.x, Data.y, Data.z);
    }

    #endregion

    #region .................................... String Data Vector Dencypt

    public static Vector2 GetDencyptVector2(char Key, string Data)
    {
        List<float> Dencypt = GetDencyptFloat(Key, Data);
        //
        return new Vector2(Dencypt[0], Dencypt[1]);
    }

    public static Vector2 GetDencyptVector2(string Key, string Data)
    {
        List<float> Dencypt = GetDencyptFloat(Key, Data);
        //
        return new Vector2(Dencypt[0], Dencypt[1]);
    }

    //

    public static Vector3 GetDencyptVector3(char Key, string Data)
    {
        List<float> Dencypt = GetDencyptFloat(Key, Data);
        //
        return new Vector3(Dencypt[0], Dencypt[1], Dencypt[2]);
    }

    public static Vector3 GetDencyptVector3(string Key, string Data)
    {
        List<float> Dencypt = GetDencyptFloat(Key, Data);
        //
        return new Vector3(Dencypt[0], Dencypt[1], Dencypt[2]);
    }

    //

    public static Vector2Int GetDencyptVector2Int(char Key, string Data)
    {
        List<int> Dencypt = GetDencyptInt(Key, Data);
        //
        return new Vector2Int(Dencypt[0], Dencypt[1]);
    }

    public static Vector2Int GetDencyptVector2Int(string Key, string Data)
    {
        List<int> Dencypt = GetDencyptInt(Key, Data);
        //
        return new Vector2Int(Dencypt[0], Dencypt[1]);
    }

    //

    public static Vector3Int GetDencyptVector3Int(char Key, string Data)
    {
        List<int> Dencypt = GetDencyptInt(Key, Data);
        //
        return new Vector3Int(Dencypt[0], Dencypt[1], Dencypt[2]);
    }

    public static Vector3Int GetDencyptVector3Int(string Key, string Data)
    {
        List<int> Dencypt = GetDencyptInt(Key, Data);
        //
        return new Vector3Int(Dencypt[0], Dencypt[1], Dencypt[2]);
    }

    #endregion

    #endregion

    #endregion
}

public class QEncypt256Bit
{
    //This constant is used to determine the keysize of the encryption algorithm in bits.
    //We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int KEY_SIZE = 256;

    //This constant determines the number of iterations for the password bytes generation function.
    private const int DERIVATION_ITERATIONS = 1000;

    public static string SetEncrypt(string Data, string Pass)
    {
        //Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
        //so that the same Salt and IV values can be used when decrypting.  
        byte[] saltStringBytes = SetGenerate256BitsOfRandomEntropy();
        byte[] ivStringBytes = SetGenerate256BitsOfRandomEntropy();
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(Data);
        using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(Pass, saltStringBytes, DERIVATION_ITERATIONS))
        {
            byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);
            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            //Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                            byte[] cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
    }

    public static string SetDecrypt(string Data, string Pass)
    {
        //Get the complete stream of bytes that represent:
        //[32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
        byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(Data);
        //Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
        byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEY_SIZE / 8).ToArray();
        //Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
        byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEY_SIZE / 8).Take(KEY_SIZE / 8).ToArray();
        //Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
        byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((KEY_SIZE / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((KEY_SIZE / 8) * 2)).ToArray();

        using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(Pass, saltStringBytes, DERIVATION_ITERATIONS))
        {
            byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);
            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                {
                    using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }
    }

    private static byte[] SetGenerate256BitsOfRandomEntropy()
    {
        byte[] randomBytes = new byte[32]; //32 Bytes will give us 256 bits.
        using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
        {
            //Fill the array with cryptographically secure random bytes.
            rngCsp.GetBytes(randomBytes);
        }
        return randomBytes;
    }
} //From: Tạ Xuân Hiển