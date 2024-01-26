#if UNITY_EDITOR

using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

public class AddressablesTool : EditorWindow
{
    private string PATH_ADDRESSABLES_SETTING = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

    [MenuItem("Tools/Addressables")]
    public static void Init()
    {
        GetWindow<AddressablesTool>("Addressables");
    }

    private void OnEnable()
    {

    }

    private void OnGUI()
    {
        if (QUnityEditor.SetButton("Refresh"))
            SetAddressablesRefresh();
    }

    private void SetAddressablesRefresh()
    {
        AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(PATH_ADDRESSABLES_SETTING) as AddressableAssetSettings;
        //
        //List<string> labels = settings.GetLabels();
        //foreach (string label in labels)
        //    settings.RemoveLabel(label, true);
        //
        AssetDatabase.Refresh();
        if (settings == null)
            settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(PATH_ADDRESSABLES_SETTING) as AddressableAssetSettings;
        //
        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group.IsDefaultGroup())
            {
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    string name = Path.GetFileNameWithoutExtension(entry.AssetPath);
                    settings.AddLabel(name);
                    entry.SetLabel(name, true);
                    entry.SetAddress(entry.AssetPath);
                }
            }
        }
        //
        AssetDatabase.Refresh();
        //
        if (settings == null)
            settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(PATH_ADDRESSABLES_SETTING) as AddressableAssetSettings;
        //
        foreach (AddressableAssetGroup group in settings.groups)
        {
            var groupSchema = group.GetSchema<BundledAssetGroupSchema>();
            if (groupSchema != null)
            {
                groupSchema.UseAssetBundleCache = false;
                groupSchema.UseAssetBundleCrc = false;
                groupSchema.UseUnityWebRequestForLocalBundles = true;
                FieldInfo field = groupSchema.GetType().GetField("m_AssetBundleProviderType", BindingFlags.NonPublic | BindingFlags.Instance);
                SerializedType t = new SerializedType();
                field.SetValue(groupSchema, t);
            }
        }
        //
        AssetDatabase.Refresh();
    }
}

#endif