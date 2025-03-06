using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    public PuzzleDefinition PuzzleDefinition;
    public Action<PuzzleDefinition> LevelSelected { get; internal set; }

    internal void Setup(int i, PuzzleDefinition puzzleDefinition)
    {
        ButtonText.text = i.ToString();
        PuzzleDefinition = puzzleDefinition;
    }

    public void On_Clicked()
    {
        LevelSelected?.Invoke(PuzzleDefinition);
    }
}
