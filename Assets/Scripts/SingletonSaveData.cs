using System;
using System.Collections.Generic;
using UnityEngine;

public class SingletonSaveData : MonoBehaviour
{
    public static SingletonSaveData Instance { get; private set; }

    public SaveData SaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetData(); //TODO lead from platform
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    internal void ResetData()
    {
        SaveData = new();
    }
}

public class SaveData
{
    public HashSet<int> ClearedStageIds = new();
}