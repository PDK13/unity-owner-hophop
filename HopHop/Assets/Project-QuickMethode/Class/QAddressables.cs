using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections.Generic;

public class QAddressables
{
    //Assets

    public static AsyncOperationHandle<T> SetAssetsLoad<T>(AssetReference Asset)
    {
        return Asset.LoadAssetAsync<T>();
    }

    public static AsyncOperationHandle<T> SetAssetsLoad<T>(string LabelOrPath)
    {
        return Addressables.LoadAssetAsync<T>(LabelOrPath);
    }

    public static AsyncOperationHandle<IList<T>> SetAssetsLoadList<T>(string Label)
    {
        return Addressables.LoadAssetsAsync<T>(Label, null);
    }

    public static void SetAssetsRelease<T>(AsyncOperationHandle<T> Asset)
    {
        Asset.Completed += (AsyncOperationHandle<T> Handle) => Addressables.Release(Asset);
    }

    //Scene

    public static AsyncOperationHandle<SceneInstance> SetSceneLoad(AssetReference Asset, bool ActiveOnLoad = false)
    {
        return Asset.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Single, ActiveOnLoad);
    }

    public static AsyncOperationHandle<SceneInstance> SetSceneLoad(string LabelOrPath, bool ActiveOnLoad = false)
    {
        return Addressables.LoadSceneAsync(LabelOrPath, UnityEngine.SceneManagement.LoadSceneMode.Single, ActiveOnLoad);
    }

    public static void SetSceneActive(AsyncOperationHandle<SceneInstance> Scene)
    {
        Scene.Completed += (AsyncOperationHandle<SceneInstance> Handle) => Scene.Result.ActivateAsync();
    }

    //GameObject

    public static AsyncOperationHandle<GameObject> SetPrefabInstantiate(AssetReference Asset)
    {
        return Asset.InstantiateAsync();
    }

    public static AsyncOperationHandle<GameObject> SetPrefabInstantiate(string LabelOrPath)
    {
        return Addressables.InstantiateAsync(LabelOrPath);
    }

    public static void SetPrefabRelease(AsyncOperationHandle<GameObject> Prefab)
    {
        Prefab.Completed += (AsyncOperationHandle<GameObject> Handle) => Addressables.ReleaseInstance(Prefab);
    }
}