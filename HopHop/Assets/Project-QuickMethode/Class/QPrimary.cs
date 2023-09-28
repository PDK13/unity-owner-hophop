using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

#region Primary

public class QVector
{
    #region ==================================== Middle (Trung điểm)

    public static Vector2 GetMiddlePoint(Vector2 PointA, Vector2 PointB)
    {
        return new Vector2((PointA.x + PointB.x) / 2, (PointA.y + PointB.y) / 2);
    }

    public static Vector3 GetMiddlePoint(Vector3 PointA, Vector3 PointB)
    {
        return new Vector3((PointA.x + PointB.x) / 2, (PointA.y + PointB.y) / 2, (PointA.z + PointB.z) / 2);
    }

    #endregion

    #region ==================================== Reflech (Phản xạ)

    public static Vector2 GetDirReflect(Vector2 Dir, Collision2D Collision)
    {
        //Get Dir Reflect (Phản xạ) from Dir to!!
        return Vector2.Reflect(Dir, Collision.contacts[0].normal);
    }

    public static Vector3 GetDirReflect(Vector3 Dir, Collision Collision)
    {
        //Get Dir Reflect (Phản xạ) from Dir to!!
        return Vector3.Reflect(Dir, Collision.contacts[0].normal);
    }

    #endregion
}

public class QCircle
{
    /* IMPORTANCE NOTE:
     * Deg can be understand as Euler in Unity.
     * Deg Caculated from X-Axis Right of World.
     * Pos Caculated from Center Zero of World (Should use Local Pos instead World Pos).
     * Deg caculated from this class all base on 0 to 360 degree.
     * Command "transform.eulerAngles" use Deg180 in Editor and Deg360 in Script.
    */

    #region ==================================== Primary

    public static float GetDeg180(float Deg360)
    {
        return Deg360 <= 180 ? Deg360 : -(360 - Deg360);
    }

    public static float GetDeg360(float Deg180)
    {
        return Deg180 <= 180 && Deg180 >= 0 ? Deg180 : 360 + Deg180;
    }

    #endregion

    #region ==================================== Pos by Deg

    public static Vector3 GetPosXY(float Deg360, float Radius)
    {
        //Get Pos from Center (0; 0)

        return new Vector3(Mathf.Cos(Deg360 * Mathf.Deg2Rad), Mathf.Sin(Deg360 * Mathf.Deg2Rad), 0) * Radius;
    }

    public static Vector3 GetPosXZ(float Deg360, float Radius)
    {
        //Get Pos from Center (0; 0; 0)

        return new Vector3(Mathf.Cos(Deg360 * Mathf.Deg2Rad), 0, Mathf.Sin(Deg360 * Mathf.Deg2Rad)) * Radius;
    }

    #endregion

    #region ==================================== Deg by Pos & Dir

    public static float GetDeg360(Vector2 Dir)
    {
        //Get Deg from Center (0; 0)
        float Deg = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        return Deg >= 0 ? Deg : 360 + Deg;
    }

    public static float GetDeg360(Vector2 Center, Vector2 Pos)
    {
        //Get Deg from Center
        return GetDeg360(Pos - Center);
    }

    #endregion

    #region ==================================== Opposite (Đối diện)

    public static float GetDegOppositeUD(float Deg360)
    {
        if (0f == Deg360 || Deg360 == 360f)
        {
            return 180f;
        }

        if (0f < Deg360 && Deg360 < 90f)
        {
            return 90f + (90f - Deg360);
        }

        if (Deg360 == 90f)
        {
            return 90f;
        }

        if (90f < Deg360 && Deg360 < 180f)
        {
            return 90f - (Deg360 - 90f);
        }

        if (Deg360 == 180f)
        {
            return 0f;
        }

        if (180f < Deg360 && Deg360 < 270f)
        {
            return 270f + (270f - Deg360);
        }

        if (Deg360 == 270f)
        {
            return 270f;
        }

        if (270f < Deg360 && Deg360 < 360f)
        {
            return 270f - (Deg360 - 270f);
        }

        Debug.LogError("Sonething wrong here!");
        return 90f;
    }

    public static float GetDegOppositeLR(float Deg360)
    {
        if (0f == Deg360 || Deg360 == 360f)
        {
            return 0f;
        }

        if (0f < Deg360 && Deg360 < 90f)
        {
            return 360f - (0f + Deg360);
        }

        if (Deg360 == 90f)
        {
            return 270f;
        }

        if (90f < Deg360 && Deg360 < 180f)
        {
            return 180f + (180f - Deg360);
        }

        if (Deg360 == 180f)
        {
            return 180f;
        }

        if (180f < Deg360 && Deg360 < 270f)
        {
            return 180f + (Deg360 - 180f);
        }

        if (Deg360 == 270f)
        {
            return 90f;
        }

        if (270f < Deg360 && Deg360 < 360f)
        {
            return 0f - (360f - Deg360);
        }

        Debug.LogError("Sonething wrong here!");
        return 0f;
    }

