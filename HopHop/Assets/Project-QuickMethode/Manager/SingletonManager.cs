using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (SingletonManager<T>.Instance != null)
        {
            Debug.Log("[Singleton] There're more than 1 singleton instance, so destroy this!");
            Destroy(this.gameObject);
            return;
        }
        if (Application.isPlaying)
            DontDestroyOnLoad(this.gameObject);
        Instance = GetComponent<T>();
    }
}