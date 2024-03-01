//IMPORTANCE: This JSON class still can't handle with big-size data!!
using System.Collections.Generic;
using UnityEngine;

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
        QDataFile FileIO = new QDataFile();
        FileIO.SetWriteAdd(JsonData);
        FileIO.SetWriteStart(Path);
    }

    public static void SetDataPath<ClassData>(ClassData Data, string Path)
    {
        string JsonData = JsonUtility.ToJson(Data, true);
        //
        QDataFile FileIO = new QDataFile();
        FileIO.SetWriteAdd(JsonData);
        FileIO.SetWriteStart(Path);
    }

    public static ClassData GetDataPath<ClassData>(string Path)
    {
        QDataFile FileIO = new QDataFile();
        FileIO.SetReadStart(Path);
        List<string> JSonRead = FileIO.DataReadQueue;
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