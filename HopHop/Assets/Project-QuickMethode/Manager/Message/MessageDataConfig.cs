using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "message-config", menuName = "Message/Message Config", order = 0)]
public class MessageDataConfig : ScriptableObject
{
    public List<MessageDataConfigTextAuthor> Author = new List<MessageDataConfigTextAuthor>();

    public MessageDataTextDelayDefault MessageTextDelayDefault;

    public List<string> AuthorName
    {
        get
        {
            List<string> NameFound = new List<string>();
            //
            if (Author == null)
                return NameFound;
            //
            if (Author.Count == 0)
                return NameFound;
            //
            foreach (MessageDataConfigTextAuthor AuthorItem in Author)
                NameFound.Add(AuthorItem.Name);
            //
            return NameFound;
        }
    }

    public List<Sprite> AuthorAvatar
    {
        get
        {
            if (Author == null)
                return null;
            //
            if (Author.Count == 0)
                return null;
            //
            List<Sprite> NameFound = new List<Sprite>();
            //
            foreach (MessageDataConfigTextAuthor AuthorItem in Author)
                NameFound.Add(AuthorItem.Avatar);
            //
            return NameFound;
        }
    }

    public Sprite GetAvatar(string Name)
    {
        return Author.Find(t => t.Name == Name).Avatar;
    }
}

//Data

[Serializable]
public class MessageDataConfigTextAuthor
{
    public string Name;
    public Sprite Avatar;
}

[Serializable]
public class MessageDataText
{
    public int AuthorIndex; //Use index for 'AuthorName' and 'AuthorAvatar'!!
    public string Message;
    //
    public float DelayAlpha;
    public float DelaySpace;
    public float DelayMark;
    //
    public string TriggerCode;
    public GameObject TriggerObject;

    public MessageDataText()
    {
        //...
    }

    public MessageDataText(MessageDataTextDelayDefault Default)
    {
        DelayAlpha = Default.DelayAlpha;
        DelaySpace = Default.DelaySpace;
        DelayMark = Default.DelayMark;
    }
}

[Serializable]
public class MessageDataChoice
{
    public string Text;
    //
    public int AuthorIndex;
    public string Message;
    //
    public MessageDataConfigText Next;
}

//Editor

[Serializable]
public class MessageDataTextDelayDefault
{
    public float DelayAlpha;
    public float DelaySpace;
    public float DelayMark;
}

#if UNITY_EDITOR

[CustomEditor(typeof(MessageDataConfig))]
public class MessageDataConfigEditor : Editor
{
    private MessageDataConfig m_target;

    private SerializedProperty Author;
    private SerializedProperty MessageTextDelayDefault;

    private void OnEnable()
    {
        m_target = target as MessageDataConfig;
        //
        Author = QUnityEditorCustom.GetField(this, "Author");
        MessageTextDelayDefault = QUnityEditorCustom.GetField(this, "MessageTextDelayDefault");
        //
        SetConfigAuthorFixed();
    }

    private void OnDisable()
    {
        SetConfigAuthorFixed();
    }

    private void OnDestroy()
    {
        SetConfigAuthorFixed();
    }

    public override void OnInspectorGUI()
    {
        QUnityEditorCustom.SetUpdate(this);
        //
        QUnityEditorCustom.SetField(Author);
        QUnityEditorCustom.SetField(MessageTextDelayDefault);
        //
        QUnityEditorCustom.SetApply(this);
    }

    //

    private void SetConfigAuthorFixed()
    {
        bool RemoveEmty = false;
        int Index = 0;
        while (Index < m_target.Author.Count)
        {
            if (m_target.Author[Index].Name == "")
            {
                RemoveEmty = true;
                m_target.Author.RemoveAt(Index);
            }
            else
                Index++;
        }
        QUnityEditorCustom.SetDirty(m_target);
        //
        if (RemoveEmty)
            Debug.Log("[Message] Author(s) emty have been remove from list");
    }
}

#endif