using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QResources
{
    //NOTE:
    //Folder(s) "Resources" can be created everywhere from root "Assests/*", that can be access by Unity or Application

    //BEWARD:
    //All content(s) in folder(s) "Resources" will be builded to Application, even they ightn't be used in Build-Game Application

    #region ==================================== Prefab

    public static List<GameObject> GetPrefab(params string[] PathChildInResources)
    {
        string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
        GameObject[] LoadArray = Resources.LoadAll<GameObject>(PathInResources);
        List<GameObject> LoadList = new List<GameObject>();
        LoadList.AddRange(LoadArray);
        return LoadList;
    }

    #endregion

    #region ==================================== Sprite

    public static List<Sprite> GetSprite(params string[] PathChildInResources)
    {
        string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
        Sprite[] LoadArray = Resources.LoadAll<Sprite>(PathInResources);
        List<Sprite> LoadList = new List<Sprite>();
        LoadList.AddRange(LoadArray);
        return LoadList;
    }

    #endregion

    #region ==================================== Text Asset

    public static List<TextAsset> GetTextAsset(params string[] PathChildInResources)
    {
        string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
        TextAsset[] LoadArray = Resources.LoadAll<TextAsset>(PathInResources);
        List<TextAsset> LoadList = new List<TextAsset>();
        LoadList.AddRange(LoadArray);
        return LoadList;
    }

    #endregion
}

public class QPath
{
    public const string ExamplePath = @"D:/ClassFileIO.txt";

    #region ==================================== Path Get

    public enum PathType { None, Persistent, Assets, Resources, Document, Picture, Music, Video, }

    public static string GetPath(PathType PathType, params string[] PathChild)
    {
        string PathFinal = "";

        switch (PathType)
        {
            case PathType.Persistent:
                PathFinal = Application.persistentDataPath;
                break;
            case PathType.Assets:
                PathFinal = Application.dataPath;
                break;
            case PathType.Resources:
                PathFinal = Application.dataPath + @"/resources";
                break;
            case PathType.Document:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                break;
            case PathType.Picture:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                break;
            case PathType.Music:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                break;
            case PathType.Video:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                break;
        }

        foreach (string PathChildAdd in PathChild)
        {
            QEncypt.GetEncyptAdd('/', PathFinal, PathChildAdd, out PathFinal);
        }

        return PathFinal;
    }

    #endregion

    #region ==================================== File

    //Name

    public static string GetFileName(string PathPrimary)
    {
        return Path.GetFileNameWithoutExtension(PathPrimary);
    }

    public static string GetFileName(PathType PathType, params string[] PathChild)
    {
        return GetFileName(GetPath(PathType, PathChild));
    }

    //Extension

    public static string GetFileExtension(string PathPrimary)
    {
        return Path.GetExtension(PathPrimary);
    }

    public static string GetFileExtension(PathType PathType, params string[] PathChild)
    {
        return GetFileExtension(GetPath(PathType, PathChild));
    }

    #endregion

    #region ==================================== Path Pannel

#if UNITY_EDITOR

