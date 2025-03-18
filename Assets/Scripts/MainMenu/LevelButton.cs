using System;
using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    public PuzzleDefinition PuzzleDefinition;
    public Action<PuzzleDefinition> LevelSelected { get; internal set; }

    internal void Setup(PuzzleDefinition puzzleDefinition)
    {
        ButtonText.text = puzzleDefinition.PuzzleNum.ToString();
        PuzzleDefinition = puzzleDefinition;
    }

    public void On_Clicked()
    {
        LevelSelected?.Invoke(PuzzleDefinition);
    }
}
