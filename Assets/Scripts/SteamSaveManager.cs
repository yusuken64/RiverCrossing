#if UNITY_STANDALONE_WIN
using UnityEngine;

public class SteamSaveManager : ISaveManager
{
    private readonly string filePath = Application.persistentDataPath + "/SteamSaveData.json";

    //public void Save(object data)
    //{
    //    string json = JsonUtility.ToJson(data, true);
    //    //File.WriteAllText(filePath, json);
    //    Debug.Log("Saved data for Steam: " + filePath);
    //}

    //public T Load<T>()
    //{
    //    if (File.Exists(filePath))
    //    {
    //        string json = File.ReadAllText(filePath);
    //        return JsonUtility.FromJson<T>(json);
    //    }
    //    Debug.LogWarning("Save file not found for Steam.");
    //    return default;
    //}

    public void Save(SaveData data)
    {
    }

    public SaveData Load()
    {
        return new();
    }
}
#endif