    //Open

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public static (bool Result, string Path, string Name) GetPathFolderOpenPanel(string Title, string PathPrimary = "")
    {
        string Path = EditorUtility.OpenFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public static (bool Result, string Path, string Name) GetPathFileOpenPanel(string Title, string Extension, string PathPrimary = "")
    {
        string Path = EditorUtility.OpenFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, Extension);
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    //Save

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public static (bool Result, string Path, string Name) GetPathFolderSavePanel(string Title, string PathPrimary = "")
    {
        string Path = EditorUtility.SaveFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public static (bool Result, string Path, string Name) GetPathFileSavePanel(string Title, string Extension, string PathPrimary = "")
    {
        string Path = EditorUtility.SaveFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "", Extension);
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

#endif

    #endregion

    #region ==================================== Path Exist

    //File

    public static bool GetPathFileExist(string PathFile)
    {
        return File.Exists(PathFile);
    }

    public static bool GetPathFileExist(PathType PathType, params string[] PathChild)
    {
        return File.Exists(GetPath(PathType, PathChild));
    }

    //Folder

    public static bool GetPathFolderExist(string PathFolder)
    {
        return Directory.Exists(PathFolder);
    }

    public static bool GetPathFolderExist(PathType PathType, params string[] PathChild)
    {
        return Directory.Exists(GetPath(PathType, PathChild));
    }

    #endregion
}

public class QFileIO : QPath
{
    #region ==================================== File IO Write 

    private string TextWrite = "";

    #region ------------------------------------ Write Start

    public void SetWriteStart(string Path)
    {
        SetWriteToFile(Path, GetWriteString());
    } //Call Last

    private void SetWriteToFile(string Path, string Data)
    {
        try
        {
            using (FileStream Stream = File.Create(Path))
            {
                byte[] Info = new UTF8Encoding(true).GetBytes(Data);
                Stream.Write(Info, 0, Info.Length);
            }
        }
        catch
        {
            Debug.LogErrorFormat("[Error] File Write Fail: {0}", Path);
        }
    }

    public void SetWriteClear()
    {
        TextWrite = "";
    }

    #endregion

    #region ------------------------------------ Write Add

    public void SetWriteAdd()
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += "";
    }

    public void SetWriteAdd(string DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += DataAdd;
    }

    public void SetWriteAdd(char Key, params string[] DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += QEncypt.GetEncypt(Key, DataAdd.ToList());
    }

    public void SetWriteAdd(char Key, params int[] DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += QEncypt.GetEncypt(Key, DataAdd.ToList());
    }

    public void SetWriteAdd(char Key, params float[] DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += QEncypt.GetEncypt(Key, DataAdd.ToList());
    }

    public void SetWriteAdd(char Key, params bool[] DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += QEncypt.GetEncypt(Key, DataAdd.ToList());
    }

    public void SetWriteAdd(int DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += DataAdd.ToString();
    }

    public void SetWriteAdd(float DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += DataAdd.ToString();
    }

    public void SetWriteAdd(double DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += DataAdd.ToString();
    }

    public void SetWriteAdd(bool DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += ((DataAdd) ? "True" : "False");
    }

    public void SetWriteAdd(char Key, Vector2 DataAdd)
    {
        SetWriteAdd(QEncypt.GetEncyptVector2(Key, DataAdd));
    }

    public void SetWriteAdd(char Key, Vector2Int DataAdd)
    {
        SetWriteAdd(QEncypt.GetEncyptVector2Int(Key, DataAdd));
    }

    public void SetWriteAdd(char Key, Vector3 DataAdd)
    {
        SetWriteAdd(QEncypt.GetEncyptVector3(Key, DataAdd));
    }

    public void SetWriteAdd(char Key, Vector3Int DataAdd)
    {
        SetWriteAdd(QEncypt.GetEncyptVector3Int(Key, DataAdd));
    }

    public void SetWriteAdd<EnumType>(EnumType DataAdd)
    {
        if (TextWrite.Length != 0)
        {
            TextWrite += "\n";
        }

        TextWrite += QEnum.GetChoice(DataAdd).ToString();
    }

    public string GetWriteString()
    {
        return TextWrite;
    }

    #endregion

    #endregion

    #region ==================================== File IO Read

    private List<string> TextRead = new List<string>();
    private int ReadRun = -1;

    #region ------------------------------------ Read Start

    public void SetReadStart(string Path)
    {
        TextRead = GetReadFromFile(Path);
    } //Call First

    private List<string> GetReadFromFile(string Path)
    {
        try
        {
            List<string> TextRead = new List<string>();
            using (StreamReader Stream = File.OpenText(Path))
            {
                string ReadRun = "";
                while ((ReadRun = Stream.ReadLine()) != null)
                {
                    TextRead.Add(ReadRun);
                }
            }
            return TextRead;
        }
        catch
        {
            Debug.LogErrorFormat("[Error] File Read Fail: {0}", Path);
            return null;
        }
    }

    public void SetReadStart(TextAsset FileTest)
    {
        TextRead = GetReadFromFile(FileTest);
    } //Call First

    private List<string> GetReadFromFile(TextAsset FileTest)
    {
        try
        {
            string ReadRun = FileTest.text.Replace("\r\n", "\n");
            List<string> TextRead = QEncypt.GetDencyptString('\n', ReadRun);
            return TextRead;
        }
        catch
        {
            Debug.LogErrorFormat("[Error] File Read Fail: {0}", FileTest.name);
            return null;
        }
    }

    public void SetReadClear()
    {
        TextRead = new List<string>();
        ReadRun = -1;
    }

    #endregion

    #region ------------------------------------ Read Auto

    public void GetReadAuto()
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return;
        }

        ReadRun++;
    }

    public string GetReadAutoString()
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return "";
        }

        ReadRun++;
        return TextRead[ReadRun];
    }

    public List<string> GetReadAutoString(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new List<string>();
        }

        ReadRun++;
        return QEncypt.GetDencyptString(Key, TextRead[ReadRun]);
    }

    public List<int> GetReadAutoInt(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new List<int>();
        }

        ReadRun++;
        return QEncypt.GetDencyptInt(Key, TextRead[ReadRun]);
    }

    public List<float> GetReadAutoFloat(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new List<float>();
        }

        ReadRun++;
        return QEncypt.GetDencyptFloat(Key, TextRead[ReadRun]);
    }

    public List<bool> GetReadAutoBool(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new List<bool>();
        }

        ReadRun++;
        return QEncypt.GetDencyptBool(Key, TextRead[ReadRun]);
    }

    public int GetReadAutoInt()
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return 0;
        }

        ReadRun++;
        return int.Parse(TextRead[ReadRun]);
    }

    public float GetReadAutoFloat()
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return 0f;
        }

        ReadRun++;
        return float.Parse(TextRead[ReadRun]);
    }

    public double GetReadAutoDouble()
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return 0f;
        }

        ReadRun++;
        return double.Parse(TextRead[ReadRun]);
    }

    public bool GetReadAutoBool()
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return false;
        }

        ReadRun++;
        return TextRead[ReadRun] == "True";
    }

    public Vector2 GetReadAutoVector2(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new Vector2();
        }

        ReadRun++;
        return QEncypt.GetDencyptVector2(Key, TextRead[ReadRun]);
    }

    public Vector2Int GetReadAutoVector2Int(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new Vector2Int();
        }

        ReadRun++;
        return QEncypt.GetDencyptVector2Int(Key, TextRead[ReadRun]);
    }

    public Vector3 GetReadAutoVector3(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new Vector3();
        }

        ReadRun++;
        return QEncypt.GetDencyptVector3(Key, TextRead[ReadRun]);
    }

    public Vector3Int GetReadAutoVector3Int(char Key)
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return new Vector3Int();
        }

        ReadRun++;
        return QEncypt.GetDencyptVector3Int(Key, TextRead[ReadRun]);
    }

    public EnumType GetReadAutoEnum<EnumType>()
    {
        if (ReadRun >= TextRead.Count - 1)
        {
            return QEnum.GetChoice<EnumType>(0);
        }

        ReadRun++;
        return QEnum.GetChoice<EnumType>(int.Parse(TextRead[ReadRun]));
    }

    public bool GetReadAutoEnd()
    {
        return ReadRun >= TextRead.Count - 1;
    }

    public int GetReadAutoCurrent()
    {
        return ReadRun;
    }

    public List<string> GetRead()
    {
        return TextRead;
    }

    #endregion

    #endregion
}