using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.Video;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

///<summary>
///Caution: Unity Editor only!
///</summary>
public class QAssetsDatabase : QPath
{
    //Folder "Assets" is the main root of all assets in project, that can find any assets from it.

    #region ==================================== Create Folder

    public static string SetCreateFolder(params string[] PathChildInAssets)
    {
        List<string> Path = new List<string>();
        //
        string PathString = "";
        //
        for (int i = 0; i < PathChildInAssets.Length; i++)
        {
            Path.Add(PathChildInAssets[i]);
            //
            QEncypt.GetEncyptAdd('/', PathString, PathChildInAssets[i], out PathString);
            //
            SetCreateFolderExist(Path.ToArray());
        }
        //
        return PathString;
    }

    private static string SetCreateFolderExist(params string[] PathChildInAssets)
    {
        //If Root Folder not Exist, then can't Create new Folder from that Root Folder
        //
        string PathInAssets = "Assets";
        //
        for (int i = 0; i < PathChildInAssets.Length - 1; i++)
            QEncypt.GetEncyptAdd('/', PathInAssets, PathChildInAssets[i], out PathInAssets);
        //
        string PathFolderInAssets = PathChildInAssets[PathChildInAssets.Length - 1];

        if (QPath.GetPathFolderExist(PathType.Assets, PathChildInAssets))
        {
            //Debug.LogWarningFormat("[Debug] Folder Exist!!\n{0}", PathInAssets + "/" + PathFolderInAssets);
            return "";
        }
        //
        try
        {
            string PathString = AssetDatabase.CreateFolder(PathInAssets, PathFolderInAssets);
            //
            SetRefresh();
            //
            return AssetDatabase.GUIDToAssetPath(PathString);
        }
        catch
        {
            //Debug.LogWarningFormat("[Debug] Root Folder not Exist!!\n{0}", PathInAssets + "/" + PathFolderInAssets);
            return "";
        }
    }

    #endregion

    #region ==================================== Delete

    public static void SetDelete(PathType PathType, params string[] PathChild)
    {
        FileUtil.DeleteFileOrDirectory(QPath.GetPath(PathType, PathChild) + ".meta");
        FileUtil.DeleteFileOrDirectory(QPath.GetPath(PathType, PathChild));
        //
        SetRefresh();
    }

    #endregion

    #region ==================================== Refresh

    public static void SetRefresh()
    {
        AssetDatabase.Refresh();
    }

    #endregion

    #region ==================================== Get

    public static List<AnimationClip> GetAnimationClip(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<AnimationClip> ObjectsFound = new List<AnimationClip>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:AnimationClip"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            AnimationClip ObjectFound = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<AudioClip> GetAudioClip(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<AudioClip> ObjectsFound = new List<AudioClip>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:AudioClip"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            AudioClip ObjectFound = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<Font> GetFont(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<Font> ObjectsFound = new List<Font>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:Font"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            Font ObjectFound = AssetDatabase.LoadAssetAtPath<Font>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<Material> GetMaterial(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<Material> ObjectsFound = new List<Material>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:Material"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            Material ObjectFound = AssetDatabase.LoadAssetAtPath<Material>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<PhysicMaterial> GetPhysicMaterial(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<PhysicMaterial> ObjectsFound = new List<PhysicMaterial>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:PhysicMaterial"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            PhysicMaterial ObjectFound = AssetDatabase.LoadAssetAtPath<PhysicMaterial>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<Texture> GetTexture(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<Texture> ObjectsFound = new List<Texture>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:Texture"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            Texture ObjectFound = AssetDatabase.LoadAssetAtPath<Texture>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<VideoClip> GetVideoClip(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<VideoClip> ObjectsFound = new List<VideoClip>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:VideoClip"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            VideoClip ObjectFound = AssetDatabase.LoadAssetAtPath<VideoClip>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<GameObject> GetPrefab(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<GameObject> ObjectsFound = new List<GameObject>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:Prefab"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            GameObject ObjectFound = AssetDatabase.LoadAssetAtPath<GameObject>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<Sprite> GetSprite(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<Sprite> ObjectsFound = new List<Sprite>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:Sprite"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            Sprite ObjectFound = AssetDatabase.LoadAssetAtPath<Sprite>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    public static List<RuntimeAnimatorController> GetAnimatorController(string NameSpecial, string PathAssets = "Assets/")
    {
        if (PathAssets == null)
            PathAssets = "Assets/";
        else
        if (PathAssets == "")
            PathAssets = "Assets/";
        else
        if (!GetPathFolderExist(PathAssets))
            return null;
        //
        List<RuntimeAnimatorController> ObjectsFound = new List<RuntimeAnimatorController>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameSpecial, "t:AnimatorController"), new string[] { PathAssets });
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            RuntimeAnimatorController ObjectFound = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AssetsSinglePath);
            ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    #endregion
}

#endif