using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class QPlayerPrefs
{
    #region ==================================== Set

    #region ------------------------------------ Set Clear

    public static void SetValueClearAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public static void SetValueClear(string Name)
    {
        if (!GetValueExist(Name))
        {
            Debug.LogError("SetValueClear: Not Exist" + "\"" + Name + "\"");
            return;
        }
        PlayerPrefs.DeleteKey(Name);
        PlayerPrefs.Save();
    }

    #endregion

    #region ------------------------------------ Set Primary

    public static void SetValue(string Name, string Value)
    {
        PlayerPrefs.SetString(Name, Value);
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, int Value)
    {
        PlayerPrefs.SetInt(Name, Value);
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, float Value)
    {
        PlayerPrefs.SetFloat(Name, Value);
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, bool Value)
    {
        PlayerPrefs.SetInt(Name, (Value) ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void SetValue<EnumType>(string Name, EnumType Value)
    {
        PlayerPrefs.SetInt(Name, QEnum.GetChoice(Value));
        PlayerPrefs.Save();
    }

    #endregion

    #region ------------------------------------ Set Params

    public static void SetValue(string Name, char Key, params string[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, char Key, params int[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, char Key, params float[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    public static void SetValue(string Name, char Key, params bool[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    public static void SetValueEnum<EnumType>(string Name, char Key, params EnumType[] Value)
    {
        PlayerPrefs.SetString(Name, QEncypt.GetEncypt(Key, Value.ToList()));
        PlayerPrefs.Save();
    }

    #endregion

    #region ------------------------------------ Set Vector

    public static void SetValue(string Name, Vector2 Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector2(';', Value));
    }

    public static void SetValue(string Name, Vector2Int Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector2Int(';', Value));
    }

    public static void SetValue(string Name, Vector3 Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector3(';', Value));
    }

    public static void SetValue(string Name, Vector3Int Value)
    {
        SetValue(Name, QEncypt.GetEncyptVector3Int(';', Value));
    }

    #endregion

    #region ------------------------------------ Set Time

    public static void SetValue(string Name, DateTime Value, string FormatTime)
    {
        SetValue(Name, Value.ToString(FormatTime));
    }

    #endregion

    #endregion

    #region ==================================== Get

    #region ------------------------------------ Get Exist

    public static bool GetValueExist(string Name)
    {
        return PlayerPrefs.HasKey(Name);
    }

    #endregion

    #region ------------------------------------ Get Primary

    public static string GetValueString(string Name, string Default = "")
    {
        return PlayerPrefs.GetString(Name, Default);
    }

    public static int GetValueInt(string Name, int Default = 0)
    {
        return PlayerPrefs.GetInt(Name, Default);
    }

    public static float GetValueFloat(string Name, float Default = 0.0f)
    {
        return PlayerPrefs.GetFloat(Name, Default);
    }

    public static bool GetValueBool(string Name, bool Default = false)
    {
        if (PlayerPrefs.GetInt(Name, 0) == 1)
        {
            return true;
        }
        return Default;
    }

    public static EnumType GetValueEnum<EnumType>(string Name)
    {
        return QEnum.GetChoice<EnumType>(PlayerPrefs.GetInt(Name));
    }

    #endregion

    #region ------------------------------------ Get Params

    public static List<string> GetValueString(string Name, char Key)
    {
        return QEncypt.GetDencyptString(Key, GetValueString(Name));
    }

    public static List<int> GetValueInt(string Name, char Key)
    {
        return QEncypt.GetDencyptInt(Key, GetValueString(Name));
    }

    public static List<float> GetValueFloat(string Name, char Key)
    {
        return QEncypt.GetDencyptFloat(Key, GetValueString(Name));
    }

    public static List<bool> GetValueBool(string Name, char Key)
    {
        return QEncypt.GetDencyptBool(Key, GetValueString(Name));
    }

    public static List<EnumType> GetValueEnum<EnumType>(string Name, char Key)
    {
        return QEncypt.GetDencyptEnum<EnumType>(Key, GetValueString(Name));
    }

    #endregion

    #region ------------------------------------ Get Vector

    public static Vector2 SetValueVector2(string Name)
    {
        return QEncypt.GetDencyptVector2(';', GetValueString(Name));
    }

    public static Vector2Int SetValueVector2Int(string Name)
    {
        return QEncypt.GetDencyptVector2Int(';', GetValueString(Name));
    }

    public static Vector3 SetValueVector3(string Name)
    {
        return QEncypt.GetDencyptVector3(';', GetValueString(Name));
    }

    public static Vector3Int SetValueVector3Int(string Name)
    {
        return QEncypt.GetDencyptVector3Int(';', GetValueString(Name));
    }

    #endregion

    #region ------------------------------------ Get Time

    public static DateTime GetValueTime(string Name, string FormatTime)
    {
        return QDateTime.GetConvert(GetValueString(Name), FormatTime);
    }

    #endregion

    #endregion

    #region ==================================== App First Run (Should Remove)

    public static bool SetFirstRun(string PlayerPref = "-First-Run")
    {
        if (GetValueExist(Application.productName + PlayerPref))
        {
            return false;
        }

        SetValue(Application.productName + PlayerPref, true);

        return true;
    }

    #endregion
}

public class QEnum
{
    public static int GetChoice<EnumType>(EnumType Choice)
    {
        //Simple: (int)EnumType
        return (int)Convert.ChangeType(Choice, typeof(int));
    }

    public static EnumType GetChoice<EnumType>(int Index)
    {
        //Simple: (EnumType)Index
        return (EnumType)Enum.ToObject(typeof(EnumType), Index);
    }

    public static List<string> GetListName<EnumType>(bool Fixed = true)
    {
        if (Fixed)
        {
            List<string> ListName = Enum.GetNames(typeof(EnumType)).ToList();
            for (int i = 0; i < ListName.Count; i++)
            {
                if (ListName[i][0].Equals('_'))
                {
                    ListName[i] = ListName[i].Remove(0, 1);
                }
                //
                ListName[i] = ListName[i].Replace("_", " ");
            }
            return ListName;
        }

        return Enum.GetNames(typeof(EnumType)).ToList();
    }

    public static List<int> GetListIndex<EnumType>()
    {
        return Enum.GetValues(typeof(EnumType)).Cast<int>().ToList();
    }

    public static List<int> GetListIndex<EnumType>(params EnumType[] Value)
    {
        List<int> Index = new List<int>();
        for (int i = 0; i < Value.Length; i++)
        {
            Index.Add((int)Convert.ChangeType(Value[i], typeof(int)));
        }

        return Index;
    }

    public static string GetName<EnumType>(int Index)
    {
        return Enum.GetName(typeof(EnumType), Index);
    }
}

public class QFlag
{
    //NOTE:
    //Bit       : "1 << 3" mean "0100" or "8"
    //Bit |     : 1 | 0 = 1 + 0 = 1
    //Bit &     : 1 & 0 = 1 * 0 = 0
    //Bit ~     : Revert Bit, like ~8 = ~0100 = 1011 + 1 = 1100 = -9 (?)
    //Add       : "Flag = Flag.A | Flag.B | Flag.C"
    //Remove    : "Flag &= ~Flag.A"
    //Exist     : "(Flag & Flag.A) == Flag.A" or "Flag.HasFlag(Flag.A)"
    //Emty      : "Alpha == 0"

    public static List<int> GetBit<EnumType>()
    {
        List<int> Index = QEnum.GetListIndex<EnumType>();
        for (int i = 0; i < Index.Count; i++)
        {
            if (i == 0)
            {
                Index[i] = 1;
            }
            else
            {
                Index[i] = Index[i - 1] * 2;
            }
        }

        return Index;
    }

    public static int GetChoice<EnumType>(params EnumType[] Choice)
    {
        int Sum32 = 0;
        foreach (EnumType Value in Choice)
        {
            int Value32 = (int)Convert.ChangeType(Value, typeof(int));
            Sum32 += Value32;
        }
        return Sum32;
    }

    public static int GetAdd<EnumType>(EnumType Current, params EnumType[] Choice)
    {
        int Sum32 = GetChoice(Current);
        foreach (EnumType Value in Choice)
        {
            if (GetExist(Current, Value))
            {
                continue;
            }

            int Value32 = (int)Convert.ChangeType(Value, typeof(int));
            Sum32 += Value32;
        }
        return Sum32;
    }

    public static int GetRemove<EnumType>(EnumType Current, params EnumType[] Choice)
    {
        int Sum32 = GetChoice(Current);
        foreach (EnumType Value in Choice)
        {
            if (!GetExist(Current, Value))
            {
                continue;
            }

            int Value32 = (int)Convert.ChangeType(Value, typeof(int));
            Sum32 -= Value32;
        }
        return Sum32;
    }

    public static bool GetExist<EnumType>(EnumType Current, params EnumType[] Check)
    {
        return (GetChoice(Current) & GetChoice(Check)) == GetChoice(Check);
    }

    public static bool GetEmty<EnumType>(EnumType Current)
    {
        return GetChoice(Current) == 0;
    }
}

public class QList
{
    #region ==================================== Get Data

    public static List<T> GetData<T>(List<T> Data)
    {
        //Use to Get Data from List, not it's Memory Pointer!!
        List<T> DataGet = new List<T>();
        foreach (T Value in Data)
        {
            DataGet.Add(Value);
        }

        return DataGet;
    }

    public static T[] GetData<T>(T[] Data)
    {
        //Use to Get Data from List, not it's Memory Pointer!!
        T[] DataGet = new T[Data.Length];
        for (int i = 0; i < Data.Length; i++)
        {
            DataGet[i] = Data[i];
        }

        return DataGet;
    }

    #endregion

    #region ==================================== Find Data

    public static T GetComponent<T>(List<GameObject> DataList, GameObject DataFind)
    {
        return DataList.Find(t => DataFind).GetComponent<T>();
    }

    public static T GetComponent<T>(List<Transform> DataList, Transform DataFind)
    {
        return DataList.Find(t => DataFind).GetComponent<T>();
    }

    #endregion

    #region ==================================== Get Random

    public static int GetIndexRandom(params int[] Percent)
    {
        //Get random index from percent index list!
        List<(int Index, int Percent)> ListPercent = new List<(int Index, int Percent)>();

        int MaxPercent = 0;
        int MaxIndex = -1;

        int SumPercent = 0;
        for (int i = 0; i < Percent.Length; i++)
        {
            if (Percent[i] >= 100)
            {
                return i; //Get index of 100% percent!
            }

            ListPercent.Add((i, Percent[i]));
            SumPercent += Percent[i];

            if (Percent[i] > MaxPercent)
            {
                MaxPercent = Percent[i];
                MaxIndex = i;
            }
        }

        int SumFixed = 100 - SumPercent;
        if (SumFixed != 0)
        {
            for (int i = 0; i < ListPercent.Count; i++)
            {
                int ChildPercent = ListPercent[i].Percent + (int)(1.0f * SumFixed / ListPercent.Count);
                ListPercent[i] = (ListPercent[i].Index, ChildPercent);
            }
        }

        ListPercent = ListPercent.OrderBy(t => t.Percent).ToList(); //Order by!

        int RandomPercent;
        int RandomLast = -1;
        for (int i = 0; i < 10; i++)
        {
            int RandomCurrent = Random.Range(0, 100);

            if (RandomLast == -1)
            {
                RandomLast = RandomCurrent;
            }
            else
            if (RandomLast == RandomCurrent)
            {
                continue;
            }
            else
            {
                RandomLast = RandomCurrent;
            }
        }
        RandomPercent = RandomLast;

        int randomNumber = 0;
        int lastNumber = -1;
        int maxAttempts = 10;
        for (int i = 0; randomNumber == lastNumber && i < maxAttempts; i++)
        {
            randomNumber = Random.Range(0, 10);
        }
        lastNumber = randomNumber;

        int CheckPercent = 0;
        foreach ((int Index, int Percent) Child in ListPercent)
        {
            CheckPercent += Child.Percent;

            if (CheckPercent < RandomPercent)
            {
                continue;
            }

            return Child.Index; //Get index of higher than random percent!
        }

        return MaxIndex; //Get index of highest percent!
    }

    #endregion
}

//IMPORTANCE: This JSON class still can't handle with big-size data!!
public class QJSON
{
    //NOTE:
    //Type "TextAsset" is a "Text Document" File or "*.txt" File

    //SAMPLE:
    //ClassData Data = ClassFileIO.GetDatafromJson<ClassData>(JsonDataTextDocument);

    #region ==================================== Path

    public static void SetDataPath(object Data, string Path)
    {
        string JsonData = JsonUtility.ToJson(Data, true);
        //
        QFileIO FileIO = new QFileIO();
        FileIO.SetWriteAdd(JsonData);
        FileIO.SetWriteStart(Path);
    }

    public static void SetDataPath<ClassData>(ClassData Data, string Path)
    {
        string JsonData = JsonUtility.ToJson(Data, true);
        //
        QFileIO FileIO = new QFileIO();
        FileIO.SetWriteAdd(JsonData);
        FileIO.SetWriteStart(Path);
    }

    public static ClassData GetDataPath<ClassData>(string Path)
    {
        QFileIO FileIO = new QFileIO();
        FileIO.SetReadStart(Path);
        List<string> JSonRead = FileIO.GetRead();
        //
        string JsonData = "";
        for (int i = 0; i < JSonRead.Count; i++)
        {
            JsonData += (FileIO.GetReadAutoString() + "\n");
        }
        //
        return JsonUtility.FromJson<ClassData>(JsonData);
    }

    #endregion

    #region ==================================== Primary

    public static string GetDataConvertJson(object JsonDataClass)
    {
        return JsonUtility.ToJson(JsonDataClass);
    }

    public static ClassData GetDataConvertClass<ClassData>(TextAsset JsonDataTextDocument)
    {
        return GetDataConvertClass<ClassData>(JsonDataTextDocument.text);
    }

    public static ClassData GetDataConvertClass<ClassData>(string JsonData)
    {
        return JsonUtility.FromJson<ClassData>(JsonData);
    }

    #endregion
}

public class QEncypt
{
    #region ==================================== String Split

    public static List<string> GetStringSplitList(string FatherString, char Key)
    {
        return FatherString.Split(Key).ToList();
    }

    #endregion

    #region ==================================== String Data

    #region ------------------------------------ String Data Main

    #region String Data Main Encypt

    //List!!

    public static string GetEncypt(char Key, List<string> Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(char Key, List<int> Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(char Key, List<float> Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(char Key, List<bool> Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt<EnumType>(char Key, List<EnumType> Data)
    {
        string Encypt = "";
        //
        for (int i = 0; i < Data.Count; i++)
        {
            GetEncyptAdd(Key, Encypt, QEnum.GetChoice(Data[i]), out Encypt);
        }
        //
        return Encypt;
    }

    //Array - Params!!

    public static string GetEncypt(char Key, params string[] Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(char Key, params int[] Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(char Key, params float[] Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt(char Key, params bool[] Data)
    {
        return string.Join(Key, Data);
    }

    public static string GetEncypt<EnumType>(char Key, params EnumType[] Data)
    {
        string Encypt = "";
        //
        for (int i = 0; i < Data.Length; i++)
        {
            GetEncyptAdd(Key, Encypt, QEnum.GetChoice(Data[i]), out Encypt);
        }
        //
        return Encypt;
    }

    #endregion

    #region String Data Main Add Encypt

    //Single

    public static void GetEncyptAdd(char Key, string Data, string DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd;
    }

    public static void GetEncyptAdd(char Key, string Data, int DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
    }

    public static void GetEncyptAdd(char Key, string Data, float DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + DataAdd.ToString();
    }

    public static void GetEncyptAdd(char Key, string Data, bool DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + ((DataAdd) ? "1" : "0");
    }

    //List

    public static void GetEncyptAdd(char Key, string Data, List<string> DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static void GetEncyptAdd(char Key, string Data, List<int> DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static void GetEncyptAdd(char Key, string Data, List<float> DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    public static void GetEncyptAdd(char Key, string Data, List<bool> DataAdd, out string DataFinal)
    {
        DataFinal = Data + ((Data.Length != 0) ? Key.ToString() : "") + GetEncypt(Key, DataAdd);
    }

    #endregion

    #region String Data Main Dencypt

    public static List<string> GetDencyptString(char Key, string Data)
    {
        return Data.Split(Key).ToList();
    }

    public static List<int> GetDencyptInt(char Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(int.Parse);
    }

    public static List<float> GetDencyptFloat(char Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(float.Parse);
    }

    public static List<bool> GetDencyptBool(char Key, string Data)
    {
        return Data.Split(Key).ToList().ConvertAll(bool.Parse);
    }

    public static List<EnumType> GetDencyptEnum<EnumType>(char Key, string Data)
    {
        if (Data.Equals(""))
        {
            return new List<EnumType>();
        }
        //
        List<string> DataString = GetDencyptString(Key, Data);
        //
        List<EnumType> DataEnum = new List<EnumType>();
        //
        for (int i = 0; i < DataString.Count; i++)
        {
            DataEnum.Add(QEnum.GetChoice<EnumType>(int.Parse(DataString[i])));
        }
        //
        return DataEnum;
    }

    #endregion

    #endregion

    #region ------------------------------------ String Data Vector

    #region String Data Vector Encypt

    public static string GetEncyptVector2(char Key, Vector2 Data)
    {
        return GetEncypt(Key, Data.x, Data.y);
    }

    public static string GetEncyptVector3(char Key, Vector3 Data)
    {
        return GetEncypt(Key, Data.x, Data.y, Data.z);
    }

    public static string GetEncyptVector2Int(char Key, Vector2Int Data)
    {
        return GetEncypt(Key, Data.x, Data.y);
    }

    public static string GetEncyptVector3Int(char Key, Vector3Int Data)
    {
        return GetEncypt(Key, Data.x, Data.y, Data.z);
    }

    #endregion

    #region String Data Vector Dencypt

    public static Vector2 GetDencyptVector2(char Key, string Data)
    {
        List<float> Dencypt = GetDencyptFloat(Key, Data);
        //
        return new Vector2(Dencypt[0], Dencypt[1]);
    }

    public static Vector3 GetDencyptVector3(char Key, string Data)
    {
        List<float> Dencypt = GetDencyptFloat(Key, Data);
        //
        return new Vector3(Dencypt[0], Dencypt[1], Dencypt[2]);
    }

    public static Vector2Int GetDencyptVector2Int(char Key, string Data)
    {
        List<int> Dencypt = GetDencyptInt(Key, Data);
        //
        return new Vector2Int(Dencypt[0], Dencypt[1]);
    }

    public static Vector3Int GetDencyptVector3Int(char Key, string Data)
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

public enum Opption { Yes = 1, No = 0 }

public enum Direction { None, Up, Down, Left, Right, }

public enum DirectionX { None = 0, Left = -1, Right = 1, }

public enum DirectionY { None = 0, Up = 1, Down = -1, }

public enum Axis { Up, Right, Forward, }

[Flags]
public enum Coordinates
{
    X = 1 << 0, //001 = 1
    Y = 1 << 1, //010 = 2
    Z = 1 << 2, //100 = 4
}