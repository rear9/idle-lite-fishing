using System;
using System.IO;
using UnityEngine;

public class SaveService : MonoBehaviour
{
    [SerializeField] private string _fileName = "save.json";

    private string Path => Application.persistentDataPath + "/" + _fileName;

    public void Save(GameState state)
    {
        try
        {
            state.lastSaveTime = DateTime.UtcNow.ToString("O");
            File.WriteAllText(Path, JsonUtility.ToJson(state, true));
            Debug.Log("[SaveService] Saved.");
        }
        catch (Exception e) { Debug.LogError("[SaveService] " + e.Message); }
    }

    public GameState Load()
    {
        try
        {
            if (!File.Exists(Path)) return null;

            GameState state = JsonUtility.FromJson<GameState>(File.ReadAllText(Path));
            if (state == null) { Debug.LogWarning("[SaveService] Corrupted save."); return null; }
            if (state.saveVersion != "1.0.0")
                Debug.LogWarning($"[SaveService] Version mismatch: {state.saveVersion}");

            return state;
        }
        catch (Exception e) { Debug.LogError("[SaveService] " + e.Message); return null; }
    }

    public void DeleteSave()
    {
        try { if (File.Exists(Path)) File.Delete(Path); }
        catch (Exception e) { Debug.LogError("[SaveService] " + e.Message); }
    }
}
