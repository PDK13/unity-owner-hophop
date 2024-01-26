using UnityEngine;

public class QResources
{
    //NOTE:
    //Folder(s) "Resources" can be created everywhere from root "Assests/*", that can be access by Unity or Application

    //BEWARD:
    //All content(s) in folder(s) "Resources" will be builded to Application, even they ightn't be used in Build-Game Application

    public static GameObject[] GetPrefab(params string[] PathChildInResources)
    {
        string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
        GameObject[] LoadArray = Resources.LoadAll<GameObject>(PathInResources);
        return LoadArray;
    }

    public static Sprite[] GetSprite(params string[] PathChildInResources)
    {
        string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
        Sprite[] LoadArray = Resources.LoadAll<Sprite>(PathInResources);
        return LoadArray;
    }

    public static TextAsset[] GetTextAsset(params string[] PathChildInResources)
    {
        string PathInResources = QPath.GetPath(QPath.PathType.None, PathChildInResources);
        TextAsset[] LoadArray = Resources.LoadAll<TextAsset>(PathInResources);
        return LoadArray;
    }
}