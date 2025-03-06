using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public LevelButton ButtonPrefab;
    public Transform ButtonContainer;

    public List<PuzzleDefinition> Puzzles;

    private void Start()
    {
        Game game = FindObjectOfType<Game>();
        game.GameCanvas.SetActive(false);
        game.ClearAllActors();
        Setup();
    }

    public void Setup()
    {
        foreach(Transform child in ButtonContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < Puzzles.Count; i++)
        {
            PuzzleDefinition puzzleDefinition = Puzzles[i];
            var newButton = Instantiate(ButtonPrefab, ButtonContainer);
            newButton.Setup(i + 1, puzzleDefinition);
            newButton.LevelSelected = (puzzle) => LevelClicked(puzzle);
        }
    }

    private void LevelClicked(PuzzleDefinition puzzle)
    {
        this.gameObject.SetActive(false);
        Game game = FindObjectOfType<Game>();

        game.PuzzleDefinition = puzzle;
        game.SetupGame();
    }

    public void NextPuzzle(PuzzleDefinition currentPuzzle)
    {
        var index = Puzzles.IndexOf(currentPuzzle);
        var nextPuzzle = Puzzles[index + 1];

        LevelClicked(nextPuzzle);
    }
}
