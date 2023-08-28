using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QGameObject
{
    #region ==================================== Create

    public static GameObject SetCreate(GameObject Prepab, Transform Parent = null, bool WorldStay = true)
    {
        GameObject GameObject = MonoBehaviour.Instantiate(Prepab);

        if (Parent != null)
        {
            GameObject.transform.SetParent(Parent, WorldStay);
        }

        GameObject.transform.position = Prepab.transform.position;
        GameObject.transform.localScale = Prepab.transform.localScale;

        return GameObject;
    }

    public static GameObject SetCreate(string Name, Transform Parent = null, bool WorldStay = true)
    {
        GameObject GameObject = new GameObject(Name);

        if (Parent != null)
        {
            GameObject.transform.SetParent(Parent, WorldStay);
        }

        GameObject.transform.position = Vector3.zero;
        GameObject.transform.localScale = Vector3.one;

        return GameObject;
    }

    #endregion

    #region ==================================== Destroy

    public static void SetDestroy(Object From)
    {
        //Remove GameObject from Scene or Component from GameObject!!

        if (From == null)
        {
            return;
        }

        if (Application.isEditor)
        {
            if (Application.isPlaying)
            {
                MonoBehaviour.Destroy(From);
            }
            else
            {
                MonoBehaviour.DestroyImmediate(From);
            }
        }
        else
        {
            MonoBehaviour.Destroy(From);
        }
    }

    #endregion

    #region ==================================== Transform

    public static void SetIndex(Transform From, int Index)
    {
        if (From.parent != null)
        {
            if (Index < 0 || Index > From.parent.childCount - 1)
            {
                return;
            }
        }

        From.SetSiblingIndex(Index);
    }

    #endregion

    #region ==================================== Message

    public static void SetMessage(GameObject From, string MethodeName, SendMessageOptions Option = SendMessageOptions.RequireReceiver)
    {
        From.SendMessage(MethodeName, Option);
    }

    #endregion

    #region ==================================== GameObject

    public static string GetNameReplaceClone(string Name)
    {
        return Name.Replace("(Clone)", "");
    }

#if UNITY_EDITOR

    public static bool GetCheckPrefab(GameObject From)
    {
        //Check if GameObject is a Prefab?!
#if UNITY_2018_3_OR_NEWER
        return PrefabUtility.IsPartOfAnyPrefab(From);
#else
	        return PrefabUtility.GetPrefabType(go) != PrefabType.None;
#endif
    }

#endif

#if UNITY_EDITOR

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public static GameObject SetFocus(GameObject From)
    {
        return Selection.activeGameObject = From;
    }

    ///<summary>
    ///Caution: Unity Editor only!
    ///</summary>
    public static GameObject GetFocus()
    {
        return Selection.activeGameObject;
    }

#endif

    #endregion
}

public class QComponent
{
    #region ==================================== Primary

    public static T GetComponent<T>(GameObject From) where T : Component
    {
        //Get Component from GameObject. If null, Add Component to GameObject.

        if (From.GetComponent<T>() == null)
        {
            return From.AddComponent<T>();
        }
        else
        {
            return From.GetComponent<T>();
        }
    }

    public static T GetComponent<T>(Transform From) where T : Component
    {
        //Get Component from GameObject. If null, Add Component to GameObject.

        if (From.GetComponent<T>() == null)
        {
            return From.gameObject.AddComponent<T>();
        }
        else
        {
            return From.gameObject.GetComponent<T>();
        }
    }

    #endregion

    #region ==================================== Button

    public static void SetButton(UnityEngine.UI.Button From, UnityAction Action)
    {
        //Add an void methode to Action!!

        //Caution: Some version of Unity might not run this after build app!!

        From.onClick.AddListener(Action);
    }

    #endregion
}

public class QSprite
{
    #region ==================================== Sprite

    public static Vector2 GetSpriteSizePixel(Sprite From)
    {
        return GetSpriteSizeUnit(From) * GetSpritePixelPerUnit(From) * 1.0f;
    }

    public static Vector2 GetSpriteSizeUnit(Sprite From)
    {
        return From.bounds.size * 1.0f;
    }

    public static float GetSpritePixelPerUnit(Sprite From)
    {
        return From.pixelsPerUnit * 1.0f;
    }

    #endregion

    #region ==================================== Sprite Renderer

    //...

    #endregion

    #region ==================================== Border

    public static Vector2 GetBorderPos(SpriteRenderer From, params Direction[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;

        //Caculate Position from Value Data
        foreach (Direction DirBound in Bound)
        {
            switch (DirBound)
            {
                case Direction.Up:
                    Pos.y += Size.y / 2;
                    break;
                case Direction.Down:
                    Pos.y -= Size.y / 2;
                    break;
                case Direction.Left:
                    Pos.x -= Size.x / 2;
                    break;
                case Direction.Right:
                    Pos.x += Size.x / 2;
                    break;
            }
        }
        return Pos;
    }

    public static Vector2 GetBorderPos(SpriteRenderer From, params DirectionX[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;

        //Caculate Position from Value Data
        foreach (DirectionX DirBound in Bound)
        {
            switch (DirBound)
            {
                case DirectionX.Left:
                    Pos.x -= Size.x / 2;
                    break;
                case DirectionX.Right:
                    Pos.x += Size.x / 2;
                    break;
            }
        }
        return Pos;
    }

    public static Vector2 GetBorderPos(SpriteRenderer From, params DirectionY[] Bound)
    {
        //Primary Value: Collider
        Vector2 Pos = From.bounds.center;
        Vector2 Size = From.bounds.size;

        //Caculate Position from Value Data
        foreach (DirectionY DirBound in Bound)
        {
            switch (DirBound)
            {
                case DirectionY.Up:
                    Pos.y += Size.y / 2;
                    break;
                case DirectionY.Down:
                    Pos.y -= Size.y / 2;
                    break;
            }
        }
        return Pos;
    }

    #endregion

    #region ==================================== Texture

    //Texture can be used for Window Editor (Button)

    //Ex:
    //Window Editor:
    //Texture2D Texture = QSprite.GetTextureConvert(Sprite);
    //GUIContent Content = new GUIContent("", (Texture)Texture);
    //GUILayout.Button(Content());

    public static Texture2D GetTextureConvert(Sprite From)
    {
        if (From.texture.isReadable == false)
        {
            return null;
        }

        Texture2D Texture = new Texture2D((int)From.rect.width, (int)From.rect.height);

        Color[] ColorPixel = From.texture.GetPixels(
            (int)From.textureRect.x,
            (int)From.textureRect.y,
            (int)From.textureRect.width,
            (int)From.textureRect.height);
        Texture.SetPixels(ColorPixel);
        Texture.Apply();
        return Texture;
    }

    public static string GetTextureConvertName(Sprite From)
    {
        return GetTextureConvert(From).name;
    }

    #endregion
}