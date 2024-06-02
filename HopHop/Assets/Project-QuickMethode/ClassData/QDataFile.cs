using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class QDataFile
{
    #region ==================================== File IO Write 

    public List<string> DataWriteQueue { private set; get; } = new List<string>();

    public string DataWriteText { private set; get; } = "";

    #region ------------------------------------ Write Start

    public void SetWriteStart(string Path)
    {
        SetWriteToFile(Path, DataWriteText);
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
        DataWriteText = "";
        DataWriteQueue.Clear();
    }

    #endregion

    #region ------------------------------------ Write Add

    public void SetWriteAdd()
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += "";
        DataWriteQueue.Add("");
    }

    //Primary

    public void SetWriteAdd(string Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += Value;
        DataWriteQueue.Add(Value);
    }

    public void SetWriteAdd(int Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += Value;
        DataWriteQueue.Add(Value.ToString());
    }

    public void SetWriteAdd(float Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += Value;
        DataWriteQueue.Add(Value.ToString());
    }

    public void SetWriteAdd(double Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += Value;
        DataWriteQueue.Add(Value.ToString());
    }

    public void SetWriteAdd(bool Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += Value.ToString().ToLower();
        DataWriteQueue.Add(Value.ToString().ToLower());
    }

    public void SetWriteAdd<T>(T Value) where T : Enum
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += QEnum.GetChoice(Value).ToString();
        DataWriteQueue.Add(QEnum.GetChoice(Value).ToString());
    }

    //Params & List

    public void SetWriteAdd(char Key, params string[] Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += QString.GetSplit(Key, Value.ToList());
        DataWriteQueue.Add(QString.GetSplit(Key, Value.ToList()));
    }

    public void SetWriteAdd(char Key, params int[] Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += QString.GetSplit(Key, Value.ToList());
        DataWriteQueue.Add(QString.GetSplit(Key, Value.ToList()));
    }

    public void SetWriteAdd(char Key, params float[] Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += QString.GetSplit(Key, Value.ToList());
        DataWriteQueue.Add(QString.GetSplit(Key, Value.ToList()));
    }

    public void SetWriteAdd(char Key, params bool[] Value)
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += QString.GetSplit(Key, Value.ToList());
        DataWriteQueue.Add(QString.GetSplit(Key, Value.ToList()));
    }

    public void SetWriteAdd<T>(char Key, params T[] Value) where T : Enum
    {
        if (DataWriteText.Length != 0)
            DataWriteText += "\n";
        DataWriteText += QString.GetSplit(Key, Value.ToList());
        DataWriteQueue.Add(QString.GetSplit(Key, Value.ToList()));
    }

    //Vector

    public void SetWriteAdd(char Key, Vector2 Value)
    {
        SetWriteAdd(QString.GetSplitVector2(Key, Value));
    }

    public void SetWriteAdd(char Key, Vector2Int Value)
    {
        SetWriteAdd(QString.GetSplitVector2Int(Key, Value));
    }

    public void SetWriteAdd(char Key, Vector3 Value)
    {
        SetWriteAdd(QString.GetSplitVector3(Key, Value));
    }

    public void SetWriteAdd(char Key, Vector3Int Value)
    {
        SetWriteAdd(QString.GetSplitVector3Int(Key, Value));
    }

    #endregion

    #endregion

    #region ==================================== File IO Read

    public List<string> DataReadQueue { private set; get; } = new List<string>();

    public int RunRead { private set; get; } = -1;

    public bool RunReadEnd => RunRead >= DataReadQueue.Count - 1;

    #region ------------------------------------ Read Start

    public void SetReadStart(string Path)
    {
        DataReadQueue = GetReadFromFile(Path);
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
        DataReadQueue = GetReadFromFile(FileTest);
    } //Call First

    private List<string> GetReadFromFile(TextAsset FileTest)
    {
        try
        {
            string ReadRun = FileTest.text.Replace("\r\n", "\n");
            List<string> TextRead = QString.GetUnSplitString('\n', ReadRun);
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
        DataReadQueue = new List<string>();
        RunRead = -1;
    }

    public void SetReadReset()
    {
        RunRead = -1;
    }

    #endregion

    #region ------------------------------------ Read Auto

    //Emty

    public void GetReadAuto()
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return;
        RunRead++;
    }

    //Primary

    public string GetReadAutoString()
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return "";
        RunRead++;
        return DataReadQueue[RunRead];
    }

    public int GetReadAutoInt()
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return 0;
        RunRead++;
        return int.Parse(DataReadQueue[RunRead]);
    }

    public float GetReadAutoFloat()
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return 0f;
        RunRead++;
        return float.Parse(DataReadQueue[RunRead]);
    }

    public double GetReadAutoDouble()
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return 0f;
        RunRead++;
        return double.Parse(DataReadQueue[RunRead]);
    }

    public bool GetReadAutoBool()
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return false;
        RunRead++;
        return DataReadQueue[RunRead].ToLower() == "true";
    }

    public T GetReadAutoEnum<T>() where T : Enum
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return QEnum.GetChoice<T>(0);
        RunRead++;
        return QEnum.GetChoice<T>(int.Parse(DataReadQueue[RunRead]));
    }

    //Params & List

    public List<string> GetReadAutoString(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new List<string>();
        RunRead++;
        return QString.GetUnSplitString(Key, DataReadQueue[RunRead]);
    }

    public List<int> GetReadAutoInt(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new List<int>();
        RunRead++;
        return QString.GetUnSplitInt(Key, DataReadQueue[RunRead]);
    }

    public List<float> GetReadAutoFloat(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new List<float>();
        RunRead++;
        return QString.GetUnSplitFloat(Key, DataReadQueue[RunRead]);
    }

    public List<bool> GetReadAutoBool(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new List<bool>();
        RunRead++;
        return QString.GetUnSplitBool(Key, DataReadQueue[RunRead]);
    }

    public List<T> GetReadAutoString<T>(char Key) where T : Enum
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new List<T>();
        RunRead++;
        return QString.GetUnSplitEnum<T>(Key, DataReadQueue[RunRead]);
    }

    //Vector

    public Vector2 GetReadAutoVector2(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new Vector2();
        RunRead++;
        return QString.GetUnSplitVector2(Key, DataReadQueue[RunRead]);
    }

    public Vector2Int GetReadAutoVector2Int(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new Vector2Int();
        RunRead++;
        return QString.GetUnSplitVector2Int(Key, DataReadQueue[RunRead]);
    }

    public Vector3 GetReadAutoVector3(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new Vector3();
        RunRead++;
        return QString.GetUnSplitVector3(Key, DataReadQueue[RunRead]);
    }

    public Vector3Int GetReadAutoVector3Int(char Key)
    {
        if (RunRead >= DataReadQueue.Count - 1)
            return new Vector3Int();
        RunRead++;
        return QString.GetUnSplitVector3Int(Key, DataReadQueue[RunRead]);
    }

    #endregion

    #endregion
}
