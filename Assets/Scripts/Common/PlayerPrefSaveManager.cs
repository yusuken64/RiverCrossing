#if UNITY_EDITOR || UNITY_ANDROID || UNITY_WEBGL
using UnityEngine;

public class PlayerPrefSaveManager : ISaveManager
{
    private const string PlayerPrefSaveDataKey = "SaveData";

    public void Save(SaveData data)
    {
        SaveToPlayerPrefs(data);
    }

    public SaveData Load()
    {
        return LoadFromPlayerPrefs();
    }

    public static void SaveToPlayerPrefs(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData); // Serialize SaveData to JSON
        PlayerPrefs.SetString(PlayerPrefSaveDataKey, json);   // Store JSON in PlayerPrefs
        PlayerPrefs.Save();                       // Ensure the data is saved immediately
        Debug.Log("SaveData successfully saved to PlayerPrefs.");
    }

    public static SaveData LoadFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(PlayerPrefSaveDataKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefSaveDataKey);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("SaveData successfully loaded from PlayerPrefs.");
            return saveData;
        }
        else
        {
            Debug.LogWarning("No SaveData found in PlayerPrefs.");
            return null;
        }
    }
}
#endif