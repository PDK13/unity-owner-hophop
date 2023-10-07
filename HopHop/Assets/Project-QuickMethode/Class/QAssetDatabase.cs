using System.Collections.Generic;
using UnityEngine;
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

    public const string ExamplePathAssets = "Assets/Scene";

    #region ==================================== Create Folder

    public static string SetCreateFolder(params string[] PathChildInAssets)
    {
        List<string> Path = new List<string>();

        string PathString = "";

        for (int i = 0; i < PathChildInAssets.Length; i++)
        {
            Path.Add(PathChildInAssets[i]);

            QEncypt.GetEncyptAdd('/', PathString, PathChildInAssets[i], out PathString);

            SetCreateFolderExist(Path.ToArray());
        }

        return PathString;
    }

    private static string SetCreateFolderExist(params string[] PathChildInAssets)
    {
        //If Root Folder not Exist, then can't Create new Folder from that Root Folder

        string PathInAssets = "Assets";

        for (int i = 0; i < PathChildInAssets.Length - 1; i++)
        {
            QEncypt.GetEncyptAdd('/', PathInAssets, PathChildInAssets[i], out PathInAssets);
        }

        string PathFolderInAssets = PathChildInAssets[PathChildInAssets.Length - 1];

        if (QPath.GetPathFolderExist(PathType.Assets, PathChildInAssets))
        {
            //Debug.LogWarningFormat("[Debug] Folder Exist!!\n{0}", PathInAssets + "/" + PathFolderInAssets);

            return "";
        }

        try
        {
            string PathString = AssetDatabase.CreateFolder(PathInAssets, PathFolderInAssets);

            SetRefresh();

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

    public static List<GameObject> GetPrefab(string NameContains, params string[] PathChildInAssets)
    {
        string Path = QPath.GetPath(PathType.Assets, PathChildInAssets);
        //
        if (!GetPathFolderExist(Path))
            return null;
        //
        List<GameObject> ObjectsFound = new List<GameObject>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameContains, "t:prefab"), new string[] { Path });
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

    public static List<Sprite> GetSprite(string NameContains, params string[] PathChildInAssets)
    {
        string Path = QPath.GetPath(PathType.Assets, PathChildInAssets);
        //
        if (!GetPathFolderExist(Path))
            return null;
        //
        List<Sprite> ObjectsFound = new List<Sprite>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(string.Format("{0} {1}", NameContains, "t:sprite"), new string[] { Path });
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

    public static List<RuntimeAnimatorController> GetAnimatorController(string NameContains, params string[] PathChildInAssets)
    {
        if (NameContains == null)
            return null;
        //
        if (NameContains == "")
            return null;
        //
        string Path = QPath.GetPath(PathType.Assets, PathChildInAssets);
        //
        if (!GetPathFolderExist(Path))
            return null;
        //
        List<RuntimeAnimatorController> ObjectsFound = new List<RuntimeAnimatorController>();
        //
        string[] GUIDPathUnityFound = AssetDatabase.FindAssets(NameContains);
        //
        foreach (string GUIDPath in GUIDPathUnityFound)
        {
            string AssetsSinglePath = AssetDatabase.GUIDToAssetPath(GUIDPath);
            RuntimeAnimatorController ObjectFound = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath(AssetsSinglePath, typeof(RuntimeAnimatorController));
            if (ObjectFound != null)
                ObjectsFound.Add(ObjectFound);
        }
        //
        return ObjectsFound;
    }

    #endregion
}

#endif