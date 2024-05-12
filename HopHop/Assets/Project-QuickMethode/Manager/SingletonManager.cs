using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected bool SetInstance()
    {
        if (Instance != null)
        {
            Debug.LogWarning("There're more than once Singeleton, so not get this instance");
            return false;
        }
        if (Application.isPlaying)
            DontDestroyOnLoad(this.gameObject);
        Instance = GetComponent<T>();
        return true;
    }
}