    #endregion

    #region ==================================== Deg to Target: Use for Face to a Target

    public static float GetDegTargetOffset(Transform Body, Vector3 BodyDir, Transform Target)
    {
        //Get value of deg remain to target:
        //- Value > 0 mean rotate follow anti-clockwise to head target
        //- Value < 0 mean rotate follow clockwise to head target
        Vector2 Dir = (Target.position - Body.position).normalized;
        return Vector3.SignedAngle(BodyDir, Dir, Vector3.forward);
    }

    #endregion
}

public class QColor
{
    #region ==================================== Color

    //Key "ref" use to immediately save value to primary varible param!

    public static void SetColor(ref Color Color, float A)
    {
        Color = new Color(Color.r, Color.g, Color.b, A);
    }

    public static Color GetColor(Color Color, float A)
    {
        return new Color(Color.r, Color.g, Color.b, A);
    }

    #endregion

    #region ==================================== Color - Hex Color

    private static string GetColorHex(Color Color)
    {
        return ColorUtility.ToHtmlStringRGB(Color);
    }

    public static string GetColorHexFormat(Color Color)
    {
        string ColorHex = GetColorHex(Color);
        string ColorHexCode = string.Format("#{0}", ColorHex);
        return ColorHexCode;
    }

    public static string GetColorHexFormat(Color Color, string Text)
    {
        string ColorHex = GetColorHex(Color);
        string TextFormat = string.Format("<#{0}>{1}</color>", ColorHex, Text);
        return TextFormat;
    }

    #endregion

    #region ==================================== Color - Mesh Renderer & Material

    //Can be applied to Spine Material!!

    public static void SetMaterial(MeshRenderer MessRenderer, float Alpha)
    {
        if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
        {
            return;
        }

        Color Color = MessRenderer.material.color;
        SetColor(ref Color, Alpha);
        MessRenderer.material.color = Color;
    }

    public static void SetMaterial(MeshRenderer MessRenderer, Color Color, float Alpha)
    {
        if (!MessRenderer.sharedMaterial.HasProperty("_Color"))
        {
            return;
        }

        SetColor(ref Color, Alpha);
        MessRenderer.material.color = Color;
    }

    public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color)
    {
        //Shader of Material must set to "Tint"!!

        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

        int IdColor = Shader.PropertyToID("_Color");
        int IdBlack = Shader.PropertyToID("_Black");

        MaterialPropertyBlock.SetColor(IdColor, Color);
        MaterialPropertyBlock.SetColor(IdBlack, Color.black); //Should be "Color.black"!!

        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    public static void SetMaterialTint(MeshRenderer MessRenderer, Color Color, Color Black)
    {
        //Shader of Material must set to "Tint"!!

        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

        int IdColor = Shader.PropertyToID("_Color");
        int IdBlack = Shader.PropertyToID("_Black");

        MaterialPropertyBlock.SetColor(IdColor, Color);
        MaterialPropertyBlock.SetColor(IdBlack, Black); //Should be "Color.black"!!

        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    public static void SetMaterialFill(MeshRenderer MessRenderer, Color FillColor, float FillPhase = 1)
    {
        //Shader of Material must set to "Fill"!!

        MaterialPropertyBlock MaterialPropertyBlock = new MaterialPropertyBlock();

        MessRenderer.GetPropertyBlock(MaterialPropertyBlock);

        int IdFillColor = Shader.PropertyToID("_FillColor");
        int IdFillPhase = Shader.PropertyToID("_FillPhase");

        MaterialPropertyBlock.SetColor(IdFillColor, FillColor);
        MaterialPropertyBlock.SetFloat(IdFillPhase, FillPhase);

        MessRenderer.SetPropertyBlock(MaterialPropertyBlock);
    }

    #endregion

    #region ==================================== Color - SpriteRenderer

    public static void SetSprite(SpriteRenderer SpriteRenderer, float Alpha)
    {
        Color Color = SpriteRenderer.color;
        SetColor(ref Color, Alpha);
        SpriteRenderer.color = Color;
    }

    public static void SetSprite(SpriteRenderer SpriteRenderer, Color Color, float Alpha)
    {
        SetColor(ref Color, Alpha);
        SpriteRenderer.color = Color;
    }

    #endregion

    #region ==================================== Color - Image

    public static void SetSprite(UnityEngine.UI.Image Image, float Alpha)
    {
        Color Color = Image.color;
        SetColor(ref Color, Alpha);
        Image.color = Color;
    }

    public static void SetSprite(UnityEngine.UI.Image Image, Color Color, float Alpha)
    {
        SetColor(ref Color, Alpha);
        Image.color = Color;
    }

    #endregion
}

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