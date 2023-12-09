using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        foreach (string PathChildAdd in PathChild)
            PathFinal = QEncypt.GetEncyptAdd('/', PathFinal, PathChildAdd);
        //
        return PathFinal;
    }

    #endregion

    #region ==================================== File

    //Name

    public static string GetFileName(string PathPrimary)
    {
        return Path.GetFileNameWithoutExtension(PathPrimary);
    }

    public static string GetFileName(PathType PathType, params string[] PathChild)
    {
        return GetFileName(GetPath(PathType, PathChild));
    }

    //Extension

    public static string GetFileExtension(string PathPrimary)
    {
        return Path.GetExtension(PathPrimary);
    }

    public static string GetFileExtension(PathType PathType, params string[] PathChild)
    {
        return GetFileExtension(GetPath(PathType, PathChild));
    }

    #endregion

    #region ==================================== Path Pannel

#if UNITY_EDITOR

    //Open

    ///<summary><b>(UnityEditorOnly)</b></summary>
    public static (bool Result, string Path, string Name) GetPathFolderOpenPanel(string Title, string PathPrimary = "")
    {
        string Path = EditorUtility.OpenFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    ///<summary><b>(UnityEditorOnly)</b></summary>
    public static (bool Result, string Path, string Name) GetPathFileOpenPanel(string Title, string Extension, string PathPrimary = "")
    {
        string Path = EditorUtility.OpenFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, Extension);
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    //Save

    ///<summary><b>(UnityEditorOnly)</b></summary>
    public static (bool Result, string Path, string Name) GetPathFolderSavePanel(string Title, string PathPrimary = "")
    {
        string Path = EditorUtility.SaveFolderPanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "");
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
        return (Path != "", Path, (PathDencypt.Count > 0) ? PathDencypt[PathDencypt.Count - 1] : "");
    }

    ///<summary><b>(UnityEditorOnly)</b></summary>
    public static (bool Result, string Path, string Name) GetPathFileSavePanel(string Title, string Extension, string PathPrimary = "")
    {
        string Path = EditorUtility.SaveFilePanel(Title, (PathPrimary == "") ? GetPath(PathType.Assets) : PathPrimary, "", Extension);
        List<string> PathDencypt = QEncypt.GetDencyptString('/', Path);
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