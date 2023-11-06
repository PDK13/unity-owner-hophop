using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //
        Instance = this;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public IEnumerator ISetWrite(TextMeshProUGUI TextMessPro, MessageConfig MessageData)
    {
        foreach (MessageConfigSingle MessageSingle in MessageData.List)
        {
            bool ColorFormat = false;
            //
            string Text = MessageSingle.Text;
            //
            if (Text == null)
                continue;
            //
            if (Text != "")
            {
                foreach (char MessageChar in Text)
                {
                    //TEXT:
                    TextMessPro.text += MessageChar;
                    //
                    //COLOR:
                    if (!ColorFormat && MessageChar == '<')
                    {
                        ColorFormat = true;
                        continue;
                    }
                    else
                    if (ColorFormat && MessageChar == '>')
                    {
                        ColorFormat = false;
                        continue;
                    }
                    //
                    //DELAY:
                    if (ColorFormat)
                        continue;
                    //
                    //if (MessageSingle.DelayFinal > 0 && (MessageChar == '\n'))
                    //    yield return new WaitForSeconds(MessageSingle.DelayFinal);
                    //else
                    if (MessageSingle.DelaySpace > 0 && (MessageChar == ' '))
                        yield return new WaitForSeconds(MessageSingle.DelaySpace);
                    else
                    if (MessageSingle.DelayAlpha > 0)
                        yield return new WaitForSeconds(MessageSingle.DelayAlpha);
                }
            }
            //
            if (MessageSingle.DelayFinal > 0)
                yield return new WaitForSeconds(MessageSingle.DelayFinal);
        }
    }
}