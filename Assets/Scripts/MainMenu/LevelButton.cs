using System;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    public PuzzleDefinition PuzzleDefinition;
    public Action<PuzzleDefinition> LevelSelected { get; internal set; }
    public GameObject ClearedObject;

    internal void Setup(PuzzleDefinition puzzleDefinition)
    {
        ButtonText.text = puzzleDefinition.PuzzleNum.ToString();
        PuzzleDefinition = puzzleDefinition;

        var cleared = SingletonSaveData.Instance.SaveData.GameData.ClearedStageIds.Contains(puzzleDefinition.PuzzleNum);
        ClearedObject.SetActive(cleared);
    }

    public void On_Clicked()
    {
        LevelSelected?.Invoke(PuzzleDefinition);
    }
}
