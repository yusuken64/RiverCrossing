using System;
using System.Collections.Generic;
using UnityEngine;

public class SingletonSaveData : MonoBehaviour
{
    public static SingletonSaveData Instance { get; private set; }
    public ISaveManager SaveManager { get; internal set; }

    public SaveData SaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    internal void ResetData()
    {
        SaveData = SaveData.DefaultData;
    }

    internal void Load()
    {
        var saveData = SaveManager.Load();
        if (saveData == null)
        {
            ResetData();
        }
        else
        {
            SaveData = saveData;
        }

        //TODO refactor to SaveDataChanged;
        AudioManager.Instance.ApplicationInitialized(SaveData);
    }

    internal void Save()
    {
        SaveManager.Save(SaveData);
    }
}

[Serializable]
public class SaveData
{
    public ApplicationData ApplicationData;
    public GameData GameData;

    public static SaveData DefaultData => new SaveData()
    {
        ApplicationData = new()
        {
            BGMVolume = 0.5f,
            SFXVolume = 0.5f
        },
        GameData = new()
        {
            ClearedStageIds = new()
        }
    };
}

[Serializable]
public class ApplicationData
{
    public float SFXVolume;
    public float BGMVolume;
}

[Serializable]
public class GameData
{
    public List<int> ClearedStageIds = new();
}
