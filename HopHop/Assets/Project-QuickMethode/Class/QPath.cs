using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QPath
{
    #region ==================================== Path Get

    public enum PathType
    {
        None,
        Persistent,
        Assets,
        Resources,
        Document,
        Picture,
        Music,
        Video,
    }

    public enum ExtensionType
    {
        //Script
        cs,
        //Image
        jpg,
        jpeg,
        png,
        gif,
        tiff,
        psd,
        pdf,
        eps,
        ai,
        //Media (Sound & Movie)
        mp3,
        mp4,
        wav,
        ogg,
        //Text
        txt,
        html,
        htm,
        xml,
        bytes,
        json,
        csv,
        yaml,
        fnt,
        md,
    }

    public static string GetPath(PathType PathType, params string[] PathChild)
    {
        string PathFinal = "";

        switch (PathType)
        {
            case PathType.Persistent:
                PathFinal = Application.persistentDataPath;
                break;
            case PathType.Assets:
                PathFinal = Application.dataPath;
                break;
            case PathType.Resources:
                PathFinal = Application.dataPath + @"/resources";
                break;
            case PathType.Document:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                break;
            case PathType.Picture:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                break;
            case PathType.Music:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                break;
            case PathType.Video:
                PathFinal = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                break;
        }
        //
        if (PathChild == null)
            PathChild = new string[1] { "" };
        //
        foreach (string PathChildAdd in PathChild)
            PathFinal = QString.GetSplitAdd('/', PathFinal, PathChildAdd);
        //
        return PathFinal;
    }

    #endregion

    #region ==================================== File Name & Extension

    //primary

    public static string GetFileName(string Name, ExtensionType Extension)
    {
        return string.Format("{0}.{1}", Name, Extension.ToString());
    }

    //Get File Name from Path

    public static string GetFileNameFromPath(string PathPrimary)
    {
        return Path.GetFileNameWithoutExtension(PathPrimary);
    }

    public static string GetFileNameFromPath(PathType PathType, params string[] PathChild)
    {
        return GetFileNameFromPath(GetPath(PathType, PathChild));
    }

    //Get File Extension from Path

    public static string GetFileExtensionFromPath(string PathPrimary)
    {
        return Path.GetExtension(PathPrimary);
    }

    public static string GetFileExtensionFromPath(PathType PathType, params string[] PathChild)
    {
        return GetFileExtensionFromPath(GetPath(PathType, PathChild));
    }

    #endregion

    #region ==================================== Path Pannel

#if UNITY_EDITOR

    //Open

    ///<remarks><b>UnityEditorOnly</b></remarks>
    public static (bool Result, string Path, string Name) GetPathFolderOpenPanel(string Title, string PathPrimary = "")
    {
        string Path = EditorUtility.OpenFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
        List<string> PathDencypt = QString.GetUnSplitString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    ///<remarks><b>UnityEditorOnly</b></remarks>
    public static (bool Result, string Path, string Name) GetPathFileOpenPanel(string Title, string Extension, string PathPrimary = "")
    {
        string Path = EditorUtility.OpenFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, Extension);
        List<string> PathDencypt = QString.GetUnSplitString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    //Save

    ///<remarks><b>UnityEditorOnly</b></remarks>
    public static (bool Result, string Path, string Name) GetPathFolderSavePanel(string Title, string PathPrimary = "")
    {
        string Path = EditorUtility.SaveFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
        List<string> PathDencypt = QString.GetUnSplitString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    ///<remarks><b>UnityEditorOnly</b></remarks>
    public static (bool Result, string Path, string Name) GetPathFileSavePanel(string Title, string Extension, string PathPrimary = "")
    {
        string Path = EditorUtility.SaveFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "", Extension);
        List<string> PathDencypt = QString.GetUnSplitString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

#endif

    #endregion

    #region ==================================== Path Exist

    //File

    public static bool GetPathFileExist(string PathFile)
    {
        return File.Exists(PathFile);
    }

    public static bool GetPathFileExist(PathType PathType, params string[] PathChild)
    {
        return File.Exists(GetPath(PathType, PathChild));
    }

    //Folder

    public static bool GetPathFolderExist(string PathFolder)
    {
        return Directory.Exists(PathFolder);
    }

    public static bool GetPathFolderExist(PathType PathType, params string[] PathChild)
    {
        return Directory.Exists(GetPath(PathType, PathChild));
    }

    #endregion
}