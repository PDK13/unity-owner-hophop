using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "message-data", menuName = "", order = 0)]
public class MessageConfig : ScriptableObject
{
    public List<MessageConfigSingle> List;

    public MessageConfig()
    {
        List = new List<MessageConfigSingle>();
    }

    public void SetWrite(string Text, Color Color)
    {
        List.Add(new MessageConfigSingle(Text, Color, 0f, 0f, 0f));
    }

    public void SetWrite(string Text, Color Color, float DelayAlpha, float DelaySpace, float DelayFianl)
    {
        List.Add(new MessageConfigSingle(Text, Color, DelayAlpha, DelaySpace, DelayFianl));
    }

    public void SetWriteByChar(string Text, Color Color, float Delay)
    {
        List.Add(new MessageConfigSingle(Text, Color, Delay, 0f, 0f));
    }

    public void SetWriteByWord(string Text, Color Color, float Delay)
    {
        List.Add(new MessageConfigSingle(Text, Color, 0f, Delay, 0f));
    }

    public void SetWriteDelay(float Delay)
    {
        List.Add(new MessageConfigSingle("", Color.clear, 0f, 0f, Delay));
    }
}

[Serializable]
public class MessageConfigSingle
{
    [SerializeField][TextArea(0, 5)] private string m_text;
    [SerializeField] private Color m_color;
    public float DelayAlpha;
    public float DelaySpace;
    public float DelayFinal;

    public string Text => m_color != Color.clear ? QColor.GetTextHexFormat(m_color, m_text) : m_text;

    public MessageConfigSingle(string Text, Color Color, float DelayAlpha, float DelaySpace, float DelayFinal)
    {
        this.m_text = Text;
        this.m_color = Color;
        this.DelayAlpha = DelayAlpha;
        this.DelaySpace = DelaySpace;
        this.DelayFinal = DelayFinal;
    }
}