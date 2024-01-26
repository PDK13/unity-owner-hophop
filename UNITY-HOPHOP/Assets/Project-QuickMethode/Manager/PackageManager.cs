using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif

public class PackageManager : MonoBehaviour
{
#if UNITY_EDITOR

    //NOTE: To get Package Name, go to Window\Package Manager to see them!!
    [SerializeField] private List<string> m_package = new List<string>() { "com.unity.ide.vscode", };

    private IEnumerator Start()
    {
        var pack = Client.List();
        //
        while (!pack.IsCompleted) yield return null;
        //
        foreach (string Package in m_package)
        {
            var haveProgrids = pack.Result.FirstOrDefault(q => q.name == Package);
            if (haveProgrids == null)
                Debug.LogFormat("[PackageManager] Missing {0}", Package);
        }
    }

#endif
}