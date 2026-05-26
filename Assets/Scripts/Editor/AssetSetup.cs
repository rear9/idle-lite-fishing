using UnityEditor;
using UnityEngine;

public static class AssetSetup
{
    [InitializeOnLoadMethod]
    private static void EnsureDefaultAssets()
    {
        EnsureAsset<EconomyBalanceData>("Assets/ScriptableObjects/Economy/EconomyBalanceData.asset");
    }

    private static void EnsureAsset<T>(string path) where T : ScriptableObject
    {
        T existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null)
            return;

        T asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"[AssetSetup] Created {typeof(T).Name} at {path}");
    }
}
