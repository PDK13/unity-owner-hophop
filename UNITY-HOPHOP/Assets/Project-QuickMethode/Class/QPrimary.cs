using System.Linq;
using System.Text;
using UnityEngine;

#region Primary

public class QMath
{
    #region ==================================== Sum

    public static int GetSum(params int[] Value)
    {
        return Value.Sum();
    }

    public static float GetSum(params float[] Value)
    {
        return Value.Sum();
    }

    #endregion

    #region ==================================== Bit

    public static int GetBitIndex(int BitValue32)
    {
        return (int)Mathf.Log(BitValue32, 2); //BitValue32 = 2 ^ BitIndex
    }

    public static int GetBitValue32(int BitIndex)
    {
        return (int)Mathf.Pow(2, BitIndex); //BitValue32 = 2 ^ BitIndex
    }

    #endregion
}

#endregion

#region Data & Varible

public class QEmail
{
    public static bool GetEmail(string EmailCheck)
    {
        //Check Not Invalid
        if (!GetEmailNotInvalid(EmailCheck))
        {
            return false;
        }

        //Lower AIL
        EmailCheck = EmailCheck.ToLower();

        return
            GetEmailGmail(EmailCheck) &&
            GetEmailYahoo(EmailCheck);
    }

    private static bool GetEmailNotInvalid(string EmailCheck)
    {
        //Check SPACE
        if (EmailCheck.Contains(" "))
        {
            return false;
        }

        //Check @
        bool CheckAAExist = false;
        for (int i = 0; i < EmailCheck.Length; i++)
        {
            if (!CheckAAExist && EmailCheck[i] == '@')
            {
                CheckAAExist = true;
            }
            else
            if (CheckAAExist && EmailCheck[i] == '@')
            {
                return false;
            }
        }
        if (!CheckAAExist)
        {
            return false;
        }

        //All Check Done
        return true;
    }

    private static bool GetEmailGmail(string EmailCheck)
    {
        //Check if GMAIL
        if (EmailCheck.Contains("@gmail.com"))
        {
            //Get ASCII
            byte[] ba_Ascii = Encoding.ASCII.GetBytes(EmailCheck);

            //First Character (Just '0-9' and 'a-z')
            if (ba_Ascii[0] >= 48 && ba_Ascii[0] <= 57 ||
                ba_Ascii[0] >= 97 && ba_Ascii[0] <= 122)
            {
                //Next Character (Just '0-9' and 'a-z' and '.')
                for (int i = 1; i < EmailCheck.Length; i++)
                {
                    if (EmailCheck[i] == '@')
                    {
                        break;
                    }

                    if (ba_Ascii[i] >= 48 && ba_Ascii[i] <= 57 ||
                        ba_Ascii[i] >= 97 && ba_Ascii[i] <= 122 ||
                        EmailCheck[i] == '.')
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        //All Check Done
        return true;
    }

    private static bool GetEmailYahoo(string EmailCheck)
    {
        //Check if GMAIL
        if (EmailCheck.Contains("@yahoo.com"))
        {
            //Get ASCII
            byte[] ba_Ascii = Encoding.ASCII.GetBytes(EmailCheck);

            //First Character (Just 'a-z')
            if (ba_Ascii[0] >= 97 && ba_Ascii[0] <= 122)
            {
                //Next Character (Just '0-9' and 'a-z' and '.' and '_')
                for (int i = 1; i < EmailCheck.Length; i++)
                {
                    if (EmailCheck[i] == '@')
                    {
                        break;
                    }

                    if (ba_Ascii[i] >= 48 && ba_Ascii[i] <= 57 ||
                        ba_Ascii[i] >= 97 && ba_Ascii[i] <= 122 ||
                        EmailCheck[i] == '.' ||
                        EmailCheck[i] == '_')
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        //All Check Done
        return true;
    }
}

#endregion

//namespace QuickManager
//{
//    public class QScene
//    {
//        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//        public static void OnAfterSceneLoad()
//        {
//            Debug.Log("[Debug] On After Scene Load!");
//        }
//    }
//}